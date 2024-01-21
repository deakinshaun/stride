// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Stride.Core.Assets.Editor.View.Behaviors;
using AvalonDock.Layout;
using Avalonia.Controls;

namespace Stride.GameStudio.Layout.Behaviors
{
    /// <summary>
    /// An implementation of the <see cref="ActivateOnLocationChangedBehavior{T}"/> for the <see cref="LayoutAnchorable"/> control.
    /// </summary>
    public class ALayoutAnchorableActivateOnLocationChangedBehavior : AActivateOnLocationChangedBehavior<Dock.Model.Avalonia.Core.DockableBase>
    {
        protected override void Activate()
        {
//            AssociatedObject.Show();
//            AssociatedObject.IsSelected = true;
            AssociatedObject.OnSelected();
        }
    }
}
