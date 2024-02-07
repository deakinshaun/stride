// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;


//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Controls.Primitives;
//using System.Windows.Data;
//using System.Windows.Input;
using Stride.Core.Annotations;
using Stride.Core.Mathematics;
using Stride.Core.Presentation.Core;
using Stride.Core.Presentation.Extensions;
using Stride.Core.Presentation.Internal;
using Stride.Core.Presentation.ValueConverters;
using Stride.Core.Yaml.Tokens;

namespace Stride.Core.Presentation.Controls
{
    /// <summary>
    /// An enum describing when the related <see cref="NumericTextBox"/> should be validated, when the user uses the mouse to change its value.
    /// </summary>
    public enum AMouseValidationTrigger
    {
        /// <summary>
        /// The validation occurs every time the mouse moves.
        /// </summary>
        OnMouseMove,
        /// <summary>
        /// The validation occurs when the mouse button is released.
        /// </summary>
        OnMouseUp,
    }

    public class ARepeatButtonPressedRoutedEventArgs : RoutedEventArgs
    {
        public ARepeatButtonPressedRoutedEventArgs(ANumericTextBox.RepeatButtons button, RoutedEvent routedEvent)
            : base(routedEvent)
        {
            Button = button;
        }

        public ANumericTextBox.RepeatButtons Button { get; private set; }
    }

    /// <summary>
    /// A specialization of the <see cref="TextBoxBase"/> control that can be used for numeric values.
    /// It contains a <see cref="Value"/> property that is updated on validation.
    /// </summary>
    /// PART_IncreaseButton") as RepeatButton;
    [TemplatePart(Name = "PART_IncreaseButton", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_DecreaseButton", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_ContentHost", Type = typeof(ScrollViewer))]
    public class ANumericTextBox : ATextBoxBase
    {
        protected override Type StyleKeyOverride { get { return typeof(ANumericTextBox); } }
        public enum RepeatButtons
        {
            IncreaseButton,
            DecreaseButton,
        }

        private RepeatButton increaseButton;
        private RepeatButton decreaseButton;
        // FIXME: turn back private
        internal ScrollViewer contentHost;
        private bool updatingValue;

