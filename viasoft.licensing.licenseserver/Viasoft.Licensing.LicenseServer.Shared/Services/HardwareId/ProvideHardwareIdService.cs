using System.Security.Cryptography;
using System.Threading;
using DeviceId;
using DeviceId.Encoders;
using DeviceId.Formatters;
using Viasoft.Core.IoC.Abstractions;

namespace Viasoft.Licensing.LicenseServer.Shared.Services.HardwareId
{
    public class ProvideHardwareIdService: IProvideHardwareIdService, ISingletonDependency
    {
        private readonly ReaderWriterLockSlim _lock;
        private string _hardwareId;
        
        public ProvideHardwareIdService()
        {
            _lock = new ReaderWriterLockSlim();
        }
            
        public string ProvideHardwareId(bool useSimpleHardwareId)
        {
            // se esse método for utilizar async, terá que rever esse método de lock porque ReaderWriterLockSlim não funciona com async
            _lock.EnterReadLock();
            try
            {
                if (!string.IsNullOrEmpty(_hardwareId))
                    return _hardwareId;
            }
            finally
            {
                _lock.ExitReadLock();
            }
            
            _lock.EnterWriteLock();
            try
            {
                _hardwareId = BuildHardwareId(useSimpleHardwareId); 
            }
            finally
            {
                _lock.ExitWriteLock();
            }


            return _hardwareId;
        }

        public void Reset()
        {
            _lock.EnterWriteLock();
            try
            {
                _hardwareId = null;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        private static string BuildHardwareId(bool useSimpleHardwareId)
        {
            var hashAlgorithm = SHA256.Create();
            var deviceId = new DeviceIdBuilder()
                .OnWindows(windowsBuilder =>
                {
                    if (useSimpleHardwareId)
                    {
                        windowsBuilder
                            .AddMacAddressFromWmi(false, false)
                            .AddMachineGuid()
                            .AddSystemUuid();
                    }
                    else
                    { 
                        windowsBuilder
                            .AddProcessorId()
                            .AddMotherboardSerialNumber()
                            .AddSystemDriveSerialNumber()
                            .AddSystemUuid();
                    }
                })
                .UseFormatter(new HashDeviceIdFormatter(() => hashAlgorithm, new Base64ByteArrayEncoder()))
                .ToString();

            return deviceId;
        }
    }
}