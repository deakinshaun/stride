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
using ReactiveUI;
using System.Threading.Tasks;
using Avalonia.Controls;
using Microsoft.CodeAnalysis.Differencing;
using Stride.Core.Assets.Editor.Settings;
using Stride.Core.Assets.Editor.ViewModel;
using Stride.Core.IO;
using Stride.Core.Presentation.Windows;
using Stride.GameStudio.View;
using System.Diagnostics;
using System.Linq;
using System;
using SharpDX.MediaFoundation;
using Stride.GameStudio.ViewModels;
using Stride.Core.MostRecentlyUsedFiles;

namespace Stride.GameStudio;

public partial class AvaloniaApp : Application
{
    private static RenderDocManager renderDocManager;
    public static GameStudioViewModel editor;

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

    private EventHandler doDialog;

    private async Task DoShowProjectSelectionDialogAsync(NewOrOpenSessionTemplateCollectionViewModel viewModel, Window mainWindowRoot, IViewModelServiceProvider serviceProvider, GameStudioViewModel editor)
    {
        bool? completed;

        mainWindowRoot.Activated -= doDialog;

        var startupWindow = new AProjectSelectionWindow(viewModel);

        startupWindow.Templates = viewModel;
        await startupWindow.ShowDialog(mainWindowRoot);

        // The user selected a template to instantiate
        if (startupWindow.NewSessionParameters != null)
        {
            // Clean existing entry in the MRU data
            var directory = startupWindow.NewSessionParameters.OutputDirectory;
            var name = startupWindow.NewSessionParameters.OutputName;
            var mruData = new MRUAdditionalDataCollection(InternalSettings.LoadProfileCopy, GameStudioInternalSettings.MostRecentlyUsedSessionsData, InternalSettings.WriteFile);
            mruData.RemoveFile(UFile.Combine(UDirectory.Combine(directory, name), new UFile(name + SessionViewModel.SolutionExtension)));

            completed = await editor.NewSession(startupWindow.NewSessionParameters);
        }
        // The user selected a path to open
        else if (startupWindow.ExistingSessionPath != null)
        {
            completed = await editor.OpenSession(startupWindow.ExistingSessionPath);
        }
        // The user cancelled from the new/open project window, so exit the application
        else
        {
            completed = true;
        }

        if (completed != true)
        {
            var windowsClosed = new List<Task>();
            foreach (var window in System.Windows.Application.Current.Windows.Cast<System.Windows.Window>().Where(x => x.IsLoaded))
            {
                var tcs = new TaskCompletionSource<int>();
                window.Unloaded += (s, e) => tcs.SetResult(0);
                windowsClosed.Add(tcs.Task);
            }

            await Task.WhenAll(windowsClosed);

            // When a project has been partially loaded, it might already have initialized some plugin that could conflict with
            // the next attempt to start something. Better start the application again.
            var commandLine = string.Join(" ", Environment.GetCommandLineArgs().Skip(1).Select(x => $"\"{x}\""));
            var process = new Process { StartInfo = new ProcessStartInfo(typeof(Program).Assembly.Location, commandLine) };
            process.Start();
 //           app.Shutdown();
            return;
        }

        if (editor.Session != null)
        {
            // If a session was correctly loaded, show the main window
            var mainWindow = new GameStudioWindow(editor);
            System.Windows.Application.Current.MainWindow = mainWindow;
            WindowManager.ShowMainWindow(mainWindow);
        }
        else
        {
            // Otherwise, exit.
//            app.Shutdown();
        }

    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var serviceProvider = InitializeServiceProvider();
            var viewModel = new NewOrOpenSessionTemplateCollectionViewModel(serviceProvider/*, startupWindow*/);
//            var startupWindow = new AProjectSelectionWindow(viewModel);

//            startupWindow.Templates = viewModel;

            var mainWindow = new AvaloniaMainWindow();

            //desktop.MainWindow = startupWindow;
            //startupWindow.ShowModal();
            doDialog = (s, e) => DoShowProjectSelectionDialogAsync(viewModel, mainWindow, serviceProvider, AvaloniaApp.editor);
            mainWindow.Activated += doDialog;
            //await startupWindow.ShowDialog(mainWindow);

            mainWindow.Show();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
