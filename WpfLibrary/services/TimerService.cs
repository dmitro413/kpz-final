using System.Windows.Threading;

namespace WpfLibrary.services
{
    public class TimerService : ITimerService
    {
        private readonly DispatcherTimer _timer;
        private int _elapsedSeconds;

        public int ElapsedSeconds => _elapsedSeconds;
        public bool IsRunning { get; private set; }

        public event Action<int> OnTick;

        public TimerService()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (_, __) => HandleTick();
        }

        public void Start()
        {
            _elapsedSeconds = 0;
            IsRunning = true;
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
            IsRunning = false;
        }

        public void Reset()
        {
            Stop();
            _elapsedSeconds = 0;
        }

        private void HandleTick()
        {
            _elapsedSeconds++;
            OnTick?.Invoke(_elapsedSeconds);
        }
    }
}
