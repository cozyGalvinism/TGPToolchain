using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using ReactiveUI;
using TGPToolchain.Helpers;
using TGPToolchain.Services;
using TGPToolchain.ViewModels;

namespace TGPToolchain.Backups
{
    public class DatabaseBackup
    {
        public static string GetBackupDirectory()
        {
            var appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (appPath == null) throw new ArgumentException("Unable to determine where app is located!");
            var backupFolder = Path.Join(appPath, "backups");
            return backupFolder;
        }
        
        public static void EnsureBackupFolder()
        {
            var backupFolder = GetBackupDirectory();
            if (Directory.Exists(backupFolder)) return;
            Directory.CreateDirectory(backupFolder);
        }
        
        public static void CreateBackup(LiteDBService databaseService, string backupName = "items")
        {
            EnsureBackupFolder();
            databaseService.Dispose();
            var backupFolder = GetBackupDirectory();
            var databasePath = ConfigurationManager.AppSettings["DatabasePath"];
            if (string.IsNullOrEmpty(databasePath)) return;
            var now = DateTime.Now;
            var backupPath = Path.Join(backupFolder, $"{backupName}-{now:dd-MM-yyyy-HH-mm-ss}.ldb.bak");
            File.Copy(databasePath, backupPath);
            databaseService.InitializeDatabase(databasePath);
        }

        public static void RestoreBackup(LiteDBService databaseService, string backupPath, bool createBackupBefore = false)
        {
            if(createBackupBefore) CreateBackup(databaseService, "before-restore");
            databaseService.Dispose();
            var databasePath = ConfigurationManager.AppSettings["DatabasePath"];
            if (string.IsNullOrEmpty(databasePath)) return;
            File.Copy(backupPath, databasePath, true);
            databaseService.InitializeDatabase(databasePath);
        }
    }
}