// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;


//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Input;

using Stride.Core.Presentation.Collections;
using Stride.Core.Presentation.Extensions;

namespace Stride.Core.Presentation.Controls
{
    public class APropertyView : Avalonia.Controls.TreeView
    {
        protected override Type StyleKeyOverride { get { return typeof(APropertyView); } }

        private readonly ObservableList<APropertyViewItem> properties = new ObservableList<APropertyViewItem>();

        /// <summary>
        /// Identifies the <see cref="HighlightedItem"/> dependency property.
        /// </summary>
        public static readonly DirectProperty<APropertyView, APropertyViewItem> HighlightedItemPropertyKey = AvaloniaProperty.RegisterDirect<APropertyView, APropertyViewItem>(nameof(HighlightedItem), o => o.HighlightedItem);

        /// <summary>
        /// Identifies the <see cref="HoveredItem"/> dependency property.
        /// </summary>
        public static readonly DirectProperty<APropertyView, APropertyViewItem> HoveredItemPropertyKey = AvaloniaProperty.RegisterDirect<APropertyView, APropertyViewItem>(nameof(HoveredItem), o => o.HoveredItem);

        /// <summary>
        /// Identifies the <see cref="KeyboardActiveItem"/> dependency property.
        /// </summary>
        public static readonly DirectProperty<APropertyView, APropertyViewItem> KeyboardActiveItemPropertyKey = AvaloniaProperty.RegisterDirect<APropertyView, APropertyViewItem>(nameof(KeyboardActiveItem), o => o.KeyboardActiveItem);
 
        /// <summary>
        /// Identifies the <see cref="NameColumnSize"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<GridLength> NameColumnSizeProperty = StyledProperty<GridLength>.Register<APropertyView, GridLength>(nameof(NameColumnSize), new GridLength(150), defaultBindingMode : Avalonia.Data.BindingMode.TwoWay);

        /// <summary>
        /// Identifies the PreparePropertyItem event.
        /// This attached routed event may be raised by the PropertyGrid itself or by a PropertyItemBase containing sub-items.
        /// </summary>
        public static readonly RoutedEvent PrepareItemEvent = RoutedEvent.Register< APropertyView, APropertyViewItemEventArgs>("PrepareItem", RoutingStrategies.Bubble);

        /// <summary>
        /// Identifies the ClearPropertyItem event.
        /// This attached routed event may be raised by the PropertyGrid itself or by a
        /// PropertyItemBase containing sub items.
        /// </summary>
        public static readonly RoutedEvent ClearItemEvent = RoutedEvent.Register<APropertyView, APropertyViewItemEventArgs >("ClearItem", RoutingStrategies.Bubble);

        static APropertyView()
        {
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(APropertyView), new FrameworkPropertyMetadata(typeof(APropertyView)));
    }

    public APropertyView()
        {
 //           IsKeyboardFocusWithinChanged += OnIsKeyboardFocusWithinChanged;
        }

        public IReadOnlyCollection<APropertyViewItem> Properties => (IReadOnlyCollection < APropertyViewItem > ) properties;

        /// <summary>
        /// Gets the <see cref="PropertyViewItem"/> that is currently highlighted by the mouse cursor.
        /// </summary>
        private APropertyViewItem _HighlightedItem;
        public APropertyViewItem HighlightedItem { get { return _HighlightedItem; } private set { SetAndRaise(HighlightedItemPropertyKey, ref _HighlightedItem, value); } }

        /// <summary>
        /// Gets the <see cref="PropertyViewItem"/> that is currently hovered by the mouse cursor.
        /// </summary>
        private APropertyViewItem _HoveredItem;
        public APropertyViewItem HoveredItem { get { return _HoveredItem; } private set { SetAndRaise(HoveredItemPropertyKey, ref _HoveredItem, value); } }

        /// <summary>
        /// Gets the <see cref="PropertyViewItem"/> that currently owns the control who have the keyboard focus.
        /// </summary>
        private APropertyViewItem _KeyboardActiveItem;
        public APropertyViewItem KeyboardActiveItem { get { return _KeyboardActiveItem; } private set { SetAndRaise(KeyboardActiveItemPropertyKey, ref _KeyboardActiveItem, value); } }

