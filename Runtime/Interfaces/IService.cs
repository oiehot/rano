namespace Rano
{
    public enum EServiceState
    {
        None,
        Initalized,
        Running,
        Paused,
        Stopped
    }

    public interface IService
    {
        EServiceState state { get; }
        void Init();
        void Run();
        void Pause();
        void Resume();
        void Stop();
    }
}