        /// <summary>
        /// The amount of pixel to move the mouse in order to add/remove a <see cref="SmallChange"/> to the current <see cref="Value"/>.
        /// </summary>
        public static readonly double DragSpeed = 3;

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double?> ValueProperty = StyledProperty<double?>.Register<ANumericTextBox, double?>(nameof(Value), 0.0, defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <see cref="DecimalPlaces"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<int> DecimalPlacesProperty = StyledProperty<int>.Register<ANumericTextBox, int>(nameof(DecimalPlaces), -1);

        /// <summary>
        /// Identifies the <see cref="Minimum"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> MinimumProperty = StyledProperty<double>.Register<ANumericTextBox, double>(nameof(Minimum), double.MinValue);

        /// <summary>
        /// Identifies the <see cref="Maximum"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> MaximumProperty = StyledProperty<double>.Register<ANumericTextBox, double>(nameof(Maximum), double.MaxValue);

        /// <summary>
        /// Identifies the <see cref="ValueRatio"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> ValueRatioProperty = StyledProperty<double>.Register<ANumericTextBox, double>(nameof(ValueRatio), default(double));

        /// <summary>
        /// Identifies the <see cref="LargeChange"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> LargeChangeProperty = StyledProperty<double>.Register<ANumericTextBox, double>(nameof(LargeChange), 10.0);

        /// <summary>
        /// Identifies the <see cref="SmallChange"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> SmallChangeProperty = StyledProperty<double>.Register<ANumericTextBox, double>(nameof(SmallChange), 1.0);

        /// <summary>
        /// Identifies the <see cref="DisplayUpDownButtons"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> DisplayUpDownButtonsProperty = StyledProperty<bool>.Register<ANumericTextBox, bool>(nameof(DisplayUpDownButtons), true);

        /// <summary>
        /// Identifies the <see cref="AllowMouseDrag"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> AllowMouseDragProperty = StyledProperty<bool>.Register<ANumericTextBox, bool>(nameof(AllowMouseDrag), true);

        /// <summary>
        /// Identifies the <see cref="MouseValidationTrigger"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<AMouseValidationTrigger> MouseValidationTriggerProperty = StyledProperty<AMouseValidationTrigger>.Register<ANumericTextBox, AMouseValidationTrigger>("MouseValidationTrigger", AMouseValidationTrigger.OnMouseUp);

        /// <summary>
        /// Raised when the <see cref="Value"/> property has changed.
        /// </summary>
        public static readonly RoutedEvent ValueChangedEvent = RoutedEvent.Register<ANumericTextBox, RoutedEventArgs>("ValueChanged", RoutingStrategies.Bubble);

        /// <summary>
        /// Raised when one of the repeat button is pressed.
        /// </summary>
        public static readonly RoutedEvent RepeatButtonPressedEvent = RoutedEvent.Register < ANumericTextBox, ARepeatButtonPressedRoutedEventArgs>("RepeatButtonPressed", RoutingStrategies.Bubble);

        /// <summary>
        /// Raised when one of the repeat button is released.
        /// </summary>
        public static readonly RoutedEvent RepeatButtonReleasedEvent = RoutedEvent.Register < ANumericTextBox, ARepeatButtonPressedRoutedEventArgs>("RepeatButtonReleased", RoutingStrategies.Bubble);

        /// <summary>
        /// Increases the current value with the value of the <see cref="LargeChange"/> property.
        /// </summary>
        public static RoutedCommand LargeIncreaseCommand { get; }

        /// <summary>
        /// Increases the current value with the value of the <see cref="SmallChange"/> property.
        /// </summary>
        public static RoutedCommand SmallIncreaseCommand { get; }

        /// <summary>
        /// Decreases the current value with the value of the <see cref="LargeChange"/> property.
        /// </summary>
        public static RoutedCommand LargeDecreaseCommand { get; }

        /// <summary>
        /// Decreases the current value with the value of the <see cref="SmallChange"/> property.
        /// </summary>
        public static RoutedCommand SmallDecreaseCommand { get; }

        /// <summary>
        /// Resets the current value to zero.
        /// </summary>
        public static RoutedCommand ResetValueCommand { get; }

        static ANumericTextBox()
        {
          ValueProperty.Changed.AddClassHandler<ANumericTextBox> (OnValuePropertyChanged);
          DecimalPlacesProperty.Changed.AddClassHandler<ANumericTextBox>(OnDecimalPlacesPropertyChanged);
          MinimumProperty.Changed.AddClassHandler<ANumericTextBox>(OnMinimumPropertyChanged);
          MaximumProperty.Changed.AddClassHandler<ANumericTextBox>(OnMaximumPropertyChanged);
          ValueRatioProperty.Changed.AddClassHandler<ANumericTextBox>(ValueRatioChanged);



      //  DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericTextBox), new FrameworkPropertyMetadata(typeof(NumericTextBox)));
      //      HorizontalScrollBarVisibilityProperty.OverrideMetadata(typeof(NumericTextBox), new FrameworkPropertyMetadata(ScrollBarVisibility.Hidden, OnForbiddenPropertyChanged));
      //      VerticalScrollBarVisibilityProperty.OverrideMetadata(typeof(NumericTextBox), new FrameworkPropertyMetadata(ScrollBarVisibility.Hidden, OnForbiddenPropertyChanged));
      //      AcceptsReturnProperty.OverrideMetadata(typeof(NumericTextBox), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, OnForbiddenPropertyChanged));
      //      AcceptsTabProperty.OverrideMetadata(typeof(NumericTextBox), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, OnForbiddenPropertyChanged));

            // Since the NumericTextBox is not focusable itself, we have to bind the commands to the inner text box of the control.
            // The handlers will then find the parent that is a NumericTextBox and process the command on this control if it is found.
 /*           LargeIncreaseCommand = new RoutedCommand("LargeIncreaseCommand", typeof(System.Windows.Controls.TextBox));
            CommandManager.RegisterClassCommandBinding(typeof(System.Windows.Controls.TextBox), new CommandBinding(LargeIncreaseCommand, OnLargeIncreaseCommand));
            CommandManager.RegisterClassInputBinding(typeof(System.Windows.Controls.TextBox), new InputBinding(LargeIncreaseCommand, new KeyGesture(Key.PageUp)));
            CommandManager.RegisterClassInputBinding(typeof(System.Windows.Controls.TextBox), new InputBinding(LargeIncreaseCommand, new KeyGesture(Key.Up, ModifierKeys.Shift)));

            LargeDecreaseCommand = new RoutedCommand("LargeDecreaseCommand", typeof(System.Windows.Controls.TextBox));
            CommandManager.RegisterClassCommandBinding(typeof(System.Windows.Controls.TextBox), new CommandBinding(LargeDecreaseCommand, OnLargeDecreaseCommand));
            CommandManager.RegisterClassInputBinding(typeof(System.Windows.Controls.TextBox), new InputBinding(LargeDecreaseCommand, new KeyGesture(Key.PageDown)));
            CommandManager.RegisterClassInputBinding(typeof(System.Windows.Controls.TextBox), new InputBinding(LargeDecreaseCommand, new KeyGesture(Key.Down, ModifierKeys.Shift)));

            SmallIncreaseCommand = new RoutedCommand("SmallIncreaseCommand", typeof(System.Windows.Controls.TextBox));
            CommandManager.RegisterClassCommandBinding(typeof(System.Windows.Controls.TextBox), new CommandBinding(SmallIncreaseCommand, OnSmallIncreaseCommand));
            CommandManager.RegisterClassInputBinding(typeof(System.Windows.Controls.TextBox), new InputBinding(SmallIncreaseCommand, new KeyGesture(Key.Up)));

            SmallDecreaseCommand = new RoutedCommand("SmallDecreaseCommand", typeof(System.Windows.Controls.TextBox));
            CommandManager.RegisterClassCommandBinding(typeof(System.Windows.Controls.TextBox), new CommandBinding(SmallDecreaseCommand, OnSmallDecreaseCommand));
            CommandManager.RegisterClassInputBinding(typeof(System.Windows.Controls.TextBox), new InputBinding(SmallDecreaseCommand, new KeyGesture(Key.Down)));

            ResetValueCommand = new RoutedCommand("ResetValueCommand", typeof(System.Windows.Controls.TextBox));
            CommandManager.RegisterClassCommandBinding(typeof(System.Windows.Controls.TextBox), new CommandBinding(ResetValueCommand, OnResetValueCommand));*/
        }

        public ANumericTextBox ()
        {
            Initialized += OnInitialized;
        }

        /// <summary>
        /// Gets or sets the current value of the <see cref="NumericTextBox"/>.
        /// </summary>
        public double? Value { get { return (double?)GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }

        /// <summary>
        /// Gets or sets the number of decimal places displayed in the <see cref="NumericTextBox"/>.
        /// </summary>
        public int DecimalPlaces { get { return (int)GetValue(DecimalPlacesProperty); } set { SetValue(DecimalPlacesProperty, value); } }

        /// <summary>
        /// Gets or sets the minimum value that can be set on the <see cref="Value"/> property.
        /// </summary>
        public double Minimum { get { return (double)GetValue(MinimumProperty); } set { SetValue(MinimumProperty, value); } }

        /// <summary>
        /// Gets or sets the maximum value that can be set on the <see cref="Value"/> property.
        /// </summary>
        public double Maximum { get { return (double)GetValue(MaximumProperty); } set { SetValue(MaximumProperty, value); } }

        /// <summary>
        /// Gets or sets the ratio of the <see cref="NumericTextBox.Value"/> between <see cref="NumericTextBox.Minimum"/> (0.0) and
        /// <see cref="NumericTextBox.Maximum"/> (1.0).
        /// </summary>
        public double ValueRatio { get { return (double)GetValue(ValueRatioProperty); } set { SetValue(ValueRatioProperty, value); } }

        /// <summary>
        /// Gets or sets the value to be added to or substracted from the <see cref="NumericTextBox.Value"/>.
        /// </summary>
        public double LargeChange { get { return (double)GetValue(LargeChangeProperty); } set { SetValue(LargeChangeProperty, value); } }

        /// <summary>
        /// Gets or sets the value to be added to or substracted from the <see cref="NumericTextBox.Value"/>.
        /// </summary>
        public double SmallChange { get { return (double)GetValue(SmallChangeProperty); } set { SetValue(SmallChangeProperty, value); } }

        /// <summary>
        /// Gets or sets whether to display Up and Down buttons on the side of the <see cref="NumericTextBox"/>.
        /// </summary>
        public bool DisplayUpDownButtons { get { return (bool)GetValue(DisplayUpDownButtonsProperty); } set { SetValue(DisplayUpDownButtonsProperty, value.Box()); } }

        /// <summary>
        /// Gets or sets whether dragging the value of the <see cref="NumericTextBox"/> is enabled.
        /// </summary>
        public bool AllowMouseDrag { get { return (bool)GetValue(AllowMouseDragProperty); } set { SetValue(AllowMouseDragProperty, value.Box()); } }

        /// <summary>
        /// Gets or sets when the <see cref="NumericTextBox"/> should be validated when the user uses the mouse to change its value.
        /// </summary>
        public AMouseValidationTrigger MouseValidationTrigger { get { return (AMouseValidationTrigger)GetValue(MouseValidationTriggerProperty); } set { SetValue(MouseValidationTriggerProperty, value); } }

        /// <summary>
        /// Raised when the <see cref="Value"/> property has changed.
        /// </summary>
 //       public event RoutedEventArgs ValueChanged { add { AddHandler(ValueChangedEvent, value); } remove { RemoveHandler(ValueChangedEvent, value); } }

        /// <summary>
        /// Raised when one of the repeat button is pressed.
        /// </summary>
        public event EventHandler<RepeatButtonPressedRoutedEventArgs> RepeatButtonPressed { add { AddHandler(RepeatButtonPressedEvent, value); } remove { RemoveHandler(RepeatButtonPressedEvent, value); } }

        /// <summary>
        /// Raised when one of the repeat button is released.
        /// </summary>
        public event EventHandler<RepeatButtonPressedRoutedEventArgs> RepeatButtonReleased { add { AddHandler(RepeatButtonReleasedEvent, value); } remove { RemoveHandler(RepeatButtonReleasedEvent, value); } }

        /// <inheritdoc/>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            increaseButton = e.NameScope.Find<RepeatButton>("PART_IncreaseButton");
            if (increaseButton == null)
                throw new InvalidOperationException("A part named 'PART_IncreaseButton' must be present in the ControlTemplate, and must be of type 'RepeatButton'.");

            decreaseButton = e.NameScope.Find<RepeatButton>("PART_DecreaseButton");
            if (decreaseButton == null)
                throw new InvalidOperationException("A part named 'PART_DecreaseButton' must be present in the ControlTemplate, and must be of type 'RepeatButton'.");

            contentHost = e.NameScope.Find<ScrollViewer>("PART_ContentHost");
            if (contentHost == null)
                throw new InvalidOperationException("A part named 'PART_ContentHost' must be present in the ControlTemplate, and must be of type 'ScrollViewer'.");

 //           var increasePressedWatcher = new DependencyPropertyWatcher(increaseButton);
//            increasePressedWatcher.RegisterValueChangedHandler(ButtonBase.IsPressedProperty, RepeatButtonIsPressedChanged);
 //           var decreasePressedWatcher = new DependencyPropertyWatcher(decreaseButton);
//            decreasePressedWatcher.RegisterValueChangedHandler(ButtonBase.IsPressedProperty, RepeatButtonIsPressedChanged);
            var textValue = FormatValue(Value);

            SetCurrentValue(TextProperty, textValue);
        }

        /// <summary>
        /// Raised when the <see cref="Value"/> property has changed.
        /// </summary>
        protected virtual void OnValueChanged(double? oldValue, double? newValue)
        {
        }

        /// <inheritdoc/>
        protected void OnInitialized(object sender, System.EventArgs e)
        {
            //base.OnInitialized(e);
            var textValue = FormatValue(Value);
            SetCurrentValue(TextProperty, textValue);
        }

        /// <inheritdoc/>
        protected sealed override void OnCancelled()
        {
//            var expression = GetBindingExpression(ValueProperty);
//            expression?.UpdateTarget();

            var textValue = FormatValue(Value);
            SetCurrentValue(TextProperty, textValue);
        }

        /// <inheritdoc/>
        protected sealed override void OnValidated()
        {
            double? value;
            if (TryParseValue(Text, out var parsedValue))
            {
                value = parsedValue;
            }
            else
            {
                value = Value;
            }
            SetCurrentValue(ValueProperty, value);

//            var expression = GetBindingExpression(ValueProperty);
//            expression?.UpdateSource();
        }

        protected override bool IsTextCompatibleWithValueBinding(string text)
        {
            return TryParseValue(text, out _);
        }

        /// <inheritdoc/>
        [NotNull]
        protected override string CoerceTextForValidation(string baseValue)
        {
            baseValue = base.CoerceTextForValidation(baseValue);
            double? value;
            if (TryParseValue(baseValue, out var parsedValue))
            {
                value = parsedValue;

                if (value > Maximum)
                {
                    value = Maximum;
                }
                if (value < Minimum)
                {
                    value = Minimum;
                }
            }
            else
            {
                value = Value;
            }

            return FormatValue(value);
        }

        [NotNull]
        protected string FormatValue(double? value)
        {
            if (!value.HasValue)
                return string.Empty;

            var decimalPlaces = DecimalPlaces;
            var coercedValue = decimalPlaces < 0 ? value.Value : Math.Round(value.Value, decimalPlaces);
            return coercedValue.ToString(CultureInfo.InvariantCulture);
        }

        private void RepeatButtonIsPressedChanged(object sender, EventArgs e)
        {
            var repeatButton = (RepeatButton)sender;
            if (ReferenceEquals(repeatButton, increaseButton))
            {
                RaiseEvent(new ARepeatButtonPressedRoutedEventArgs(RepeatButtons.IncreaseButton, repeatButton.IsPressed ? RepeatButtonPressedEvent : RepeatButtonReleasedEvent));
            }
            if (ReferenceEquals(repeatButton, decreaseButton))
            {
                RaiseEvent(new ARepeatButtonPressedRoutedEventArgs(RepeatButtons.DecreaseButton, repeatButton.IsPressed ? RepeatButtonPressedEvent : RepeatButtonReleasedEvent));
            }
        }

        private void OnValuePropertyChanged(double? oldValue, double? newValue)
        {
            if (newValue.HasValue && newValue.Value > Maximum)
            {
                SetCurrentValue(ValueProperty, Maximum);
                return;
            }
            if (newValue.HasValue && newValue.Value < Minimum)
            {
                SetCurrentValue(ValueProperty, Minimum);
                return;
            }

            var textValue = FormatValue(newValue);
            updatingValue = true;
            SetCurrentValue(TextProperty, textValue);
            SetCurrentValue(ValueRatioProperty, newValue.HasValue ? MathUtil.InverseLerp(Minimum, Maximum, newValue.Value) : 0.0);
            updatingValue = false;

//            RaiseEvent(new AvaloniaPropertyChangedEventArgs<double?>(oldValue, newValue, ValueChangedEvent));
            OnValueChanged(oldValue, newValue);
        }

        private void UpdateValue(double value)
        {
            if (IsReadOnly == false)
            {
                SetCurrentValue(ValueProperty, value);
            }
        }

        /// <summary>
        /// tries to parse the value of the textbox into a double, accommodating various cultural settings and preferences
        /// </summary>
        /// <param name="value">text value of the textbox</param>
        /// <param name="result">the resulting numeric value if parsing is successful</param>
        /// <returns>whether parsing the value was successful</returns>
        private static bool TryParseValue(ReadOnlySpan<char> value, out double result)
        {
            //thousands are disallowed as they might lead to decimal seperators falsely being interpreted as thousands
            const NumberStyles numberStyle = NumberStyles.Any & ~NumberStyles.AllowThousands;

            //try parsing a hex string
            var span = value.TrimStart('0');
            if (span.StartsWith("x", StringComparison.OrdinalIgnoreCase) || span.StartsWith("#", StringComparison.OrdinalIgnoreCase))
            {
                var span2 = span.TrimStart(stackalloc[] {'x', '#'});
                if (double.TryParse(span2, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out result))
                    return true;
            }
            //Try parsing in the current culture
            else if (double.TryParse(value, numberStyle, CultureInfo.CurrentCulture, out result) ||
                //or in neutral culture
                double.TryParse(value, numberStyle, CultureInfo.InvariantCulture, out result) ||
                //or as fallback a culture that has ',' as comma separator
                double.TryParse(value, numberStyle, CultureInfo.GetCultureInfo("de-DE"), out result))
            {
                return true;
            }

            return false;
        }

        private static void OnValuePropertyChanged(AvaloniaObject sender, AvaloniaPropertyChangedEventArgs e)
        {
            ((ANumericTextBox)sender).OnValuePropertyChanged((double?)e.OldValue, (double?)e.NewValue);
        }

        private static void OnDecimalPlacesPropertyChanged(AvaloniaObject sender, AvaloniaPropertyChangedEventArgs e)
        {
            var numericInput = (ANumericTextBox)sender;
            numericInput.CoerceValue(ValueProperty);
        }

        private static void OnMinimumPropertyChanged(AvaloniaObject sender, AvaloniaPropertyChangedEventArgs e)
        {
            var numericInput = (ANumericTextBox)sender;
            var needValidation = false;
            if (numericInput.Maximum < numericInput.Minimum)
            {
                numericInput.SetCurrentValue(MaximumProperty, numericInput.Minimum);
                needValidation = true;
            }
            if (numericInput.Value < numericInput.Minimum)
            {
                numericInput.SetCurrentValue(ValueProperty, numericInput.Minimum);
                needValidation = true;
            }

            // Do not overwrite the Value, it is already correct!
            numericInput.updatingValue = true;
            numericInput.SetCurrentValue(ValueRatioProperty, numericInput.Value.HasValue ? MathUtil.InverseLerp(numericInput.Minimum, numericInput.Maximum, numericInput.Value.Value) : 0.0);
            numericInput.updatingValue = false;

            if (needValidation)
            {
                numericInput.Validate();
            }
        }

        private static void OnMaximumPropertyChanged(AvaloniaObject sender, AvaloniaPropertyChangedEventArgs e)
        {
            var numericInput = (ANumericTextBox)sender;
            var needValidation = false;
            if (numericInput.Minimum > numericInput.Maximum)
            {
                numericInput.SetCurrentValue(MinimumProperty, numericInput.Maximum);
                needValidation = true;
            }
            if (numericInput.Value > numericInput.Maximum)
            {
                numericInput.SetCurrentValue(ValueProperty, numericInput.Maximum);
                needValidation = true;
            }

            // Do not overwrite the Value, it is already correct!
            numericInput.updatingValue = true;
            numericInput.SetCurrentValue(ValueRatioProperty, numericInput.Value.HasValue ? MathUtil.InverseLerp(numericInput.Minimum, numericInput.Maximum, numericInput.Value.Value) : 0.0);
            numericInput.updatingValue = false;

            if (needValidation)
            {
                numericInput.Validate();
            }
        }

        private static void ValueRatioChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var control = (ANumericTextBox)d;
            if (control != null && !control.updatingValue)
                control.UpdateValue(MathUtil.Lerp(control.Minimum, control.Maximum, (double)e.NewValue));
        }

