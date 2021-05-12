namespace TGPToolchain.Platform
{
    public class DummyPlatform : IPlatform
    {
        public string GetPersistentData()
        {
            throw new System.NotImplementedException();
        }

        public string GetLogPath()
        {
            throw new System.NotImplementedException();
        }

        public bool IsGamePath(string path)
        {
            throw new System.NotImplementedException();
        }

        public string GetDataPath(string path)
        {
            throw new System.NotImplementedException();
        }

        public void OpenFolder(string path)
        {
            throw new System.NotImplementedException();
        }

        public bool SetClipboardText(string text)
        {
            throw new System.NotImplementedException();
        }
    }
}