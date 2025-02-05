// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Reflection;
using Avalonia;

//using System.Windows;
using System.Windows.Input;
//using Microsoft.Xaml.Behaviors;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Stride.Core.Annotations;
using Stride.Core.Presentation.Services;

namespace Stride.Core.Presentation.Behaviors
{
    /// <summary>
    /// A base behavior that will close the window it is contained in an event occurs on a control. A command can be executed
    /// before closing the window by using the <see cref="Command"/> and <see cref="CommandParameter"/> properties of this behavior.
    /// </summary>
    public abstract class ACloseWindowBehavior<T> : Behavior<T> where T : AvaloniaObject
    {
        /// <summary>
        /// Identifies the <see cref="DialogResult"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<DialogResult> DialogResultProperty = StyledProperty<DialogResult>.Register<ACloseWindowBehavior<T>, DialogResult>("DialogResult");

        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<ICommand> CommandProperty = StyledProperty<ICommand>.Register<ACloseWindowBehavior<T>, ICommand>("Command");

        /// <summary>
        /// Identifies the <see cref="CommandParameter"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<object> CommandParameterProperty = StyledProperty<object>.Register<ACloseWindowBehavior<T>, object>("CommandParameter");

        /// <summary>
        /// Gets or sets the value to set to the <see cref="Window.DialogResult"/> property of the window the associated button is contained in.
        /// </summary>
        public DialogResult DialogResult { get { return (DialogResult)GetValue(DialogResultProperty); } set { SetValue(DialogResultProperty, value); } }

        /// <summary>
        /// Gets or sets the command to execute before closing the window.
        /// </summary>
        public ICommand Command { get { return (ICommand)GetValue(CommandProperty); } set { SetValue(CommandProperty, value); } }

        /// <summary>
        /// Gets or sets the parameter of the command to execute before closing the window.
        /// </summary>
        public object CommandParameter { get { return GetValue(CommandParameterProperty); } set { SetValue(CommandParameterProperty, value); } }

        static ACloseWindowBehavior ()
        {
           CommandProperty.Changed.AddClassHandler<ACloseWindowBehavior<T>>(CommandChanged);
           CommandParameterProperty.Changed.AddClassHandler<ACloseWindowBehavior<T>>(CommandParameterChanged);
        }

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();
            if (Command != null)
            {
                AssociatedObject.SetCurrentValue(Control.IsEnabledProperty, Command.CanExecute(CommandParameter));
            }
        }
        
        private static void CommandChanged([NotNull] AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var behavior = (AButtonCloseWindowBehavior)d;
            var oldCommand = e.OldValue as ICommand;
            var newCommand = e.NewValue as ICommand;

            if (oldCommand != null)
            {
                oldCommand.CanExecuteChanged -= behavior.CommandCanExecuteChanged;
            }
            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += behavior.CommandCanExecuteChanged;
            }
        }
        
        private static void CommandParameterChanged([NotNull] AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var behavior = (AButtonCloseWindowBehavior)d;
            if (behavior.Command != null)
            {
                behavior.AssociatedObject.SetCurrentValue(Control.IsEnabledProperty, behavior.Command.CanExecute(behavior.CommandParameter));
            }
        }
        
        private void CommandCanExecuteChanged(object sender, EventArgs e)
        {
            AssociatedObject.SetCurrentValue(Control.IsEnabledProperty, Command.CanExecute(CommandParameter));
        }
        
        /// <summary>
        /// Invokes the command and close the containing window.
        /// </summary>
        protected void Close()
        {
            if (Command != null && Command.CanExecute(CommandParameter))
            {
                Command.Execute(CommandParameter);
            }

            //            var window = Window.GetWindow(AssociatedObject);
            var window = TopLevel.GetTopLevel(AssociatedObject as Control) as Window;
            if (window == null) throw new InvalidOperationException("The button attached to this behavior is not in a window");

            bool dialogResultUpdated = false;
            bool processed = false;
            var modal = window as IModalDialogInternal;

            if (modal != null)
            {
                modal.Result = DialogResult;
                processed = true;
            }

            // Window.DialogResult setter will throw an exception when the window was not displayed with ShowDialog, even if we're setting null.
            /*if (WpfModalHelper.IsModal(window))
            {
                if (DialogResult != WpfModalHelper.ToDialogResult(window.DialogResult))
                {
                    // Setting DialogResult to a non-null value will close the window, we don't want to invoke Close after that.
                    window.DialogResult = WpfModalHelper.ToDialogResult(DialogResult);
                    dialogResultUpdated = true;
                }
                processed = true;
            }*/

            if (DialogResult != DialogResult.None && !processed)
            {
//                throw new InvalidOperationException("The DialogResult can be set by a CloseWindowBehavior only if the window has been shown with ShowDialog or implements IModalDialogInternal");
            }

            //if (window.DialogResult == null || !dialogResultUpdated)
            {
                window.Close();
            }
        }
    }

    /*internal static class WpfModalHelper
    {
        private static readonly FieldInfo ShowingAsDialog;
        private const string FieldName = "_showingAsDialog";

        static WpfModalHelper()
        {
            ShowingAsDialog = typeof(Window).GetField(FieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (ShowingAsDialog == null)
                throw new MissingFieldException(nameof(Window), FieldName);
        }

        public static bool IsModal([NotNull] Window window)
        {
            return (bool)ShowingAsDialog.GetValue(window);
        }

        public static DialogResult ToDialogResult(bool? dialogResult)
        {
            if (dialogResult.HasValue)
            {
                return dialogResult.Value ? DialogResult.Ok : DialogResult.Cancel;
            }
            return DialogResult.None;
        }

        public static bool? ToDialogResult(DialogResult dialogResult)
        {
            switch (dialogResult)
            {
                case DialogResult.None:
                    return null;
                case DialogResult.Ok:
                    return true;
                case DialogResult.Cancel:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dialogResult), dialogResult, null);
            }
        }
    }*/
}

