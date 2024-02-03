// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using Avalonia;
using Avalonia.Controls;


//using System.Windows;
//using System.Windows.Input;
using Stride.Core.Annotations;
using Stride.Core.Presentation.Collections;
using Stride.Core.Presentation.Extensions;
using Stride.Core.Presentation.Internal;

namespace Stride.Core.Presentation.Controls
{
    /// <summary>
    /// This class represents an item container of the <see cref="PropertyView"/> control.
    /// </summary>
    public class APropertyViewItem : AExpandableItemsControl
    {
        protected override Type StyleKeyOverride { get { return typeof(APropertyViewItem); } }

        private readonly ObservableList<APropertyViewItem> properties = new ObservableList<APropertyViewItem>();

        /// <summary>
        /// Identifies the <see cref="Highlightable"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> HighlightableProperty = StyledProperty<bool>.Register<APropertyViewItem, bool>("Highlightable", true);

        /// <summary>
        /// Identifies the <see cref="IsHighlighted"/> dependency property.
        /// </summary>
        public static readonly DirectProperty<APropertyViewItem, bool> IsHighlightedPropertyKey = AvaloniaProperty.RegisterDirect<APropertyViewItem, bool>("IsHighlighted", o => o.IsHighlighted);

        /// <summary>
        /// Identifies the <see cref="IsHovered"/> dependency property.
        /// </summary>
        public static readonly DirectProperty<APropertyViewItem, bool> IsHoveredPropertyKey = AvaloniaProperty.RegisterDirect<APropertyViewItem, bool>("IsHovered", o => o.IsHovered);

        /// <summary>
        /// Identifies the <see cref="IsKeyboardActive"/> dependency property.
        /// </summary>
        public static readonly DirectProperty<APropertyViewItem, bool> IsKeyboardActivePropertyKey = AvaloniaProperty.RegisterDirect<APropertyViewItem, bool>("IsKeyboardActive", o => o.IsKeyboardActive);

        /// <summary>
        /// Identifies the <see cref="Offset"/> dependency property.
        /// </summary>
        public static readonly DirectProperty<APropertyViewItem, double> OffsetPropertyKey = AvaloniaProperty.RegisterDirect<APropertyViewItem, double>("Offset", o => o.Offset);

        /// <summary>
        /// Identifies the <see cref="Increment"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> IncrementProperty = StyledProperty<double>.Register<APropertyViewItem, double>("Increment", 0.0);

        static APropertyViewItem()
        {
 //           DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyViewItem), new FrameworkPropertyMetadata(typeof(PropertyViewItem)));

            IncrementProperty.Changed.AddClassHandler<APropertyViewItem>(OnIncrementChanged);

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyViewItem"/> class.
    /// </summary>
    /// <param name="propertyView">The <see cref="PropertyView"/> instance in which this <see cref="PropertyViewItem"/> is contained.</param>
    public APropertyViewItem([NotNull] APropertyView propertyView)
        {
            if (propertyView == null) throw new ArgumentNullException(nameof(propertyView));
            PropertyView = propertyView;
 //           PreviewMouseMove += propertyView.ItemMouseMove;
 //           IsKeyboardFocusWithinChanged += propertyView.OnIsKeyboardFocusWithinChanged;
        }

        /// <summary>
        /// Gets the <see cref="PropertyView"/> control containing this instance of <see cref="PropertyViewItem"/>.
        /// </summary>
        public APropertyView PropertyView { get; }

        /// <summary>
        /// Gets the collection of <see cref="PropertyViewItem"/> instance contained in this control.
        /// </summary>
        public IReadOnlyCollection<APropertyViewItem> Properties => properties;

        /// <summary>
        /// Gets or sets whether this control can be highlighted.
        /// </summary>
        /// <seealso cref="IsHighlighted"/>
        public bool Highlightable { get { return (bool)GetValue(HighlightableProperty); } set { SetValue(HighlightableProperty, value.Box()); } }

        /// <summary>
        /// Gets whether this control is highlighted. The control is highlighted when <see cref="IsHovered"/> and <see cref="Highlightable"/> are both <c>true</c>
        /// </summary>
        /// <seealso cref="Highlightable"/>
        /// <seealso cref="IsHovered"/>
        private bool _IsHighlighted;
        public bool IsHighlighted { get { return _IsHighlighted; } private set { SetAndRaise(IsHighlightedPropertyKey, ref _IsHighlighted, value); } }

        /// <summary>
        /// Gets whether the mouse cursor is currently over this control.
        /// </summary>
        private bool _IsHovered;
        public bool IsHovered { get { return _IsHovered; } private set { SetAndRaise(IsHoveredPropertyKey, ref _IsHovered, value); } }

        /// <summary>
        /// Gets whether this control is the closest control to the control that has the keyboard focus.
        /// </summary>
        private bool _IsKeyboardActive;
        public bool IsKeyboardActive { get { return _IsKeyboardActive; } private set { SetAndRaise(IsKeyboardActivePropertyKey, ref _IsKeyboardActive, value); } }

        /// <summary>
        /// Gets the absolute offset of this <see cref="PropertyViewItem"/>.
        /// </summary>
        private double _Offset;
        public double Offset { get { return _Offset; } private set { SetAndRaise(OffsetPropertyKey, ref _Offset, value); } }

        /// <summary>
        /// Gets or set the increment value used to calculate the <see cref="Offset "/>of the <see cref="PropertyViewItem"/> contained in the <see cref="Properties"/> of this control..
        /// </summary>
        public double Increment { get { return (double)GetValue(IncrementProperty); } set { SetValue(IncrementProperty, value); } }

        /// <inheritdoc/>
        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
        {
            var itema = new APropertyViewItem(PropertyView) { Offset = Offset + Increment };
            return itema;
        }

        /// <inheritdoc/>
        protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
        {
            return NeedsContainer<ATreeViewItem>(item, out recycleKey);
        }

        /// <inheritdoc/>
        protected override void PrepareContainerForItemOverride(Control element, object? item, int index)
        {
            base.PrepareContainerForItemOverride(element, item, index);
            var container = (APropertyViewItem)element;
            properties.Add(container);
            RaiseEvent(new APropertyViewItemEventArgs(APropertyView.PrepareItemEvent, this, container, item));
        }

        /// <inheritdoc/>
        protected override void ClearContainerForItemOverride(Control element)
        {
            var container = (APropertyViewItem)element;
            RaiseEvent(new APropertyViewItemEventArgs(APropertyView.ClearItemEvent, this, (APropertyViewItem)element, null));
            properties.Remove(container);
            base.ClearContainerForItemOverride(element);
        }

 /*       protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            // base method can handle this event, but we still want to focus on it in this case.
            var handled = e.Handled;
            base.OnMouseLeftButtonDown(e);
            if (!handled && IsEnabled)
            {
                Focus();
                e.Handled = true;
            }
        }*/

        // TODO
        //protected override AutomationPeer OnCreateAutomationPeer()
        //{
        //    return (AutomationPeer)new TreeViewItemAutomationPeer(this);
        //}

        private static void OnIncrementChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var item = (APropertyViewItem)d;
  /*          var delta = (double)e.NewValue - (double)e.OldValue;
            var subItems = item.FindVisualChildrenOfType<APropertyViewItem>();
            foreach (var subItem in subItems)
            {
                subItem.Offset += delta;
            }*/
        }
    }
}
