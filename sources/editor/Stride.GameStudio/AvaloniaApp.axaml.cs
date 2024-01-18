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
using Stride.Assets.Presentation;
using Stride.Core.Assets;
using Stride.GameStudio.Plugin;
using Stride.Core.Translation;
using Stride.Core.Translation.Providers;
using Stride.Core.Assets.Editor;
using System.Globalization;
using Stride.Core.Diagnostics;
using Stride.Editor.Build;
using Stride.Editor.Engine;
using Stride.Editor.Preview;
using Stride.Metrics;
using Stride.PrivacyPolicy;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Stride.GameStudio;

public partial class AvaloniaApp : Application
{
    private static RenderDocManager renderDocManager;
    //public static GameStudioViewModel editor;
    //private static Dispatcher mainDispatcher;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private static IViewModelServiceProvider InitializeServiceProvider()
    {
        // TODO: this should be done elsewhere
        var dispatcherService = new ADispatcherService(Dispatcher.UIThread);
        var dialogService = new AStrideDialogService(dispatcherService, StrideGameStudio.EditorName);
        var pluginService = new APluginService();
        var services = new List<object> { new ADispatcherService(Dispatcher.UIThread), dialogService, pluginService };
        if (renderDocManager != null)
            services.Add(renderDocManager);
        var serviceProvider = new ViewModelServiceProvider(services);
        return serviceProvider;
    }

    private static void InitializeLanguageSettings()
    {
        TranslationManager.Instance.RegisterProvider(new GettextTranslationProvider());
        TranslationManager.Instance.CurrentLanguage = EditorSettings.Language.GetValue() switch
        {
            SupportedLanguage.MachineDefault => CultureInfo.InstalledUICulture,
            SupportedLanguage.English => new CultureInfo("en-US"),
            SupportedLanguage.French => new CultureInfo("fr-FR"),
            SupportedLanguage.Japanese => new CultureInfo("ja-JP"),
            SupportedLanguage.Russian => new CultureInfo("ru-RU"),
            SupportedLanguage.German => new CultureInfo("de-DE"),
            SupportedLanguage.Spanish => new CultureInfo("es-ES"),
            SupportedLanguage.ChineseSimplified => new CultureInfo("zh-Hans"),
            SupportedLanguage.Italian => new CultureInfo("it-IT"),
            SupportedLanguage.Korean => new CultureInfo("ko-KR"),
            _ => throw new ArgumentException("Invalid language option"),
        };
    }

    private EventHandler doDialog;

