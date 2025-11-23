
public interface ISavable
{
    string SaveKey { get; }

    string CaptureState();

    void RestoreState(string json);
}