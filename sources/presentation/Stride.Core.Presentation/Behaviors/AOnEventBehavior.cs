// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;


//using System.Windows;
//using Microsoft.Xaml.Behaviors;
using Stride.Core.Annotations;
using Stride.Core.Presentation.Core;
using Stride.Core.Presentation.Internal;

namespace Stride.Core.Presentation.Behaviors
{
    /// <summary>
    /// An abstract behavior that allows to perform actions when an event is raised. It supports both <see cref="RoutedEvent"/> and standard <c>event</c>,
    /// and allow to catch routed event triggered by any control.
    /// </summary>
    public abstract class AOnEventBehavior : Behavior<AvaloniaObject>
    {
        /// <summary>
        /// Identifies the <see cref="EventName"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<string> EventNameProperty = StyledProperty<string>.Register<AOnEventBehavior, string>("EventName");

        /// <summary>
        /// Identifies the <see cref="EventOwnerType"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<Type> EventOwnerTypeProperty = StyledProperty<Type>.Register<AOnEventBehavior, Type>("EventOwnerType");

        /// <summary>
        /// Identifies the <see cref="HandleEvent"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> HandleEventProperty = StyledProperty<bool>.Register<AOnEventBehavior, bool>("HandleEvent");

        private readonly EventHandler<RoutedEventArgs> routedEventHandler;
        private AnonymousEventHandler eventHandler;
        private RoutedEvent routedEvent;

        protected AOnEventBehavior()
        {
            routedEventHandler = RoutedEventHandler;
        }

        /// <summary>
        /// Gets or sets the name of the event to handle.
        /// </summary>
        public string EventName { get { return (string)GetValue(EventNameProperty); } set { SetValue(EventNameProperty, value); } }

        /// <summary>
        /// Gets or sets the type that owns the event when <see cref="EventName"/> describes a <see cref="RoutedEvent"/>.
        /// </summary>
        public Type EventOwnerType { get { return (Type)GetValue(EventOwnerTypeProperty); } set { SetValue(EventOwnerTypeProperty, value); } }

        /// <summary>
        /// Gets or sets whether to set the event as handled.
        /// </summary>
        public bool HandleEvent { get { return (bool)GetValue(HandleEventProperty); } set { SetValue(HandleEventProperty, value.Box()); } }

        /// <summary>
        /// Invoked when the monitored event is raised.
        /// </summary>
        protected abstract void OnEvent();

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            if (EventName == null)
                throw new ArgumentException($"The EventName property must be set on behavior '{GetType().FullName}'.");

            var eventOwnerType = EventOwnerType ?? AssociatedObject.GetType();

            var uiElement = AssociatedObject as Control;

//            var routedEvents = EventManager.GetRoutedEvents().Where(x => x.Name == EventName && x.OwnerType.IsAssignableFrom(eventOwnerType)).ToArray();
            var routedEvents = RoutedEventRegistry.Instance.GetAllRegistered ().Where(x => x.Name == EventName && x.OwnerType.IsAssignableFrom(eventOwnerType)).ToArray();

            if (uiElement != null && routedEvents.Length > 0)
            {
                if (routedEvents.Length > 1)
                    throw new NotImplementedException("TODO: several events found, find a way to decide the most relevant one.");

                routedEvent = routedEvents.First();
                uiElement.AddHandler(routedEvent, routedEventHandler);
            }
            else
            {
                var eventInfo = AssociatedObject.GetType().GetEvent(EventName);

                if (eventInfo == null)
                    throw new InvalidOperationException($"Impossible to find a valid event named '{EventName}'.");

                eventHandler = AnonymousEventHandler.RegisterEventHandler(eventInfo, AssociatedObject, OnEvent);
            }
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            if (routedEvent != null)
            {
                var uiElement = (Control)AssociatedObject;
                uiElement.RemoveHandler(routedEvent, routedEventHandler);
                routedEvent = null;
            }
            else if (eventHandler != null)
            {
                AnonymousEventHandler.UnregisterEventHandler(eventHandler);
                eventHandler = null;
            }
        }

        private void RoutedEventHandler(object sender, [NotNull] RoutedEventArgs e)
        {
            if (HandleEvent)
            {
                e.Handled = true;
            }
            OnEvent();
        }
    }
}
