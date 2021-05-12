using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace TGPToolchain.Platform
{
    public class Windows : IPlatform
    {
        public string GetPersistentData()
        {
            return Environment.ExpandEnvironmentVariables(
                "%userprofile%\\AppData\\LocalLow\\The Genesis Team\\The Genesis Project");
        }

        public string GetLogPath()
        {
            var path = Path.Join(GetPersistentData(), "Player.log");
            return path;
        }

        public bool IsGamePath(string path)
        {
            return File.Exists(Path.Join(path, "The Genesis Project.exe"));
        }

        public string GetDataPath(string path)
        {
            return Path.Join(path, "The Genesis Project_Data");
        }

        public void OpenFolder(string path)
        {
            Process.Start("explorer.exe", $"\"{path}\"");
        }

        public bool SetClipboardText(string text)
        {
            OpenClipboard();

            EmptyClipboard();
            IntPtr hGlobal = default;
            try
            {
                var bytes = (text.Length + 1) * 2;
                hGlobal = Marshal.AllocHGlobal(bytes);

                if (hGlobal == default)
                {
                    if (hGlobal != default)
                    {
                        Marshal.FreeHGlobal(hGlobal);
                    }

                    CloseClipboard();
                    return false;
                }

                var target = GlobalLock(hGlobal);

                if (target == default)
                {
                    if (hGlobal != default)
                    {
                        Marshal.FreeHGlobal(hGlobal);
                    }

                    CloseClipboard();
                    return false;
                }

                try
                {
                    Marshal.Copy(text.ToCharArray(), 0, target, text.Length);
                }
                finally
                {
                    GlobalUnlock(target);
                }

                if (SetClipboardData(cfUnicodeText, hGlobal) == default)
                {
                    if (hGlobal != default)
                    {
                        Marshal.FreeHGlobal(hGlobal);
                    }

                    CloseClipboard();
                    return false;
                }

                hGlobal = default;
            }
            finally
            {
                if (hGlobal != default)
                {
                    Marshal.FreeHGlobal(hGlobal);
                }

                CloseClipboard();
            }

            return true;
        }

        public static void OpenClipboard()
        {
            var num = 10;
            while (true)
            {
                if (OpenClipboard(default))
                {
                    break;
                }

                if (--num == 0)
                {
                    ThrowWin32();
                }

                Thread.Sleep(100);
            }
        }

        const uint cfUnicodeText = 13;

        static void ThrowWin32()
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetClipboardData(uint uFormat, IntPtr data);

        [DllImport("user32.dll")]
        static extern bool EmptyClipboard();
    }
}