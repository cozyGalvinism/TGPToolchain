using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;
using TGPToolchain.Models;

namespace TGPToolchain.ViewModels
{
    public class CreateRecipeViewModel : ViewModelBase
    {
        private string? _itemA;
        private string? _itemB;
        private LDBRecipe.Methods _method;
        private string? _result;
        private ObservableCollection<LDBItem> _itemBrowseList;

        public IEnumerable<LDBRecipe.Methods> AvailableMethods =>
            Enum.GetValues(typeof(LDBRecipe.Methods)).Cast<LDBRecipe.Methods>();
        
        public string? ItemA
        {
            get => _itemA;
            set => this.RaiseAndSetIfChanged(ref _itemA, value);
        }

        public string? ItemB
        {
            get => _itemB;
            set => this.RaiseAndSetIfChanged(ref _itemB, value);
        }

        public LDBRecipe.Methods Method
        {
            get => _method;
            set => this.RaiseAndSetIfChanged(ref _method, value);
        }

        public string? Result
        {
            get => _result;
            set
            {
                this.RaiseAndSetIfChanged(ref _result, value);
            }
        }

        public ObservableCollection<LDBItem> ItemBrowseList
        {
            get => _itemBrowseList;
            set => this.RaiseAndSetIfChanged(ref _itemBrowseList, value);
        }

        public AutoCompleteFilterPredicate<object> FilterPredicate => (search, obj) =>
        {
            if (obj is not LDBItem item) return false;
            return item.Code.Contains(search) ||
                   item.Name.Contains(search) ||
                   item.Prefab.Contains(search) ||
                   !string.IsNullOrEmpty(item.Icon) && item.Icon.Contains(search) ||
                   !string.IsNullOrEmpty(item.Weaponsprite) && item.Weaponsprite.Contains(search) ||
                   !string.IsNullOrEmpty(item.Strifekind) && item.Strifekind.Contains(search) ||
                   !string.IsNullOrEmpty(item.Description) && item.Description.Contains(search) ||
                   item.Tags != null && item.Tags.Contains(search) ||
                   item.Aliases != null && item.Aliases.Contains(search);
        };

        public ReactiveCommand<Unit, LDBRecipe?> CreateRecipeCommand { get; }

        public CreateRecipeViewModel() : this(null) {}
        public CreateRecipeViewModel(IEnumerable<LDBItem>? items = null)
        {
            
            _itemBrowseList = items == null ? new ObservableCollection<LDBItem>() : new ObservableCollection<LDBItem>(items);
            CreateRecipeCommand = ReactiveCommand.Create(() =>
                {
                    Trace.TraceWarning($"Result is {Result}");
                    if (ItemA == null || ItemB == null || Result == null) return null;
                    var resultItem = _itemBrowseList.FirstOrDefault(i => i.Code == Result);
                    if (resultItem == null) return null;
                    Trace.TraceWarning("Creating recipe");
                    var newRecipe = new LDBRecipe()
                    {
                        ItemA = ItemA,
                        ItemB = ItemB,
                        Method = Method,
                        Result = resultItem
                    };
                    return newRecipe;
                },
                this.WhenAnyValue(p => p.ItemA, p => p.ItemB, p => p.Result,
                    (itemA, itemB, result) =>
                        !string.IsNullOrEmpty(itemA) && !string.IsNullOrEmpty(itemB) && !string.IsNullOrEmpty(result)));
        }
    }
}