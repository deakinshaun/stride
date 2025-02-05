// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.Collections.Specialized;
using System.Windows;
using Avalonia;
using Avalonia.Xaml.Interactivity;

//using System.Windows;
//using Microsoft.Xaml.Behaviors;
using Stride.Core.Annotations;

namespace Stride.Core.Presentation.Behaviors
{
    /// <summary>
    /// The base class for a behavior that allows to activate the associated object when an observable collection changes.
    /// </summary>
    /// <typeparam name="T">The type the <see cref="ActivateOnCollectionChangedBehavior{T}"/> can be attached to.</typeparam>
    public abstract class AActivateOnCollectionChangedBehavior<T> : Behavior<T> where T : AvaloniaObject
    {
        /// <summary>
        /// Identifies the <see cref="Collection"/> dependency property.
        /// </summary>
        public static StyledProperty<INotifyCollectionChanged> CollectionProperty = StyledProperty<INotifyCollectionChanged>.Register< AActivateOnCollectionChangedBehavior < T >, INotifyCollectionChanged >(nameof(Collection));

        /// <summary>
        /// Gets or sets the collection to observe in order to trigger activation of the associated control.
        /// </summary>
        public INotifyCollectionChanged Collection
        {
            get { return (INotifyCollectionChanged)GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        static AActivateOnCollectionChangedBehavior ()
        {
            CollectionProperty.Changed.AddClassHandler<AActivateOnCollectionChangedBehavior<T>>(OnCollectionChanged);
        }

        private static void OnCollectionChanged([NotNull] AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var behavior = (AActivateOnCollectionChangedBehavior<T>)d;
            if (e.OldValue != null)
            {
                var oldValue = (INotifyCollectionChanged)e.OldValue;
                oldValue.CollectionChanged -= behavior.CollectionChanged;
            }
            if (e.NewValue != null)
            {
                var newValue = (INotifyCollectionChanged)e.NewValue;
                newValue.CollectionChanged += behavior.CollectionChanged;
            }
        }

        /// <summary>
        /// Activates the associated object.
        /// </summary>
        protected abstract void Activate();

        protected virtual bool MatchChange([NotNull] NotifyCollectionChangedEventArgs e)
        {
            return true;
        }

        private void CollectionChanged(object sender, [NotNull] NotifyCollectionChangedEventArgs e)
        {
            if (MatchChange(e))
            {
                Activate();
            }
        }
    }
}
