using System.Configuration;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using TGPToolchain.Services;
using TGPToolchain.ViewModels;
using TGPToolchain.Views;

namespace TGPToolchain
{
    public class App : Application
    {
        private IClassicDesktopStyleApplicationLifetime _desktop = null!;
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                _desktop = desktop;
                desktop.Exit += DesktopOnExit;

                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void DesktopOnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            MainWindowViewModel vm = (MainWindowViewModel)_desktop.MainWindow.DataContext!;
            vm.Dispose();
        }
    }
}