// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Input;
using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Stride.Core.Annotations;
using Stride.Core.Presentation.Internal;

namespace Stride.Core.Presentation.Controls
{
    public class AExpandableItemsControl : HeaderedItemsControl
    {
        /// <summary>
        /// Identifies the <see cref="IsExpanded"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> IsExpandedProperty = StyledProperty<bool>.Register<AExpandableItemsControl, bool>(nameof(IsExpanded), false);

        /// <summary>
        /// Identifies the <see cref="Expanded"/> routed event.
        /// </summary>
        public static readonly RoutedEvent ExpandedEvent = RoutedEvent.Register<AExpandableItemsControl, RoutedEventArgs>(nameof(Expanded), RoutingStrategies.Bubble);

        /// <summary>
        /// Identifies the <see cref="Collapsed"/> routed event.
        /// </summary>
        public static readonly RoutedEvent CollapsedEvent = RoutedEvent.Register<AExpandableItemsControl, RoutedEventArgs>(nameof(Collapsed), RoutingStrategies.Bubble);

        /// <summary>
        /// Gets or sets whether this control is expanded.
        /// </summary>
        public bool IsExpanded { get { return (bool)GetValue(IsExpandedProperty); } set { SetValue(IsExpandedProperty, value.Box()); } }

        protected bool CanExpand => (ItemCount > 0);

        /// <summary>
        /// Raised when this <see cref="ExpandableItemsControl"/> is expanded.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Expanded { add { AddHandler(ExpandedEvent, value); } remove { RemoveHandler(ExpandedEvent, value); } }

        /// <summary>
        /// Raised when this <see cref="ExpandableItemsControl"/> is collapsed.
        /// </summary>
        public event EventHandler<RoutedEventArgs> Collapsed { add { AddHandler(CollapsedEvent, value); } remove { RemoveHandler(CollapsedEvent, value); } }

        static AExpandableItemsControl ()
        {
            IsExpandedProperty.Changed.AddClassHandler<AExpandableItemsControl>(OnIsExpandedChanged);
        }

    /// <summary>
    /// Invoked when this <see cref="ExpandableItemsControl"/> is expanded. Raises the <see cref="Expanded"/> event.
    /// </summary>
    /// <param name="e">The routed event arguments.</param>
    protected virtual void OnExpanded([NotNull] RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        /// <summary>
        /// Invoked when this <see cref="ExpandableItemsControl"/> is collapsed. Raises the <see cref="Collapsed"/> event.
        /// </summary>
        /// <param name="e">The routed event arguments.</param>
        protected virtual void OnCollapsed([NotNull] RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        /// <inheritdoc/>
//        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            if (!e.Handled && IsEnabled && e.ClickCount % 2 == 0)
            {
                SetCurrentValue(IsExpandedProperty, !IsExpanded);
                e.Handled = true;
            }
            base.OnPointerPressed(e);
        }

        private static void OnIsExpandedChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var item = (AExpandableItemsControl)d;
            var isExpanded = (bool)e.NewValue;

            if (isExpanded)
                item.OnExpanded(new RoutedEventArgs(ExpandedEvent, item));
            else
                item.OnCollapsed(new RoutedEventArgs(CollapsedEvent, item));
        }
    }
}
