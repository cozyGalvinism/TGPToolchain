using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;
using TGPToolchain.Backups;
using TGPToolchain.Models;
using TGPToolchain.Platform;
using TGPToolchain.Services;
using TGPToolchain.Utils;
// ReSharper disable NotAccessedField.Local

namespace TGPToolchain.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private IDataService _dataService;
        private LDBItem? _selectedItem;
        private LDBRecipe? _selectedRecipe;
        private ObservableCollection<LDBItem> _items;
        private ObservableCollection<LDBRecipe> _recipes;
        private string _itemSearchString;
        private string _recipeSearchString;
        private List<LDBItem> _addedItems;
        private List<LDBItem> _editedItems;
        private List<string> _deletedItems;
        private List<LDBRecipe> _addedRecipes;
        private List<LDBRecipe> _editedRecipes;
        private List<string> _deletedRecipes;
        private IPlatform _platform;

        #region Items

        public ICommand CreateItemCommand { get; }
        public ICommand EditItemCommand { get; }
        public ICommand DeleteItemCommand { get; }
        public Interaction<CreateItemViewModel, LDBItem?> CreateItemDialog { get; }
        public Interaction<EditItemViewModel, LDBItem?> EditItemDialog { get; }
        
        public Interaction<OpenFolderDialog, string?> BrowseGameDialog { get; }
        
        public Interaction<OpenFileDialog, string[]?> BrowseDatabaseDialog { get; }

        #endregion

        #region Recipes

        public ICommand CreateRecipeCommand { get; }
        public ICommand EditRecipeCommand { get; }
        public ICommand DeleteRecipeCommand { get; }
        public Interaction<CreateRecipeViewModel, LDBRecipe?> CreateRecipeDialog { get; }
        public Interaction<EditRecipeViewModel, LDBRecipe?> EditRecipeDialog { get; }

        #endregion

        #region Settings

        public ICommand BrowseGameCommand { get; }
        public ICommand OpenGameFolderCommand { get; }
        public ICommand OpenStreamingAssetsCommand { get; }
        public ICommand BrowseDatabaseFileCommand { get; }
        public ICommand CreateBackupCommand { get; }
        public ICommand RestoreBackupCommand { get; }
        public ICommand OpenBackupFolderCommand { get; }
        public ICommand OpenLogFolderCommand { get; }
        public ICommand UploadLogCommand { get; }
        public ICommand QuarantineFixCommand { get; }

        #endregion

        public LDBItem? SelectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }

        public LDBRecipe? SelectedRecipe
        {
            get => _selectedRecipe;
            set => this.RaiseAndSetIfChanged(ref _selectedRecipe, value);
        }

        public string ItemSearchString
        {
            get => _itemSearchString;
            set => this.RaiseAndSetIfChanged(ref _itemSearchString, value);
        }

        public string RecipeSearchString
        {
            get => _recipeSearchString;
            set => this.RaiseAndSetIfChanged(ref _recipeSearchString, value);
        }

        public ObservableCollection<LDBItem> Items
        {
            get => _items;
            set => this.RaiseAndSetIfChanged(ref _items, value);
        }

        public ObservableCollection<LDBRecipe> Recipes
        {
            get => _recipes;
            set => this.RaiseAndSetIfChanged(ref _recipes, value);
        }

        public string? GamePath
        {
            get => ConfigurationManager.AppSettings["TGPPath"];
            set
            {
                if (value == null) return;
                SetAppSettings("TGPPath", value);
                this.RaisePropertyChanged();
            }
        }

        public string? DatabasePath
        {
            get => ConfigurationManager.AppSettings["DatabasePath"];
            set
            {
                if (value == null) return;
                SetAppSettings("DatabasePath", value);
                this.RaisePropertyChanged();
            }
        }

        public bool AllowDatabaseCustom
        {
            get => bool.Parse(ConfigurationManager.AppSettings["CustomDatabasePath"] ?? "false");
            set
            {
                SetAppSettings("CustomDatabasePath", value.ToString());
                this.RaisePropertyChanged();
            }
        }

        private void RefreshItems()
        {
            Items = string.IsNullOrEmpty(ItemSearchString)
                ? new ObservableCollection<LDBItem>(_dataService.GetItems())
                : new ObservableCollection<LDBItem>(_dataService.FindItems(ItemSearchString));
            this.RaisePropertyChanged(nameof(Items));
        }

        private void RefreshRecipes()
        {
            Recipes = string.IsNullOrEmpty(RecipeSearchString)
                ? new ObservableCollection<LDBRecipe>(_dataService.GetRecipes())
                : new ObservableCollection<LDBRecipe>(_dataService.FindRecipes(RecipeSearchString));
            this.RaisePropertyChanged(nameof(Recipes));
        }

        public MainWindowViewModel()
        {
            Trace.Listeners.Add(new TextWriterTraceListener("tgptoolbox.log"));
            Trace.AutoFlush = true;

            _itemSearchString = "";
            _recipeSearchString = "";
            _addedItems = new List<LDBItem>();
            _editedItems = new List<LDBItem>();
            _deletedItems = new List<string>();
            _addedRecipes = new List<LDBRecipe>();
            _editedRecipes = new List<LDBRecipe>();
            _deletedRecipes = new List<string>();
            _platform = new DummyPlatform();

            SetPlatform();
            HasteBinClient hasteBin = new HasteBinClient("https://paste.galvinism.ink/");
            if (!string.IsNullOrEmpty(DatabasePath)) _dataService = new LiteDBService(DatabasePath);
            else _dataService = new DummyService();
            _items = new ObservableCollection<LDBItem>();
            _recipes = new ObservableCollection<LDBRecipe>();
            RefreshItems();
            RefreshRecipes();

            CreateItemDialog = new Interaction<CreateItemViewModel, LDBItem?>();
            EditItemDialog = new Interaction<EditItemViewModel, LDBItem?>();

            CreateRecipeDialog = new Interaction<CreateRecipeViewModel, LDBRecipe?>();
            EditRecipeDialog = new Interaction<EditRecipeViewModel, LDBRecipe?>();

            BrowseGameDialog = new Interaction<OpenFolderDialog, string?>();
            BrowseDatabaseDialog = new Interaction<OpenFileDialog, string[]?>();

            CreateItemCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var createItem = new CreateItemViewModel();
                var result = await CreateItemDialog.Handle(createItem);
                if (result == null) return;
                var dataServiceResult = _dataService.CreateItem(result);
                if (!dataServiceResult)
                    await MessageBox.MessageBox.ShowDialog(null, _dataService.LastError, "An error occurred",
                        MessageBox.MessageBox.MessageBoxButtons.Ok);
                RefreshItems();
            });
            EditItemCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (SelectedItem == null) return;
                var editItem = new EditItemViewModel(SelectedItem);
                var result = await EditItemDialog.Handle(editItem);
                if (result == null) return;
                var dataServiceResult = _dataService.EditItem(result);
                if (!dataServiceResult)
                    await MessageBox.MessageBox.ShowDialog(null, _dataService.LastError, "An error occurred",
                        MessageBox.MessageBox.MessageBoxButtons.Ok);
                RefreshItems();
            }, this.WhenAnyValue(p => p.SelectedItem, p => p.Items, (item, _) => item != null));
            DeleteItemCommand = ReactiveCommand.Create(async () =>
            {
                if (SelectedItem == null) return;
                var dataServiceResult = _dataService.DeleteItem(SelectedItem.Code);
                if (!dataServiceResult)
                    await MessageBox.MessageBox.ShowDialog(null, _dataService.LastError, "An error occurred",
                        MessageBox.MessageBox.MessageBoxButtons.Ok);
                SelectedItem = null;
                RefreshItems();
            }, this.WhenAnyValue(p => p.SelectedItem, p => p.Items, (item, _) => item != null));

            CreateRecipeCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var createRecipe = new CreateRecipeViewModel(_dataService.GetItems());
                var result = await CreateRecipeDialog.Handle(createRecipe);
                if (result == null) return;
                var dataServiceResult = _dataService.CreateRecipe(result);
                if (!dataServiceResult)
                    await MessageBox.MessageBox.ShowDialog(null, _dataService.LastError, "An error occurred",
                        MessageBox.MessageBox.MessageBoxButtons.Ok);
                RefreshRecipes();
            });
            EditRecipeCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (SelectedRecipe == null) return;
                var editRecipe = new EditRecipeViewModel(SelectedRecipe, _dataService.GetItems());
                var result = await EditRecipeDialog.Handle(editRecipe);
                if (result == null) return;
                var dataServiceResult = _dataService.EditRecipe(result);
                if (!dataServiceResult)
                    await MessageBox.MessageBox.ShowDialog(null, _dataService.LastError, "An error occurred",
                        MessageBox.MessageBox.MessageBoxButtons.Ok);
                RefreshRecipes();
            }, this.WhenAnyValue(p => p.SelectedRecipe, p => p.Recipes, (recipe, _) => recipe != null));
            DeleteRecipeCommand = ReactiveCommand.Create(async () =>
            {
                if (SelectedRecipe == null) return;
                var dataServiceResult = _dataService.DeleteRecipe(SelectedRecipe);
                if (!dataServiceResult)
                    await MessageBox.MessageBox.ShowDialog(null, _dataService.LastError, "An error occurred",
                        MessageBox.MessageBox.MessageBoxButtons.Ok);
                SelectedRecipe = null;
                RefreshRecipes();
            }, this.WhenAnyValue(p => p.SelectedRecipe, p => p.Recipes, (recipe, _) => recipe != null));

            BrowseGameCommand = ReactiveCommand.Create(async () =>
            {
                var ofd = new OpenFolderDialog {Title = "Where is the game located?"};
                var pickedPath = await BrowseGameDialog.Handle(ofd);
                if (string.IsNullOrEmpty(pickedPath)) return;
                if (!_platform.IsGamePath(pickedPath))
                {
                    await MessageBox.MessageBox.ShowDialog(null,
                        "The folder you selected was not a game folder! If you're on Mac, make sure you're selecting the folder that ends with .app.",
                        "Invalid path!",
                        MessageBox.MessageBox.MessageBoxButtons.Ok);
                    return;
                }

                GamePath = pickedPath;
                if (AllowDatabaseCustom) return;
                DatabasePath = Path.Join(_platform.GetDataPath(pickedPath), "StreamingAssets", "items.ldb");
                if (_dataService is not LiteDBService) _dataService = new LiteDBService(pickedPath);
                var liteDbService = _dataService as LiteDBService;
                liteDbService?.Dispose();
                liteDbService?.InitializeDatabase(DatabasePath);
                RefreshItems();
                RefreshRecipes();
            });

            BrowseDatabaseFileCommand = ReactiveCommand.Create(async () =>
            {
                var ofd = new OpenFileDialog {Title = "Where is the database file located?", AllowMultiple = false};
                ofd.Filters.Add(new FileDialogFilter() { Extensions = new List<string>() { "ldb" }, Name = "Database files" });
                var paths = await BrowseDatabaseDialog.Handle(ofd);
                if (paths is not {Length: > 0}) return;
                string pickedPath = paths[0];
                if (string.IsNullOrEmpty(pickedPath)) return;
                if (!pickedPath.EndsWith(".ldb"))
                {
                    await MessageBox.MessageBox.ShowDialog(null,
                        "The folder you selected was not a database file!",
                        "Invalid path!",
                        MessageBox.MessageBox.MessageBoxButtons.Ok);
                    return;
                }

                if (_dataService is not LiteDBService) _dataService = new LiteDBService(pickedPath);
                var liteDbService = _dataService as LiteDBService;
                DatabasePath = pickedPath;
                liteDbService?.Dispose();
                liteDbService?.InitializeDatabase(DatabasePath);
                RefreshItems();
                RefreshRecipes();
            }, this.WhenAnyValue(p => p.AllowDatabaseCustom, p => p.DatabasePath, (allowCustom, _) => allowCustom));

            OpenGameFolderCommand = ReactiveCommand.Create(() =>
            {
                if (string.IsNullOrEmpty(GamePath)) return;
                _platform.OpenFolder(GamePath);
            }, this.WhenAnyValue(p => p.GamePath, (gamePath) => !string.IsNullOrEmpty(gamePath)));

            OpenStreamingAssetsCommand = ReactiveCommand.Create(() =>
            {
                if (string.IsNullOrEmpty(GamePath)) return;
                _platform.OpenFolder(Path.Join(_platform.GetDataPath(GamePath), "StreamingAssets"));
            }, this.WhenAnyValue(p => p.GamePath, (gamePath) => !string.IsNullOrEmpty(gamePath)));

            OpenBackupFolderCommand = ReactiveCommand.Create(() =>
            {
                if (string.IsNullOrEmpty(DatabasePath)) return;
                DatabaseBackup.EnsureBackupFolder();
                _platform.OpenFolder(DatabaseBackup.GetBackupDirectory());
            }, this.WhenAnyValue(p => p.DatabasePath, (databasePath) => !string.IsNullOrEmpty(databasePath)));

            OpenLogFolderCommand = ReactiveCommand.Create(() =>
            {
                _platform.OpenFolder(_platform.GetPersistentData());
            });

            UploadLogCommand = ReactiveCommand.Create(async () =>
            {
                if (!File.Exists(_platform.GetLogPath())) return;
                var result = await hasteBin.Post(File.ReadAllText(_platform.GetLogPath()));
                if (result.IsSuccess)
                {
                    if (result.FullUrl != null)
                    {
                        _platform.SetClipboardText(result.FullUrl);
                        await MessageBox.MessageBox.ShowDialog(null,
                            $"The log file has been uploaded to {result.FullUrl} and has been copied to your clipboard! This log may contain information like your username, so don't share this with regular users and DM it to a developer when they ask for it!",
                            "Upload successful", MessageBox.MessageBox.MessageBoxButtons.Ok);
                    }
                }
                else
                {
                    Trace.TraceError(result.Error);
                    await MessageBox.MessageBox.ShowDialog(null, "That didn't work... Please open the log file folder and upload the file ", "Upload failed", MessageBox.MessageBox.MessageBoxButtons.Ok);
                }
            });

            QuarantineFixCommand = ReactiveCommand.Create(() =>
            {
                if (_platform is not Mac mac) return;
                if (string.IsNullOrEmpty(GamePath)) return;
                mac.FixQuarantine(GamePath);
            }, this.WhenAnyValue(p => p.GamePath, p => p.DatabasePath, (gamePath, _) => _platform is Mac && !string.IsNullOrEmpty(gamePath)));
            
            CreateBackupCommand = ReactiveCommand.Create(() =>
            {
                if (string.IsNullOrEmpty(DatabasePath)) return;
                if (_dataService is not LiteDBService) _dataService = new LiteDBService(DatabasePath);
                var liteDbService = _dataService as LiteDBService;
                DatabaseBackup.CreateBackup(liteDbService!);
                RefreshItems();
                RefreshRecipes();
            }, this.WhenAnyValue(p => p.DatabasePath, (databasePath) => !string.IsNullOrEmpty(databasePath)));

            RestoreBackupCommand = ReactiveCommand.Create(async () =>
            {
                var ofd = new OpenFileDialog
                {
                    Title = "Where is the backup file located?",
                    AllowMultiple = false,
                    Directory = DatabaseBackup.GetBackupDirectory()
                };
                ofd.Filters.Add(new FileDialogFilter() { Extensions = new List<string>() { "ldb.bak" }, Name = "Database backup files" });
                var paths = await BrowseDatabaseDialog.Handle(ofd);
                if (paths == null) return;
                string pickedPath = paths[0];
                if (string.IsNullOrEmpty(pickedPath)) return;
                if (!pickedPath.EndsWith(".ldb.bak"))
                {
                    await MessageBox.MessageBox.ShowDialog(null,
                        "The folder you selected was not a database file!",
                        "Invalid path!",
                        MessageBox.MessageBox.MessageBoxButtons.Ok);
                    return;
                }

                if (_dataService is not LiteDBService) _dataService = new LiteDBService(pickedPath);
                var liteDbService = _dataService as LiteDBService;
                DatabaseBackup.RestoreBackup(liteDbService!, pickedPath);
                RefreshItems();
                RefreshRecipes();
            }, this.WhenAnyValue(p => p.DatabasePath, (databasePath) => !string.IsNullOrEmpty(databasePath)));
            
            this.WhenAnyValue(p => p.ItemSearchString).Subscribe(searchString =>
            {
                if (string.IsNullOrEmpty(searchString))
                {
                    Items = new ObservableCollection<LDBItem>(_dataService.GetItems());
                    return;
                }

                Items = new ObservableCollection<LDBItem>(_dataService.FindItems(searchString));
            });

            this.WhenAnyValue(p => p.RecipeSearchString).Subscribe(searchString =>
            {
                if (string.IsNullOrEmpty(searchString))
                {
                    Recipes = new ObservableCollection<LDBRecipe>(_dataService.GetRecipes());
                    return;
                }

                Recipes = new ObservableCollection<LDBRecipe>(_dataService.FindRecipes(searchString));
            });
        }

        private void SetPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) _platform = new Windows();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) _platform = new Linux();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) _platform = new Mac();
            else
            {
                MessageBox.MessageBox.ShowDialog(null, "Sorry, but your operating system is not supported!",
                    "Unsupported operating system", MessageBox.MessageBox.MessageBoxButtons.Ok);
                Environment.Exit(1);
            }
        }

        public void OnItemsCellEdit(object? sender, DataGridCellPointerPressedEventArgs e)
        {
            if (e.Column == null || e.Row == null ||
                !e.PointerPressedEventArgs.GetCurrentPoint(null).Properties.IsRightButtonPressed) return;
            if (sender is not DataGrid dataGrid) return;
            if (dataGrid.SelectedIndex != e.Row.GetIndex())
            {
                dataGrid.SelectedIndex = e.Row.GetIndex();
            }
        }

        public void OnRecipesCellEdit(object? sender, DataGridCellPointerPressedEventArgs e)
        {
            if (e.Column == null || e.Row == null ||
                !e.PointerPressedEventArgs.GetCurrentPoint(null).Properties.IsRightButtonPressed) return;
            if (sender is not DataGrid dataGrid) return;
            if (dataGrid.SelectedIndex != e.Row.GetIndex())
            {
                dataGrid.SelectedIndex = e.Row.GetIndex();
            }
        }

        public void Dispose()
        {
            if (_dataService is LiteDBService service)
            {
                service.Dispose();
            }
        }

        private void SetAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (configFile == null) throw new ConfigurationErrorsException("Config file could not be found");
                var settings = configFile.AppSettings.Settings;
                if (settings == null) throw new ConfigurationErrorsException("Settings are invalid");
                if (settings[key] == null) settings.Add(key, value);
                else settings[key].Value = value;
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Trace.TraceError("Error while writing app settings");
            }
        }
    }
}