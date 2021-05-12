using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace TGPToolchain.Platform
{
    public class Mac : IPlatform
    {
        public string GetPersistentData()
        {
            return Environment.ExpandEnvironmentVariables("%HOME%/Library/Logs/The Genesis Team/The Genesis Project");
        }

        public string GetLogPath()
        {
            return Path.Join(GetPersistentData(), "Player.log");
        }

        public bool IsGamePath(string path)
        {
            return new DirectoryInfo(path).Name.EndsWith(".app");
        }

        public string GetDataPath(string path)
        {
            return Path.Join(path, "Contents", "Resources", "Data");
        }

        public void OpenFolder(string path)
        {
            Process.Start("open", $"\"{path}\"");
        }

        public void FixQuarantine(string path)
        {
            ProcessStartInfo info = new("/usr/bin/xattr", $"-d com.apple.quarantine \"{path}\"");
            Process.Start(info);
        }
        
        public bool SetClipboardText(string text)
        {
            var nsString = objc_getClass("NSString");
            IntPtr str = default;
            IntPtr dataType = default;
            try
            {
                str = objc_msgSend(objc_msgSend(nsString, sel_registerName("alloc")), sel_registerName("initWithUTF8String:"), text);
                dataType = objc_msgSend(objc_msgSend(nsString, sel_registerName("alloc")), sel_registerName("initWithUTF8String:"), NSPasteboardTypeString);

                var nsPasteboard = objc_getClass("NSPasteboard");
                var generalPasteboard = objc_msgSend(nsPasteboard, sel_registerName("generalPasteboard"));

                objc_msgSend(generalPasteboard, sel_registerName("clearContents"));
                objc_msgSend(generalPasteboard, sel_registerName("setString:forType:"), str, dataType);
            }
            finally
            {
                if (str != default)
                {
                    objc_msgSend(str, sel_registerName("release"));
                }

                if (dataType != default)
                {
                    objc_msgSend(dataType, sel_registerName("release"));
                }
            }

            return true;
        }

        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
        static extern IntPtr objc_getClass(string className);
        
        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
        static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
        static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, string arg1);

        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
        static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
        static extern IntPtr sel_registerName(string selectorName);

        const string NSPasteboardTypeString = "public.utf8-plain-text";
    }
}