        /// <summary>
        /// Gets or sets the shared size of the 'Name' column.
        /// </summary>
        public GridLength NameColumnSize { get { return (GridLength)GetValue(NameColumnSizeProperty); } set { SetValue(NameColumnSizeProperty, value); } }

        /// <summary>
        /// This event is raised when a property item is about to be displayed in the PropertyGrid.
        /// This allow the user to customize the property item just before it is displayed.
        /// </summary>
        public event EventHandler<APropertyViewItemEventArgs> PrepareItem { add { AddHandler(PrepareItemEvent, value); } remove { RemoveHandler(PrepareItemEvent, value); } }

        /// <summary>
        /// This event is raised when an property item is about to be remove from the display in the PropertyGrid
        /// This allow the user to remove any attached handler in the PreparePropertyItem event.
        /// </summary>
        public event EventHandler<APropertyViewItemEventArgs> ClearItem { add { AddHandler(ClearItemEvent, value); } remove { RemoveHandler(ClearItemEvent, value); } }

 /*       internal void ItemMouseMove(object sender, MouseEventArgs e)
        {
            var item = sender as APropertyViewItem;
            if (item != null)
            {
                if (item.Highlightable)
                    HighlightItem(item);

                HoverItem(item);
            }
        }

        internal void OnIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Equals(sender, this) && !(bool)e.NewValue)
            {
                KeyboardActivateItem(null);
                return;
            }

            // We want to find the closest PropertyViewItem to the element who got the keyboard focus.
            var focusedControl = Keyboard.FocusedElement as DependencyObject;
            if (focusedControl != null)
            {
                var propertyItem = focusedControl as APropertyViewItem ?? focusedControl.FindVisualParentOfType<APropertyViewItem>();
                if (propertyItem != null)
                {
                    KeyboardActivateItem(propertyItem);
                }
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            HoverItem(null);
            HighlightItem(null);
        }*/

        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
        {
            return new APropertyViewItem(this);
        }

        protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
        {
            return NeedsContainer<ATreeViewItem>(item, out recycleKey);
        }

        protected override void PrepareContainerForItemOverride(Control element, object? item, int index)
        {
            base.PrepareContainerForItemOverride(element, item, index);
            var container = (APropertyViewItem)element;
            properties.Add(container);
            RaiseEvent(new APropertyViewItemEventArgs(PrepareItemEvent, this, container, item));
        }

        protected override void ClearContainerForItemOverride(Control element)
        {
            var container = (APropertyViewItem)element;
            RaiseEvent(new APropertyViewItemEventArgs(ClearItemEvent, this, (APropertyViewItem)element, null));
            properties.Remove(container);
            base.ClearContainerForItemOverride(element);
        }

        private void KeyboardActivateItem(APropertyViewItem item)
        {
            KeyboardActiveItem?.SetValue(APropertyViewItem.IsKeyboardActivePropertyKey, false);
            KeyboardActiveItem = item;
            KeyboardActiveItem?.SetValue(APropertyViewItem.IsKeyboardActivePropertyKey, true);
        }

        private void HoverItem(APropertyViewItem item)
        {
            HoveredItem?.SetValue(APropertyViewItem.IsHoveredPropertyKey, false);
            HoveredItem = item;
            HoveredItem?.SetValue(APropertyViewItem.IsHoveredPropertyKey, true);
        }

        private void HighlightItem(APropertyViewItem item)
        {
            HighlightedItem?.SetValue(APropertyViewItem.IsHighlightedPropertyKey, false);
            HighlightedItem = item;
            HighlightedItem?.SetValue(APropertyViewItem.IsHighlightedPropertyKey, true);
        }


        //protected override AutomationPeer OnCreateAutomationPeer()
        //{
        //    return (AutomationPeer)new TreeViewAutomationPeer(this);
        //}
    }
}
