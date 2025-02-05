// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
//using System.Windows.Controls;
using Avalonia;
//using System.Windows.Data;
//using System.Windows.Media;
using Stride.Core.Mathematics;

//using System.Windows.Media.Imaging;
//using System.Windows.Input;
using Stride.Core.Annotations;
using Stride.Core.Presentation.Extensions;
using Stride.Core.Presentation.Internal;
using Color = Stride.Core.Mathematics.Color;
using Avalonia.Controls.Metadata;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Controls.Primitives;
using Point = Avalonia.Point;
using Rectangle = Avalonia.Controls.Shapes.Rectangle;
using Avalonia.Media;
using Avalonia.Platform;
using SkiaSharp;
using System.Runtime.InteropServices;
using Avalonia.Input;

namespace Stride.Core.Presentation.Controls
{
    /// <summary>
    /// Represents a color picker control.
    /// </summary>
    [TemplatePart(Name = "PART_ColorPickerSelector", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_ColorPickerRenderSurface", Type = typeof(Rectangle))]
    [TemplatePart(Name = "PART_ColorPreviewRenderSurface", Type = typeof(Rectangle))]
    [TemplatePart(Name = "PART_HuePickerSelector", Type = typeof(Control))]
    [TemplatePart(Name = "PART_HuePickerRenderSurface", Type = typeof(Rectangle))]
    public class AColorPicker : TemplatedControl
    {
        protected override Type StyleKeyOverride { get { return typeof(AColorPicker); } }

        static AColorPicker()
        {
            ColorProperty.Changed.AddClassHandler<AColorPicker>(OnColorPropertyChanged);
            HueProperty.Changed.AddClassHandler<AColorPicker>(OnHSVPropertyChanged);
            SaturationProperty.Changed.AddClassHandler<AColorPicker>(OnHSVPropertyChanged);
            BrightnessProperty.Changed.AddClassHandler<AColorPicker>(OnHSVPropertyChanged);
            RedProperty.Changed.AddClassHandler<AColorPicker>(OnRGBAPropertyChanged);
            GreenProperty.Changed.AddClassHandler<AColorPicker>(OnRGBAPropertyChanged);
            BlueProperty.Changed.AddClassHandler<AColorPicker>(OnRGBAPropertyChanged);
            AlphaProperty.Changed.AddClassHandler<AColorPicker>(OnRGBAPropertyChanged);

            //            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }

        public AColorPicker ()
        {
            var a = 1;
        }

        private Canvas colorPickerSelector;
        private Rectangle colorPickerRenderSurface;
        private Rectangle colorPreviewRenderSurface;
        private Control huePickerRenderSurface;
        private Rectangle huePickerSelector;
        private bool interlock;
        private ColorHSV internalColor;
        private bool suspendBindingUpdates;
        private bool templateApplied;
        private AvaloniaProperty initializingProperty;

        /// <summary>
        /// Identifies the <see cref="Color"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<Color4> ColorProperty = StyledProperty<Color4>.Register<AColorPicker, Color4>("Color", default(Color4), defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <see cref="Hue"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<float> HueProperty = StyledProperty<float>.Register<AColorPicker, float>("Hue", 0.0f, defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <see cref="Saturation"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<float> SaturationProperty = StyledProperty<float>.Register<AColorPicker, float>("Saturation", 0.0f, defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <see cref="Brightness"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<float> BrightnessProperty = StyledProperty<float>.Register<AColorPicker, float>("Brightness", 0.0f, defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <see cref="Red"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<byte> RedProperty = StyledProperty<byte>.Register<AColorPicker, byte>("Red", (byte)0, defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <see cref="Green"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<byte> GreenProperty = StyledProperty<byte>.Register<AColorPicker, byte>("Green", (byte)0, defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <see cref="Blue"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<byte> BlueProperty = StyledProperty<byte>.Register<AColorPicker, byte>("Blue", (byte)0, defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <see cref="Alpha"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<byte> AlphaProperty = StyledProperty<byte>.Register<AColorPicker, byte>("Alpha", (byte)0, defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <see cref="ShowAlpha"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> ShowAlphaProperty = StyledProperty<bool>.Register<AColorPicker, bool>("ShowAlpha", true, defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Identifies the <see cref="InputColumnWidth"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<GridLength> InputColumnWidthProperty = StyledProperty<GridLength>.Register<AColorPicker, GridLength>("InputColumnWidth", GridLength.Auto);

        /// <summary>
        /// Identifies the <see cref="PickupAreaSize"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<Size> PickupAreaSizeProperty = StyledProperty<Size>.Register<AColorPicker, Size>("PickupAreaSize", default(Size));

        /// <summary>
        /// Identifies the <see cref="StripsHeight"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> StripsHeightProperty = StyledProperty<double>.Register<AColorPicker, double>("StripsHeight", (double)0);
            
        /// <summary>
        /// Gets or sets the color associated to this color picker.
        /// </summary>
        /// <remarks>The float values of each component of the color are always equals to the float conversion of a <see cref="byte"/> value divided by <b>255</b>.</remarks>
        public Color4 Color { get { return (Color4)GetValue(ColorProperty); } set { SetValue(ColorProperty, value); } }

        /// <summary>
        /// Gets or sets the hue of the color associated to this color picker.
        /// </summary>
        public float Hue { get { return (float)GetValue(HueProperty); } set { SetValue(HueProperty, value); } }

        /// <summary>
        /// Gets or sets the saturation of the color associated to this color picker.
        /// </summary>
        public float Saturation { get { return (float)GetValue(SaturationProperty); } set { SetValue(SaturationProperty, value); } }

        /// <summary>
        /// Gets or sets the brightness of the color associated to this color picker.
        /// </summary>
        public float Brightness { get { return (float)GetValue(BrightnessProperty); } set { SetValue(BrightnessProperty, value); } }

        /// <summary>
        /// Gets or sets the red component of the color associated to this color picker.
        /// </summary>
        public byte Red { get { return (byte)GetValue(RedProperty); } set { SetValue(RedProperty, value); } }

        /// <summary>
        /// Gets or sets the green component of the color associated to this color picker.
        /// </summary>
        public byte Green { get { return (byte)GetValue(GreenProperty); } set { SetValue(GreenProperty, value); } }

        /// <summary>
        /// Gets or sets the blue component of the color associated to this color picker.
        /// </summary>
        public byte Blue { get { return (byte)GetValue(BlueProperty); } set { SetValue(BlueProperty, value); } }

        /// <summary>
        /// Gets or sets the alpha component of the color associated to this color picker.
        /// </summary>
        public byte Alpha { get { return (byte)GetValue(AlphaProperty); } set { SetValue(AlphaProperty, value); } }

        /// <summary>
        /// Gets or sets whether the alpha component of the color should be displayed in the color picker.
        /// </summary>
        public bool ShowAlpha { get { return (bool)GetValue(ShowAlphaProperty); } set { SetValue(ShowAlphaProperty, value.Box()); } }

        /// <summary>
        /// Gets or sets the length of the input column of the color picker.
        /// </summary>
        public GridLength InputColumnWidth { get { return (GridLength)GetValue(InputColumnWidthProperty); } set { SetValue(InputColumnWidthProperty, value); } }

        /// <summary>
        /// Gets or sets the size of the color pickup area.
        /// </summary>
        public Size PickupAreaSize { get { return (Size)GetValue(PickupAreaSizeProperty); } set { SetValue(PickupAreaSizeProperty, value); } }

        /// <summary>
        /// Gets or sets the height of the hue pickup strip and the preview color strip.
        /// </summary>
        public double StripsHeight { get { return (double)GetValue(StripsHeightProperty); } set { SetValue(StripsHeightProperty, value); } }

        /// <summary>
        /// An internal representation of the color associated to this color picker. Its value never rounded to match a byte division by 255.
        /// </summary>
        private ColorHSV InternalColor { get { return internalColor; } set { internalColor = value; var prev = interlock; interlock = true; Color = value.ToColor(); interlock = prev; } }

        /// <inheritdoc/>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            templateApplied = false;
            base.OnApplyTemplate(e);

            if (colorPickerRenderSurface != null)
            {
                colorPickerRenderSurface.PointerPressed -= OnColorPickerRenderSurfaceMouseDown;
                colorPickerRenderSurface.PointerReleased -= OnColorPickerRenderSurfaceMouseUp;
                colorPickerRenderSurface.PointerMoved -= OnColorPickerRenderSurfaceMouseMove;
                //                colorPickerRenderSurface.MouseDown -= OnColorPickerRenderSurfaceMouseDown;
                //                colorPickerRenderSurface.MouseUp -= OnColorPickerRenderSurfaceMouseUp;
                //                colorPickerRenderSurface.MouseMove -= OnColorPickerRenderSurfaceMouseMove;
            }

            if (huePickerRenderSurface != null)
            {
                huePickerRenderSurface.PointerPressed-= OnHuePickerRenderSurfaceMouseDown;
                huePickerRenderSurface.PointerReleased -= OnHuePickerRenderSurfaceMouseUp;
                huePickerRenderSurface.PointerMoved -= OnHuePickerRenderSurfaceMouseMove;

            }

            colorPickerRenderSurface = ADependencyObjectExtensions.CheckTemplatePart<Rectangle>(e.NameScope.Find<Rectangle>("PART_ColorPickerRenderSurface"));
            colorPreviewRenderSurface = ADependencyObjectExtensions.CheckTemplatePart<Rectangle>(e.NameScope.Find<Rectangle>("PART_ColorPreviewRenderSurface"));
            colorPickerSelector = ADependencyObjectExtensions.CheckTemplatePart<Canvas>(e.NameScope.Find<Canvas>("PART_ColorPickerSelector"));
            huePickerSelector = ADependencyObjectExtensions.CheckTemplatePart<Rectangle>(e.NameScope.Find<Rectangle>("PART_HuePickerSelector"));
            huePickerRenderSurface = ADependencyObjectExtensions.CheckTemplatePart<Control>(e.NameScope.Find<Control>("PART_HuePickerRenderSurface"));

            if (colorPickerRenderSurface != null)
            {
                colorPickerRenderSurface.PointerPressed += OnColorPickerRenderSurfaceMouseDown;
                colorPickerRenderSurface.PointerReleased += OnColorPickerRenderSurfaceMouseUp;
                colorPickerRenderSurface.PointerMoved += OnColorPickerRenderSurfaceMouseMove;
            }

            if (huePickerRenderSurface != null)
            {
                huePickerRenderSurface.PointerPressed += OnHuePickerRenderSurfaceMouseDown;
                huePickerRenderSurface.PointerReleased += OnHuePickerRenderSurfaceMouseUp;
                huePickerRenderSurface.PointerMoved += OnHuePickerRenderSurfaceMouseMove;
            }

            RenderColorPickerSurface();

            if (colorPickerSelector != null && colorPickerRenderSurface != null)
            {
                Canvas.SetLeft(colorPickerSelector, Saturation * colorPickerRenderSurface.Width / 100.0);
                Canvas.SetTop(colorPickerSelector, Brightness * colorPickerRenderSurface.Height / 100.0);
            }
            if (huePickerSelector != null && huePickerRenderSurface != null)
            {
                Canvas.SetLeft(huePickerSelector, Hue * huePickerRenderSurface.Width / 360.0);
            }
            templateApplied = true;
        }

        /// <summary>
        /// Handles the <see cref="Rectangle.MouseDown"/> event of the color surface.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void OnColorPickerRenderSurfaceMouseDown(object? sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint((Control)sender).Properties.IsLeftButtonPressed)
            {
                       // colorPickerRenderSurface.CaptureMouse();
                        suspendBindingUpdates = true;
                        UpdateColorPickerFromMouse(e.GetPosition(colorPickerRenderSurface));
                    }
                }

        /// <summary>
        /// Handles the <see cref="Rectangle.MouseUp"/> event of the color surface.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
               private void OnColorPickerRenderSurfaceMouseUp(object? sender, PointerReleasedEventArgs e)
        {
            if (!e.GetCurrentPoint((Control)sender).Properties.IsLeftButtonPressed)
            {
                       //colorPickerRenderSurface.ReleaseMouseCapture();
                       suspendBindingUpdates = false;
                       UpdateAllBindings();
                   }
               }

        /// <summary>
        /// Handles the <see cref="Rectangle.MouseMove"/> event of the color surface.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
                private void OnColorPickerRenderSurfaceMouseMove(object? sender, PointerEventArgs e)
        {
            if (e.GetCurrentPoint((Control)sender).Properties.IsLeftButtonPressed && colorPickerRenderSurface.IsPointerOver)
//                if (e.LeftButton == MouseButtonState.Pressed && colorPickerRenderSurface.IsMouseCaptured)
                    {
                        UpdateColorPickerFromMouse(e.GetPosition(colorPickerRenderSurface));
                    }
                }

        /// <summary>
        /// Updates the <see cref="Color"/> value from the given position in the color surface.
        /// </summary>
        /// <param name="position">The position of the cursor in the color surface.</param>
        private void UpdateColorPickerFromMouse(Point position)
        {
            Canvas.SetLeft(colorPickerSelector, MathUtil.Clamp(position.X, 0.0f, colorPickerRenderSurface.Width));
            Canvas.SetTop(colorPickerSelector, MathUtil.Clamp(position.Y, 0.0f, colorPickerRenderSurface.Height));
            var x = (float)(position.X / colorPickerRenderSurface.Width);
            var y = (float)(position.Y / colorPickerRenderSurface.Height);
            x = MathUtil.Clamp(x, 0.0f, 1.0f);
            y = MathUtil.Clamp(y, 0.0f, 1.0f);            
            var colorHSV = new ColorHSV(Hue, x, y, Alpha / 255.0f);
            //SetCurrentValue(ColorProperty, colorHSV.ToColor());
            SetCurrentValue(SaturationProperty, colorHSV.S * 100.0f);
            SetCurrentValue(BrightnessProperty, colorHSV.V * 100.0f);
        }


        /// <summary>
        /// Handles the <see cref="Rectangle.MouseDown"/> event of the hue surface.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void OnHuePickerRenderSurfaceMouseDown(object? sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint((Control) sender).Properties.IsLeftButtonPressed)
            {
//                huePickerRenderSurface.CaptureMouse();
                suspendBindingUpdates = true;
                UpdateHuePickerFromMouse(e.GetPosition(huePickerRenderSurface));
            }
        }

        /// <summary>
        /// Handles the <see cref="Rectangle.MouseUp"/> event of the hue surface.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void OnHuePickerRenderSurfaceMouseUp(object? sender, PointerReleasedEventArgs e)
        {
            if (!e.GetCurrentPoint((Control)sender).Properties.IsLeftButtonPressed)
            {
//                huePickerRenderSurface.ReleaseMouseCapture();
                suspendBindingUpdates = false;
                UpdateAllBindings();
            }
        }

        /// <summary>
        /// Handles the <see cref="Rectangle.MouseMove"/> event of the hue surface.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void OnHuePickerRenderSurfaceMouseMove(object? sender, PointerEventArgs e)
        {
            if (e.GetCurrentPoint((Control)sender).Properties.IsLeftButtonPressed && huePickerRenderSurface.IsPointerOver)
//                if (e.LeftButton == MouseButtonState.Pressed && huePickerRenderSurface.IsMouseCaptured)
            {
                UpdateHuePickerFromMouse(e.GetPosition(huePickerRenderSurface));
            }
        }

        /// <summary>
        /// Updates the <see cref="Color"/> value from the given position in the hue surface.
        /// </summary>
        /// <param name="position">The position of the cursor in the hue surface.</param>
        private void UpdateHuePickerFromMouse(Point position)
        {
            Canvas.SetLeft(huePickerSelector, MathUtil.Clamp(position.X, 0.0f, huePickerRenderSurface.Width));
            var x = (float)(position.X / huePickerRenderSurface.Width);
            x = (float)(360.0 * MathUtil.Clamp(x, 0.0f, 1.0f));
            //var colorHSV = new ColorHSV(x, Saturation / 100.0f, Brightness / 100.0f, Alpha / 255.0f);
            //SetCurrentValue(ColorProperty, colorHSV.ToColor());
            SetCurrentValue(HueProperty, x);
        }

        /// <summary>
        /// Update the color surface brush according to the current <see cref="Hue"/> value.
        /// </summary>
        private void RenderColorPickerSurface()
        {
            if (colorPreviewRenderSurface != null)
            {
                colorPreviewRenderSurface.Fill = new SolidColorBrush(Color.AToSystemColor());
            }
            if (colorPickerRenderSurface != null)
            {
                // Ensure the color picker is loaded 
                if (!double.IsNaN(colorPickerRenderSurface.Width) && !double.IsNaN(colorPickerRenderSurface.Height))
                {
                    var width = (int)colorPickerRenderSurface.Width;
                    var height = (int)colorPickerRenderSurface.Height;

                    PixelFormat pf = PixelFormats.Bgra8888; //  .Bgr32;
                    int rawStride = (width * pf.BitsPerPixel + 7) / 8;
                    var rawImage = new byte[rawStride * height];

                    for (int j = 0; j < height; ++j)
                    {
                        float y = j / (float)(height - 1);

                        for (int i = 0; i < width; ++i)
                        {
                            float x = i / (float)(width - 1);

                            var color4 = new ColorHSV(Hue, x, y, 1.0f).ToColor();
                            var color = new Color(color4);
                            rawImage[(i + j * width) * 4 + 0] = color.B;
                            rawImage[(i + j * width) * 4 + 1] = color.G;
                            rawImage[(i + j * width) * 4 + 2] = color.R;
                        }
                    }

                    int size = Marshal.SizeOf(rawImage[0]) * rawImage.Length;
                    IntPtr pnt = Marshal.AllocHGlobal(size);
                    Marshal.Copy(rawImage, 0, pnt, rawImage.Length);
                    colorPickerRenderSurface.Fill = new ImageBrush(new Avalonia.Media.Imaging.Bitmap(pf, AlphaFormat.Premul, pnt, new PixelSize (width, height), new Vector (96, 96), rawStride));
                    Marshal.FreeHGlobal(pnt);
                    //                    colorPickerRenderSurface.Fill = new DrawingBrush(new ImageDrawing(BitmapSource.Create(width, height, 96, 96, pf, null, rawImage, rawStride), new Rect(0.0f, 0.0f, width, height)));
                }
            }
        }

        /// <summary>
        /// Raised when the <see cref="Color"/> property is modified.
        /// </summary>
        private void OnColorChanged()
        {
            bool isInitializing = !templateApplied && initializingProperty == null;
            if (isInitializing)
                initializingProperty = ColorProperty;

            if (!interlock)
            {
                InternalColor = ColorHSV.FromColor(Color);
                var colorRGBA = InternalColor.ToColor();
                interlock = true;

                SetCurrentValue(RedProperty, (byte)(Math.Round(colorRGBA.R * 255.0f)));
                SetCurrentValue(GreenProperty, (byte)(Math.Round(colorRGBA.G * 255.0f)));
                SetCurrentValue(BlueProperty, (byte)(Math.Round(colorRGBA.B * 255.0f)));
                SetCurrentValue(AlphaProperty, (byte)(Math.Round(colorRGBA.A * 255.0f)));

                SetCurrentValue(HueProperty, InternalColor.H);
                SetCurrentValue(SaturationProperty, InternalColor.S * 100.0f);
                SetCurrentValue(BrightnessProperty, InternalColor.V * 100.0f);
                interlock = false;
            }

            if (!suspendBindingUpdates)
            {
                RenderColorPickerSurface();
            }
            else if (colorPreviewRenderSurface != null)
            {
                colorPreviewRenderSurface.Fill = new SolidColorBrush(Color.AToSystemColor());
            }
            UpdateBinding(ColorProperty);

            if (isInitializing)
                initializingProperty = null;
        }

        /// <summary>
        /// Raised when the <see cref="Red"/>, <see cref="Green"/>, <see cref="Blue"/> or <see cref="Alpha"/> properties are modified.
        /// </summary>
        /// <param name="e">The dependency property that has changed.</param>
        private void OnRGBAValueChanged(AvaloniaPropertyChangedEventArgs e)
        {
            bool isInitializing = !templateApplied && initializingProperty == null;
            if (isInitializing)
                initializingProperty = e.Property;

            if (!interlock)
            {
                Color4 colorRGBA;
                if (e.Property == RedProperty)
                    colorRGBA = new Color4((byte)e.NewValue / 255.0f, Green / 255.0f, Blue / 255.0f, Alpha / 255.0f);
                else if (e.Property == GreenProperty)
                    colorRGBA = new Color4(Red / 255.0f, (byte)e.NewValue / 255.0f, Blue / 255.0f, Alpha / 255.0f);
                else if (e.Property == BlueProperty)
                    colorRGBA = new Color4(Red / 255.0f, Green / 255.0f, (byte)e.NewValue / 255.0f, Alpha / 255.0f);
                else if (e.Property == AlphaProperty)
                    colorRGBA = new Color4(Red / 255.0f, Green / 255.0f, Blue / 255.0f, (byte)e.NewValue / 255.0f);
                else
                    throw new ArgumentException("Property unsupported by method OnRGBAValueChanged.");

                interlock = true;
                InternalColor = ColorHSV.FromColor(colorRGBA);
                SetCurrentValue(HueProperty, InternalColor.H);
                SetCurrentValue(SaturationProperty, InternalColor.S * 100.0f);
                SetCurrentValue(BrightnessProperty, InternalColor.V * 100.0f);
                interlock = false;
            }
            
            UpdateBinding(e.Property);
            if (isInitializing)
                initializingProperty = null;
        }

        /// <summary>
        /// Raised when the <see cref="Hue"/>, <see cref="Saturation"/>, or <see cref="Brightness"/> properties are modified.
        /// </summary>
        /// <param name="e">The dependency property that has changed.</param>
        private void OnHSVValueChanged(AvaloniaPropertyChangedEventArgs e)
        {
            bool isInitializing = !templateApplied && initializingProperty == null;
            if (isInitializing)
                initializingProperty = e.Property;
            
            if (!interlock)
            {
                if (e.Property == HueProperty)
                {
                    InternalColor = new ColorHSV((float)e.NewValue, Saturation / 100.0f, Brightness / 100.0f, Alpha / 255.0f);
                    RenderColorPickerSurface();
                }
                else if (e.Property == SaturationProperty)
                    InternalColor = new ColorHSV(Hue, (float)e.NewValue / 100.0f, Brightness / 100.0f, Alpha / 255.0f);
                else if (e.Property == BrightnessProperty)
                    InternalColor = new ColorHSV(Hue, Saturation / 100.0f, (float)e.NewValue / 100.0f, Alpha / 255.0f);
                else
                    throw new ArgumentException("Property unsupported by method OnHSVValueChanged.");

                var colorRGBA = InternalColor.ToColor();
                interlock = true;
                SetCurrentValue(RedProperty, (byte)(Math.Round(colorRGBA.R * 255.0f)));
                SetCurrentValue(GreenProperty, (byte)(Math.Round(colorRGBA.G * 255.0f)));
                SetCurrentValue(BlueProperty, (byte)(Math.Round(colorRGBA.B * 255.0f)));
                SetCurrentValue(AlphaProperty, (byte)(Math.Round(colorRGBA.A * 255.0f)));
                interlock = false;
            }

            UpdateBinding(e.Property);

            if (colorPickerSelector != null && colorPickerRenderSurface != null && !suspendBindingUpdates)
            {
                Canvas.SetLeft(colorPickerSelector, Saturation * colorPickerRenderSurface.Width / 100.0);
                Canvas.SetTop(colorPickerSelector, Brightness * colorPickerRenderSurface.Height / 100.0);
            }
            if (huePickerSelector != null && huePickerRenderSurface != null && !suspendBindingUpdates)
            {
                Canvas.SetLeft(huePickerSelector, Hue * huePickerRenderSurface.Width / 360.0);
            }

            if (isInitializing)
                initializingProperty = null;
        }

        /// <summary>
        /// Updates the binding of the given dependency property, if binding updates are not currently suspended by user actions.
        /// </summary>
        /// <param name="dependencyProperty">The dependency property.</param>
        private void UpdateBinding(AvaloniaProperty dependencyProperty)
        {
            if (!suspendBindingUpdates && dependencyProperty != initializingProperty)
            {
 //               var expression = GetBindingExpression(dependencyProperty);
//                expression?.UpdateSource();
            }
        }

        /// <summary>
        /// Updates the bindings of all dependency properties, if binding updates are not currently suspended by user actions.
        /// </summary>
        private void UpdateAllBindings()
        {
            UpdateBinding(ColorProperty);
            UpdateBinding(RedProperty);
            UpdateBinding(GreenProperty);
            UpdateBinding(BlueProperty);
            UpdateBinding(AlphaProperty);
            UpdateBinding(HueProperty);
            UpdateBinding(SaturationProperty);
            UpdateBinding(BrightnessProperty);
        }

        /// <summary>
        /// Raised by <see cref="ColorProperty"/> when the <see cref="Color"/> dependency property is modified.
        /// </summary>
        /// <param name="sender">The dependency object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private static void OnColorPropertyChanged(AvaloniaObject sender, AvaloniaPropertyChangedEventArgs e)
        {
            var colorPicker = (AColorPicker)sender;
            colorPicker.OnColorChanged();
        }

        /// <summary>
        /// Raised by <see cref="HueProperty"/>, <see cref="SaturationProperty"/> or <see cref="BrightnessProperty"/> when respectively the
        /// <see cref="Hue"/>, <see cref="Saturation"/> or <see cref="Brightness"/> dependency property is modified.
        /// </summary>
        /// <param name="sender">The dependency object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private static void OnHSVPropertyChanged(AvaloniaObject sender, AvaloniaPropertyChangedEventArgs e)
        {
            var colorPicker = (AColorPicker)sender;
            colorPicker.OnHSVValueChanged(e);
        }

        /// <summary>
        /// Raised by <see cref="RedProperty"/>, <see cref="GreenProperty"/>, <see cref="BlueProperty"/> or <see cref="AlphaProperty"/> when respectively the
        /// <see cref="Red"/>, <see cref="Green"/>, <see cref="Blue"/> or <see cref="Alpha"/> dependency property is modified.
        /// </summary>
        /// <param name="sender">The dependency object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private static void OnRGBAPropertyChanged(AvaloniaObject sender, AvaloniaPropertyChangedEventArgs e)
        {
            var colorPicker = (AColorPicker)sender;
            colorPicker.OnRGBAValueChanged(e);
        }

        /// <summary>
        /// Coerce the value of the Color so its components are always equals to the float value of a byte divided by 255.
        /// </summary>
        [NotNull]
        private static object CoreceColorValue(AvaloniaObject sender, [NotNull] object baseValue)
        {
            return new Color(((Color4)baseValue).ToArray()).ToColor4();
        }

        /// <summary>
        /// Coerce the value of the Hue so it is always contained in the -360, +360 interval
        /// </summary>
        [NotNull]
        private static object CoerceHueValue(AvaloniaObject sender, [NotNull] object baseValue)
        {
            return ((float)baseValue + 360.0f) % 360.0f;
        }

        /// <summary>
        /// Coerce the saturation and brightness values so they are always contained between 0 and 100
        /// </summary>
        [NotNull]
        private static object CoercePercentageValue(AvaloniaObject sender, [NotNull] object baseValue)
        {
            return MathUtil.Clamp((float)baseValue, 0.0f, 100.0f);
        }
    }
}
