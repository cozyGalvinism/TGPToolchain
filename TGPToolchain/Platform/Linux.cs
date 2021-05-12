using System;
using System.Diagnostics;
using System.IO;
using TGPToolchain.Utils;

namespace TGPToolchain.Platform
{
    public class Linux : IPlatform
    {
        public string GetPersistentData()
        {
            return Environment.ExpandEnvironmentVariables("%HOME%/.config/unity3d/The Genesis Team/The Genesis Project");
        }

        public string GetLogPath()
        {
            return Path.Join(GetPersistentData(), "Player.log");
        }

        public bool IsGamePath(string path)
        {
            return File.Exists(Path.Join(path, "The Genesis Project"));
        }

        public string GetDataPath(string path)
        {
            return Path.Join(path, "The Genesis Project_Data");
        }

        public void OpenFolder(string path)
        {
            Process.Start("xdg-open", $"\"{path}\"");
        }

        public bool SetClipboardText(string text)
        {
            var tempFileName = Path.GetTempFileName();
            File.WriteAllText(tempFileName, text);
            try
            {
                BashRunner.Run($"cat {tempFileName} | xclip");
            }
            finally
            {
                File.Delete(tempFileName);
            }

            return true;
        }
    }
}