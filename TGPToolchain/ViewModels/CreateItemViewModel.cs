using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Metadata;
using ReactiveUI;
using TGPToolchain.Helpers;
using TGPToolchain.Models;
using TGPToolchain.Services;
#pragma warning disable 8618

namespace TGPToolchain.ViewModels
{
    public class CreateItemViewModel : ViewModelBase
    {
        private string? _code;
        private string? _name;
        private string? _prefab;
        private int _grist;
        private string? _strifekind;
        private string? _weaponsprite;
        private string? _icon;
        private string? _description;
        private int _speed;
        private bool _spawn;
        private ObservableCollection<StringValue> _aliases;
        private ObservableCollection<StringValue> _tags;
        private int _selectedAliasIndex;
        private int _selectedTagIndex;

        public CreateItemViewModel()
        {
            SelectedAliasIndex = -1;
            SelectedTagIndex = -1;
            Aliases = new ObservableCollection<StringValue>();
            Tags = new ObservableCollection<StringValue>();

            CreateItemCommand = ReactiveCommand.Create(() =>
            {
                if (Code == null || Name == null || Prefab == null || Icon == null) return null;
                var newItem = new LDBItem()
                {
                    Code = Code,
                    Name = Name,
                    Prefab = Prefab,
                    Grist = Grist,
                    Strifekind = Strifekind,
                    Weaponsprite = Weaponsprite,
                    Icon = Icon,
                    Description = Description,
                    Speed = Speed,
                    Spawn = Spawn,
                    Aliases = Aliases.Select(x => x.Value).ToList(),
                    Tags = Tags.Select(x => x.Value).ToList(),
                    Custom = true
                };
                return newItem;
            }, this.WhenAnyValue(
                p => p.Code, 
                p => p.Name, 
                p => p.Prefab, 
                p => p.Icon, 
                (code, name, prefab, icon) => !string.IsNullOrEmpty(code) &&
                                              code.Length == 8 &&
                                              !string.IsNullOrEmpty(name) && 
                                              !string.IsNullOrEmpty(prefab) && 
                                              !string.IsNullOrEmpty(icon)).DistinctUntilChanged());
            AddAliasCommand = ReactiveCommand.Create(() =>
            {
                Aliases.Add(new StringValue("change me"));
            });
            RemoveAliasCommand = ReactiveCommand.Create(() =>
            {
                Aliases.RemoveAt(SelectedAliasIndex);
            }, this.WhenAnyValue(p => p.SelectedAliasIndex, index =>
            {
                Trace.TraceWarning($"Index: {index}, None? {index != -1}");
                return index != -1;
            }));
            AddTagCommand = ReactiveCommand.Create(() =>
            {
                Tags.Add(new StringValue("change me"));
            });
            RemoveTagCommand = ReactiveCommand.Create(() =>
            {
                Tags.RemoveAt(SelectedTagIndex);
            }, this.WhenAnyValue(p => p.SelectedTagIndex, index => index != -1));
        }
        
        public ReactiveCommand<Unit, LDBItem?> CreateItemCommand { get; }
        public ReactiveCommand<Unit, Unit> AddAliasCommand { get; }
        public ReactiveCommand<Unit, Unit> RemoveAliasCommand { get; }
        public ReactiveCommand<Unit, Unit> AddTagCommand { get; }
        public ReactiveCommand<Unit, Unit> RemoveTagCommand { get; }

        public int SelectedAliasIndex
        {
            get => _selectedAliasIndex;
            set => this.RaiseAndSetIfChanged(ref _selectedAliasIndex, value);
        }

        public int SelectedTagIndex
        {
            get => _selectedTagIndex;
            set => this.RaiseAndSetIfChanged(ref _selectedTagIndex, value);
        }
        
        public string? Code
        {
            get => _code;
            set
            {
                this.RaiseAndSetIfChanged(ref _code, value);
            }
        }

        public string? Name
        {
            get => _name;
            set
            {
                this.RaiseAndSetIfChanged(ref _name, value);
            }
        }

        public string? Prefab
        {
            get => _prefab;
            set
            {
                this.RaiseAndSetIfChanged(ref _prefab, value);
            }
        }

        public int Grist
        {
            get => _grist;
            set => this.RaiseAndSetIfChanged(ref _grist, value);
        }

        public string? Strifekind
        {
            get => _strifekind;
            set => this.RaiseAndSetIfChanged(ref _strifekind, value);
        }

        public string? Weaponsprite
        {
            get => _weaponsprite;
            set => this.RaiseAndSetIfChanged(ref _weaponsprite, value);
        }

        public string? Icon
        {
            get => _icon;
            set
            {
                this.RaiseAndSetIfChanged(ref _icon, value);
            }
        }

        public string? Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }

        public int Speed
        {
            get => _speed;
            set => this.RaiseAndSetIfChanged(ref _speed, value);
        }

        public bool Spawn
        {
            get => _spawn;
            set => this.RaiseAndSetIfChanged(ref _spawn, value);
        }

        public ObservableCollection<StringValue> Aliases
        {
            get => _aliases;
            set => this.RaiseAndSetIfChanged(ref _aliases, value);
        }

        public ObservableCollection<StringValue> Tags
        {
            get => _tags;
            set => this.RaiseAndSetIfChanged(ref _tags, value);
        }
    }
}