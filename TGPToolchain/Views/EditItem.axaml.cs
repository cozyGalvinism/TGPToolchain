using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using TGPToolchain.ViewModels;

namespace TGPToolchain.Views
{
    public class EditItem : ReactiveWindow<EditItemViewModel>
    {
        public EditItem()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            // This line breaks the UI editor...
            this.WhenActivated(d => d(ViewModel.EditItemCommand.Subscribe(Close)));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}