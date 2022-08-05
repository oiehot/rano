namespace Rano.SaveSystem
{
    public interface ISaveLoadable
    {
        void ClearState();
        void DefaultState();
        object CaptureState();
        void ValidateState(object state);
        void RestoreState(object state);
    }
}