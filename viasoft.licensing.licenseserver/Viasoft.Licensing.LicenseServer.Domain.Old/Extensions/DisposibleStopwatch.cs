using System;
using System.Diagnostics;

namespace Viasoft.Licensing.LicenseServer.Domain.Old.Extensions
{
    public class DisposableStopWatch : Stopwatch, IDisposable
    {
        private readonly string _name;
        
        public DisposableStopWatch(string name)
        {
            _name = name;
            Start();
        }
        
        public void Dispose()
        {
            Stop();
            var ts = Elapsed;
            Console.WriteLine($"{_name}: {ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000000}.{ts.Ticks}");
        }
    }
}