        private static void UpdateValueCommand([NotNull] object sender, Func<ANumericTextBox, double> getValue, bool validate = true)
        {
            var control = sender as ANumericTextBox ?? Avalonia.VisualTree.VisualExtensions.GetVisualParent<ANumericTextBox> ((Avalonia.Controls.TextBox)sender);
            if (control != null)
            {
                var value = getValue(control);
                control.UpdateValue(value);
                control.SelectAll();
                if (validate)
                    control.Validate();
            }
        }

        private static void OnLargeIncreaseCommand([NotNull] object sender, ExecutedRoutedEventArgs e)
        {
            UpdateValueCommand(sender, x => (x.Value ?? x.Minimum) + x.LargeChange);
        }

        private static void OnLargeDecreaseCommand([NotNull] object sender, ExecutedRoutedEventArgs e)
        {
            UpdateValueCommand(sender, x => (x.Value ?? x.Maximum) - x.LargeChange);
        }

        private static void OnSmallIncreaseCommand([NotNull] object sender, ExecutedRoutedEventArgs e)
        {
            UpdateValueCommand(sender, x => (x.Value ?? x.Minimum) + x.SmallChange);
        }

        private static void OnSmallDecreaseCommand([NotNull] object sender, ExecutedRoutedEventArgs e)
        {
            UpdateValueCommand(sender, x => (x.Value ?? x.Maximum) - x.SmallChange);
        }

        private static void OnResetValueCommand([NotNull] object sender, ExecutedRoutedEventArgs e)
        {
            UpdateValueCommand(sender, x => 0.0, false);
        }

        private static void OnForbiddenPropertyChanged([NotNull] AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var metadata = e.Property.GetMetadata(d.GetType ());
 /*           if (!Equals(e.NewValue, metadata.DefaultValue))
            {
                var message = $"The value of the property '{e.Property.Name}' cannot be different from the value '{metadata.DefaultValue}'";
                throw new InvalidOperationException(message);
            }*/
        }
    }
}
