using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Stride.Core.Assets.Editor.Components.TemplateDescriptions.Views;
using Stride.Core.Assets.Editor.Services;
using Stride.Core.Presentation.View;
using Stride.Core.Presentation.ViewModel;
using Stride.GameStudio.Helpers;
using Stride.GameStudio.Services;
using Stride.Graphics;

using Stride.Core.Assets.Editor.Components.TemplateDescriptions.ViewModels;

namespace Stride.GameStudio;

public partial class AvaloniaApp : Application
{
    private static RenderDocManager renderDocManager;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private static IViewModelServiceProvider InitializeServiceProvider()
    {
        // TODO: this should be done elsewhere
        var dispatcherService = new ADispatcherService(Dispatcher.UIThread);
        var dialogService = new StrideDialogService(dispatcherService, StrideGameStudio.EditorName);
        var pluginService = new PluginService();
        var services = new List<object> { new ADispatcherService(Dispatcher.UIThread), dialogService, pluginService };
        if (renderDocManager != null)
            services.Add(renderDocManager);
        var serviceProvider = new ViewModelServiceProvider(services);
        return serviceProvider;
    }
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var serviceProvider = InitializeServiceProvider();
            var startupWindow = new AProjectSelectionWindow();

            var viewModel = new NewOrOpenSessionTemplateCollectionViewModel(serviceProvider, startupWindow);
            startupWindow.Templates = viewModel;

            //desktop.MainWindow = startupWindow;
            startupWindow.ShowModal();
        }

        base.OnFrameworkInitializationCompleted();

    }
}
