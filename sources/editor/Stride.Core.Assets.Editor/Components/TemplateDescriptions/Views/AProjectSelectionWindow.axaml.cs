using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Stride.Core.Assets.Editor.Components.TemplateDescriptions.Views
{
    public partial class AProjectSelectionWindow : Window
    {
        public AProjectSelectionWindow()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
