using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using TGPToolchain.Models;
using TGPToolchain.ViewModels;

namespace TGPToolchain.Views
{
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.WhenActivated(d => d(ViewModel.CreateItemDialog.RegisterHandler(DoCreateDialogAsync)));
            this.WhenActivated(d => d(ViewModel.EditItemDialog.RegisterHandler(DoEditDialogAsync)));
            this.WhenActivated(d => d(ViewModel.CreateRecipeDialog.RegisterHandler(DoCreateRecipeDialogAsync)));
            this.WhenActivated(d => d(ViewModel.EditRecipeDialog.RegisterHandler(DoEditRecipeDialogAsync)));
            this.WhenActivated(d => d(ViewModel.BrowseGameDialog.RegisterHandler(DoBrowseGameDialog)));
            this.WhenActivated(d => d(ViewModel.BrowseDatabaseDialog.RegisterHandler(DoBrowseDatabaseFileDialog)));
        }

        private async Task DoBrowseGameDialog(InteractionContext<OpenFolderDialog, string?> arg)
        {
            var result = await arg.Input.ShowAsync(this);
            arg.SetOutput(result);
        }
        
        private async Task DoBrowseDatabaseFileDialog(InteractionContext<OpenFileDialog, string[]?> arg)
        {
            var result = await arg.Input.ShowAsync(this);
            arg.SetOutput(result);
        }

        private async Task DoEditRecipeDialogAsync(InteractionContext<EditRecipeViewModel, LDBRecipe?> arg)
        {
            var dialog = new EditRecipe() {DataContext = arg.Input};
            var result = await dialog.ShowDialog<LDBRecipe?>(this);
            arg.SetOutput(result);
        }

        private async Task DoCreateRecipeDialogAsync(InteractionContext<CreateRecipeViewModel, LDBRecipe?> arg)
        {
            var dialog = new CreateRecipe() {DataContext = arg.Input};
            var result = await dialog.ShowDialog<LDBRecipe?>(this);
            arg.SetOutput(result);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void DataGrid_OnCellPointerPressed(object? sender, DataGridCellPointerPressedEventArgs e)
        {
            MainWindowViewModel vm = (MainWindowViewModel) this.DataContext!;
            vm.OnItemsCellEdit(sender, e);
        }

        private void DataGrid_OnCellPointerPressedRecipes(object? sender, DataGridCellPointerPressedEventArgs e)
        {
            ViewModel.OnRecipesCellEdit(sender, e);
        }

        private async Task DoEditDialogAsync(InteractionContext<EditItemViewModel, LDBItem?> interaction)
        {
            var dialog = new EditItem {DataContext = interaction.Input};
            var result = await dialog.ShowDialog<LDBItem?>(this);
            interaction.SetOutput(result);
        }
        
        private async Task DoCreateDialogAsync(InteractionContext<CreateItemViewModel, LDBItem?> interaction)
        {
            var dialog = new CreateItem {DataContext = interaction.Input};
            
            var result = await dialog.ShowDialog<LDBItem?>(this);
            interaction.SetOutput(result);
        }
    }
}