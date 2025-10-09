using System;
using System.Diagnostics;
using System.Windows.Threading;

namespace Application_Bac3_WPF.Models
{
    public class M_Chronometre
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly DispatcherTimer _timer;

        public event Action<TimeSpan>? TempsChange;

        public M_Chronometre()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += (s, e) => TempsChange?.Invoke(_stopwatch.Elapsed);
        }

        public void Start()
        {
            if (!_stopwatch.IsRunning)
            {
                _stopwatch.Start();
                _timer.Start();
            }
        }

        public void Stop()
        {
            if (_stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                _timer.Stop();
            }
        }

        public void Reset()
        {
            _stopwatch.Reset();
            TempsChange?.Invoke(TimeSpan.Zero);
        }
    }
}
