using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Stride.Core.Assets.Editor.Components.TemplateDescriptions.Views;

namespace Stride.GameStudio;

public partial class AvaloniaApp : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new AProjectSelectionWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
