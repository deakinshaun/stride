// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
//using System.Windows;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

//using Microsoft.Xaml.Behaviors;
using Stride.Core.Annotations;
using Stride.Core.Presentation.Commands;
using Stride.Core.Presentation.Internal;

namespace Stride.Core.Presentation.Behaviors
{
    /// <summary>
    /// This command will bind a <see cref="ICommandBase"/> to a <see cref="RoutedCommand"/>. It works just as a <see cref="CommandBinding"/> except that the bound
    /// command is executed when the routed command is executed. The <b>CanExecute</b> handler also invoke the <b>CanExecute</b> method of the <see cref="ICommandBase"/>.
    /// </summary>
    public class ACommandBindingBehavior : Behavior<Control>
    {
        private CommandBinding commandBinding;

        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<ICommandBase> CommandProperty =
            StyledProperty<ICommandBase>.Register<ACommandBindingBehavior, ICommandBase>(nameof(Command), null);
        /// <summary>
        /// Identifies the <see cref="ContinueRouting"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool>ContinueRoutingProperty =
            StyledProperty<bool>.Register<ACommandBindingBehavior, bool>(nameof(ContinueRouting), true);
        /// <summary>
        /// Identifies the <see cref="IsEnabled"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> IsEnabledProperty =
            StyledProperty<bool>.Register<ACommandBindingBehavior, bool>(nameof(IsEnabled), true);
        /// <summary>
        /// Identifies the <see cref="RoutedCommand"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<RoutedCommand> RoutedCommandProperty =
            StyledProperty<RoutedCommand>.Register<ACommandBindingBehavior, RoutedCommand>(nameof(RoutedCommand));

        /// <summary>
        /// Gets or sets the <see cref="ICommandBase"/> to bind.
        /// </summary>
        public ICommandBase Command { get { return (ICommandBase)GetValue(CommandProperty); } set { SetValue(CommandProperty, value); } }

        /// <summary>
        /// Gets or sets whether the input routed event that invoked the command should continue to route through the element tree.
        /// </summary>
        /// <seealso cref="CanExecuteRoutedEventArgs.ContinueRouting"/>
        public bool ContinueRouting { get { return (bool)GetValue(ContinueRoutingProperty); } set { SetValue(ContinueRoutingProperty, value.Box()); } }

        /// <summary>
        /// Gets or sets whether this command binding is enabled. When disabled, the <see cref="Command"/> won't be executed.
        /// </summary>
        public bool IsEnabled { get { return (bool)GetValue(IsEnabledProperty); } set { SetValue(IsEnabledProperty, value.Box()); } }

        /// <summary>
        /// Gets or sets the <see cref="RoutedCommand"/> to bind.
        /// </summary>
        public RoutedCommand RoutedCommand { get { return (RoutedCommand)GetValue(RoutedCommandProperty); } set { SetValue(RoutedCommandProperty, value); } }

        static ACommandBindingBehavior ()
        {
            CommandProperty.Changed.AddClassHandler<ACommandBindingBehavior>(CommandChanged);
        }
        /// <inheritdoc/>
        protected override void OnAttached()
        {
            commandBinding = new CommandBinding(RoutedCommand, (s, e) => OnExecuted(e), (s, e) => OnCanExecute(e));
 //           AssociatedObject.CommandBindings.Add(commandBinding);
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
 //           AssociatedObject.CommandBindings.Remove(commandBinding);
        }

        private static void CommandChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }

        private void OnCanExecute([NotNull] CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
        {
            if (Command != null)
            {
                canExecuteRoutedEventArgs.CanExecute = IsEnabled && Command.CanExecute(canExecuteRoutedEventArgs.Parameter);
            }
            else
            {
                canExecuteRoutedEventArgs.CanExecute = false;
            }

            if (canExecuteRoutedEventArgs.CanExecute || !ContinueRouting)
            {
                canExecuteRoutedEventArgs.Handled = true;
            }
            else
            {
                canExecuteRoutedEventArgs.ContinueRouting = true;
            }
        }

        private void OnExecuted([NotNull] ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            if (Command != null && IsEnabled)
            {
                Command.Execute(executedRoutedEventArgs.Parameter);
                executedRoutedEventArgs.Handled = true;
            }
        }
    }
}
