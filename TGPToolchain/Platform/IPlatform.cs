namespace TGPToolchain.Platform
{
    public interface IPlatform
    {
        string GetPersistentData();
        string GetLogPath();
        bool IsGamePath(string path);
        string GetDataPath(string path);
        void OpenFolder(string path);
        bool SetClipboardText(string text);
    }
}