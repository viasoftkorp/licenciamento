using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetSparkleUpdater;
using NetSparkleUpdater.Enums;
using NetSparkleUpdater.SignatureVerifiers;
using Viasoft.Licensing.LicenseServer.Domain.Extensions;

namespace Viasoft.Licensing.LicenseServer.Domain.BackgroundServices
{
    public class UpdateDownloaderBackgroundService: BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UpdateDownloaderBackgroundService> _logger;

        public UpdateDownloaderBackgroundService(IConfiguration configuration, ILogger<UpdateDownloaderBackgroundService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken cancellationToken)
        {
            var skip = _configuration.SkipAutoUpdate();
            if (skip)
            {
                _logger.LogWarning("Will not check for updates because SkipAutoUpdate=true");
                return;
            }
            
            var customServiceName = _configuration["CustomServiceNameUpdater"];
            if (!string.IsNullOrEmpty(customServiceName))
            {
                _logger.LogInformation("We got a custom service name to use when updating, it is {CustomServiceName}", customServiceName);
            }
            
            while (!cancellationToken.IsCancellationRequested)
            {
                var sparkle = new SparkleUpdater(_configuration.GetAutoUpdateAppCastUrl(), new Ed25519Checker(SecurityMode.Strict, _configuration.GetAutoUpdatePublicKey()))
                {
                    RelaunchAfterUpdate = false
                };

                sparkle.LogWriter = new LogWriter(true);

                var updateInfo = await sparkle.CheckForUpdatesQuietly();

                if (updateInfo.Status == UpdateStatus.UpdateAvailable)
                {
                    var appCast = updateInfo.Updates.Aggregate((acc, cast) =>
                    {
                        if (acc.PublicationDate > cast.PublicationDate)
                            return acc;
                        
                        return cast;
                    });

                    sparkle.DownloadFinished += (_, path) =>
                    {
                        Install(path, customServiceName);
                    };

                    await sparkle.InitAndBeginDownload(appCast);

                    var fileName = await sparkle.GetDownloadPathForAppCastItem(appCast);
                    
                    if (File.Exists(fileName) && !sparkle.IsDownloadingItem(appCast))
                        Install(fileName, customServiceName);
                }
                
                await Task.Delay(GetTaskDelayToNextExecution(_configuration.GetAutoUpdateTimeHour()), cancellationToken);
            }
        }

        private static int GetTaskDelayToNextExecution(int hour)
        {
            var now = DateTime.UtcNow;
            var nextDay = now.AddDays(1);
            var nextExecutionDateTime = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, hour, 0, 0, DateTimeKind.Utc);
            var timeDifference = nextExecutionDateTime - now;
            return Convert.ToInt32(timeDifference.TotalMilliseconds);
        }

        private void Install(string filePath, string customServiceName)
        {
            var arguments = $"/C {filePath} /VERYSILENT /LOG";
            if (!string.IsNullOrEmpty(customServiceName))
            {
                arguments += $" /serviceName={customServiceName}";
            }

            var processStartInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = true,
                FileName = "cmd.exe",
                Arguments = arguments
            };

            Process.Start(processStartInfo);
        }
    }
}