    private async Task DoShowProjectSelectionDialogAsync(NewOrOpenSessionTemplateCollectionViewModel viewModel, Window mainWindowRoot, IViewModelServiceProvider serviceProvider, GameStudioViewModel editor)
    {
        bool? completed;
        mainWindowRoot.Activated -= doDialog;

        try
        {
            // temporary while testing gamestudiowindow.
        await editor.OpenSession(new Stride.Core.IO.UFile("C:/Users/sbanga/Documents/Stride Projects/VRSandbox/VRSandbox.sln"));
        var mainWindowX = new AGameStudioWindow(editor);
        mainWindowX.Show();
        return;


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
            var mainWindow = new AGameStudioWindow(editor);
            //System.Windows.Application.Current.MainWindow = mainWindow;
            //WindowManager.ShowMainWindow(mainWindow);
            mainWindow.Show();
        }
        else
        {
            // Otherwise, exit.
//            app.Shutdown();
        }
        }
        catch (Exception ex)
        {
            var a = 1;
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
//            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            EditorPath.EditorTitle = StrideGameStudio.EditorName;

            if (IntPtr.Size == 4)
            {
                //MessageBox.Show("Stride GameStudio requires a 64bit OS to run.", "Stride", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }

            //PrivacyPolicyHelper.RestartApplication = RestartApplication;
            //PrivacyPolicyHelper.EnsurePrivacyPolicyStride40();

            // We use MRU of the current version only when we're trying to reload last session.
            var mru = new MostRecentlyUsedFileCollection(InternalSettings.LoadProfileCopy, InternalSettings.MostRecentlyUsedSessions, InternalSettings.WriteFile);
            mru.LoadFromSettings();

            EditorSettings.Initialize();
            Thread.CurrentThread.Name = "Main Avalonia thread";

            var startupSessionPath = StrideEditorSettings.StartupSession.GetValue();
            var lastSessionPath = EditorSettings.ReloadLastSession.GetValue() ? mru.MostRecentlyUsedFiles.FirstOrDefault() : null;
            var initialSessionPath = !UPath.IsNullOrEmpty(startupSessionPath) ? startupSessionPath : lastSessionPath?.FilePath;
            // Install Metrics for the editor
            using (StrideGameStudio.MetricsClient = new MetricsClient(CommonApps.StrideEditorAppId))
            {
                try
                {
                    // Environment.GetCommandLineArgs correctly process arguments regarding the presence of '\' and '"'
                    var args = Environment.GetCommandLineArgs().Skip(1).ToList();

                    // Handle arguments
                    for (var i = 0; i < args.Count; i++)
                    {
                        if (args[i] == "/LauncherWindowHandle")
                        {
 //                           windowHandle = new IntPtr(long.Parse(args[++i]));
                        }
                        else if (args[i] == "/NewProject")
                        {
                            initialSessionPath = null;
                        }
                        else if (args[i] == "/DebugEditorGraphics")
                        {
                            EmbeddedGame.DebugMode = true;
                        }
                        else if (args[i] == "/RenderDoc")
                        {
                            // TODO: RenderDoc is not working here (when not in debug)
                            GameStudioPreviewService.DisablePreview = true;
                            renderDocManager = new RenderDocManager();
                            renderDocManager.Initialize();
                        }
                        else if (args[i] == "/RecordEffects")
                        {
                            GameStudioBuilderService.GlobalEffectLogPath = args[++i];
                        }
                        else
                        {
                            initialSessionPath = args[i];
                        }
                    }
                    RuntimeHelpers.RunModuleConstructor(typeof(Asset).Module.ModuleHandle);

                    //listen to logger for crash report
  //                  GlobalLogger.GlobalMessageLogged += GlobalLoggerOnGlobalMessageLogged;

  /*                  mainDispatcher = Dispatcher.CurrentDispatcher;
                    mainDispatcher.InvokeAsync(() => Startup(initialSessionPath));

                    using (new WindowManager(mainDispatcher))
                    {
                        app = new App { ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown };
                        app.Activated += (sender, eventArgs) =>
                        {
                            StrideGameStudio.MetricsClient?.SetActiveState(true);
                        };
                        app.Deactivated += (sender, eventArgs) =>
                        {
                            StrideGameStudio.MetricsClient?.SetActiveState(false);
                        };

                        app.InitializeComponent();
                        app.Run();
                    }

                    renderDocManager?.RemoveHooks();*/
                }
                catch (Exception e)
                {
 //                   HandleException(e, 0);
                }
            }

            InitializeLanguageSettings();
            var serviceProvider = InitializeServiceProvider();

            try
            {
                PackageSessionPublicHelper.FindAndSetMSBuildVersion();
            }
            catch (Exception e)
            {
                var message = "Could not find a compatible version of MSBuild.\r\n\r\n" +
                              "Check that you have a valid installation with the required workloads, or go to [www.visualstudio.com/downloads](https://www.visualstudio.com/downloads) to install a new one.\r\n" +
                              "Also make sure you have the latest [.NET 6 SDK](https://dotnet.microsoft.com/) \r\n\r\n" +
                              e;
                //await serviceProvider.Get<IEditorDialogService>().MessageBox(message, Core.Presentation.Services.MessageBoxButton.OK, Core.Presentation.Services.MessageBoxImage.Error);
                //app.Shutdown();
                return;
            }

            // We use a MRU that contains the older version projects to display in the editor
            //var mru = new MostRecentlyUsedFileCollection(InternalSettings.LoadProfileCopy, InternalSettings.MostRecentlyUsedSessions, InternalSettings.WriteFile);
            //mru.LoadFromSettings();
            var editor = new GameStudioViewModel(serviceProvider, mru);
//            AssetsPlugin.RegisterPlugin(typeof(StrideDefaultAssetsPlugin));
            AssetsPlugin.RegisterPlugin(typeof(AStrideDefaultAssetsPlugin));
            AssetsPlugin.RegisterPlugin(typeof(StrideEditorPlugin));

            // Attempt to load the startup session, if available
 /*           if (!UPath.IsNullOrEmpty(initialSessionPath))
            {
                var sessionLoaded = await editor.OpenInitialSession(initialSessionPath);
                if (sessionLoaded == true)
                {
                    var mainWindow = new GameStudioWindow(editor);
                    System.Windows.Application.Current.MainWindow = mainWindow;
                    WindowManager.ShowMainWindow(mainWindow);
                    return;
                }
            }*/

            var viewModel = new NewOrOpenSessionTemplateCollectionViewModel(serviceProvider/*, startupWindow*/);
//            var startupWindow = new AProjectSelectionWindow(viewModel);

//            startupWindow.Templates = viewModel;

            var mainWindow = new AvaloniaMainWindow();

            //desktop.MainWindow = startupWindow;
            //startupWindow.ShowModal();
            doDialog = (s, e) => DoShowProjectSelectionDialogAsync(viewModel, mainWindow, serviceProvider, editor);
            mainWindow.Activated += doDialog;
            //await startupWindow.ShowDialog(mainWindow);

            mainWindow.Show();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
