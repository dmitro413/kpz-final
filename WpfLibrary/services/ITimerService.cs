namespace WpfLibrary.services
{
    public interface ITimerService
    {
        int ElapsedSeconds { get; }
        bool IsRunning { get; }
        event Action<int> OnTick;

        void Start();
        void Stop();
        void Reset();
    }
}