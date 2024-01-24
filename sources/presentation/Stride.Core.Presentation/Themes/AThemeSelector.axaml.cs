// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Media;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Stride.Core.Presentation.Themes
{
    public partial class AThemeSelector : ResourceDictionary
    {
        public AThemeSelector()
        {
            InitializeComponent();
        }
        public void InitializeComponent(bool loadXaml = true, bool attachDevTools = true)
        {
            if (loadXaml)
            {
                AvaloniaXamlLoader.Load(this);
            }
        }

            private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Image img && img.Source is DrawingImage drawingImage)
            {
                img.Source = new DrawingImage
                {
  //                  Drawing = ImageThemingUtilities.TransformDrawing(drawingImage.Drawing, ThemeController.CurrentTheme.GetThemeBase().GetIconTheme())
                };
            }
        }
    }
}
