// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.ComponentModel;
using System.Threading;
using Avalonia;
using Avalonia.Input;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Stride.Core.Presentation.Internal;
using Stride.Core.Presentation.Behaviors;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Presenters;

namespace Stride.Core.Presentation.Controls
{
    /// <summary>
    /// An implementation of the <see cref="TextBoxBase"/> control that provides additional features such as a proper
    /// validation/cancellation workflow, and a watermark to display when the text is empty.
    /// </summary>
    [TemplatePart(Name = "PART_TextPresenter", Type = typeof(TextPresenter))]
    public class ATextBox : ATextBoxBase
    {
        protected override Type StyleKeyOverride { get { return typeof(ATextBox); } }

        private TextPresenter trimmedTextBlock;
        private readonly Timer validationTimer;

        /// <summary>
        /// Identifies the <see cref="UseTimedValidation"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> UseTimedValidationProperty = StyledProperty<bool>.Register<ATextBox, bool>("UseTimedValidation", false);

        /// <summary>
        /// Identifies the <see cref="ValidationDelay"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<int> ValidationDelayProperty = StyledProperty<int>.Register<ATextBox, int>("ValidationDelay", 500);

        /// <summary>
        /// Identifies the <see cref="TrimmedText"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<string> TrimmedTextProperty = StyledProperty<string>.Register<ATextBox, string>("TrimmedText", "");
//        public static readonly StyledProperty<string> TrimmedTextPropertyKey = StyledProperty<string>.Register<ATextBox, string>("TrimmedText", "");

        /// <summary>
        /// Identifies the <see cref="TrimmedText"/> dependency property.
        /// </summary>
        //public static readonly StyledProperty TrimmedTextProperty = TrimmedTextPropertyKey.StyledProperty;

        /// <summary>
        /// Clears the current <see cref="System.Windows.Controls.TextBox.Text"/> of a text box.
        /// </summary>
        //public static RoutedCommand ClearTextCommand { get; }

        static ATextBox()
        {
          UseTimedValidationProperty.Changed.AddClassHandler<ATextBox>(OnUseTimedValidationPropertyChanged);

//          DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBox), new FrameworkPropertyMetadata(typeof(TextBox)));
//          ClearTextCommand = new RoutedCommand("ClearTextCommand", typeof(System.Windows.Controls.TextBox));
//          CommandManager.RegisterClassCommandBinding(typeof(System.Windows.Controls.TextBox), new CommandBinding(ClearTextCommand, OnClearTextCommand));
        }

        public ATextBox()
        {
//          if (DesignerProperties.GetIsInDesignMode(this) == false)
//              validationTimer = new Timer(x => Dispatcher.InvokeAsync(Validate), null, Timeout.Infinite, Timeout.Infinite);
        }
        
        /// <summary>
        /// Gets or sets whether the text should be automatically validated after a delay defined by the <see cref="ValidationDelay"/> property.
        /// </summary>
        public bool UseTimedValidation { get { return (bool)GetValue(UseTimedValidationProperty); } set { SetValue(UseTimedValidationProperty, value.Box()); } }

        /// <summary>
        /// Gets or sets the amount of time before a validation of input text happens, in milliseconds.
        /// Every change to the <see cref="TextBox.Text"/> property reset the timer to this value.
        /// </summary>
        /// <remarks>The default value is <c>500</c> milliseconds.</remarks>
        public int ValidationDelay { get { return (int)GetValue(ValidationDelayProperty); } set { SetValue(ValidationDelayProperty, value); } }

        /// <summary>
        /// Gets the trimmed text to display when the control does not have the focus, depending of the value of the <see cref="TextTrimming"/> property.
        /// </summary>
        public string TrimmedText { get { return (string)GetValue(TrimmedTextProperty); } private set { SetValue(TrimmedTextProperty, value); } }
        //public string TrimmedText { get { return (string)GetValue(TrimmedTextPropertyKey.DependencyProperty); } private set { SetValue(TrimmedTextPropertyKey, value); } }

        /// <inheritdoc/>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            trimmedTextBlock = e.NameScope.Find<TextPresenter>("PART_TextPresenter");
            if (trimmedTextBlock == null)
                throw new InvalidOperationException("A part named 'PART_TextPresenter' must be present in the ControlTemplate, and must be of type 'TextBlock'.");
        }

        /// <summary>
        /// Raised when the text of the TextBox changes.
        /// </summary>
        /// <param name="oldValue">The old value of the <see cref="TextBox.Text"/> property.</param>
        /// <param name="newValue">The new value of the <see cref="TextBox.Text"/> property.</param>
        protected override void OnTextChanged(string oldValue, string newValue)
        {
            if (UseTimedValidation)
            {
                if (ValidationDelay > 0.0)
                {
                    validationTimer?.Change(ValidationDelay, Timeout.Infinite);
                }
                else
                {
                    Validate();
                }
            }

            var availableWidth = Bounds.Width;
            if (trimmedTextBlock != null)
                availableWidth -= trimmedTextBlock.Margin.Left + trimmedTextBlock.Margin.Right;

            TrimmedText = ATrimming.ProcessTrimming(this.trimmedTextBlock, Text, availableWidth);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var arrangedSize = base.ArrangeOverride(arrangeBounds);
            var availableWidth = arrangeBounds.Width;
            if (trimmedTextBlock != null)
                availableWidth -= trimmedTextBlock.Margin.Left + trimmedTextBlock.Margin.Right;

            TrimmedText = ATrimming.ProcessTrimming(this.trimmedTextBlock, Text, availableWidth);
            
            return arrangedSize;
        }

        private static void OnUseTimedValidationPropertyChanged(AvaloniaObject sender, AvaloniaPropertyChangedEventArgs e)
        {
            var txt = (ATextBox)sender;
            if ((bool)e.NewValue)
            {
                txt.Validate();
            }
        }

        /*
        private static void OnClearTextCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox?.Clear();
        }*/
    }
}
