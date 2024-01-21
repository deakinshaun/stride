// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
//using System.Windows;
//using System.Windows.Controls;
//using Microsoft.Xaml.Behaviors;
using Stride.Core.Presentation.Extensions;
using AvalonDock.Controls;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using Avalonia.VisualTree;

namespace Stride.GameStudio.Layout.Behaviors
{
    // TODO: this behavior was previously broken, it might work now (migration to AvalonDock) but has not been tested!
    public class AActivateParentPaneOnGotFocusBehavior : Behavior<Control>
    {
        protected override void OnAttached()
        {
            AssociatedObject.GotFocus += GotFocus;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.GotFocus -= GotFocus;
        }

        private void GotFocus(object sender, RoutedEventArgs e)
        {
            //var pane = AssociatedObject.FindVisualParentOfType<LayoutAnchorableControl>();
            var pane = AssociatedObject.GetVisualParent<Dock.Model.Avalonia.Core.DockBase>();
            if (pane != null)
            {
                pane.IsActive = true;
            }
        }
    }
}
