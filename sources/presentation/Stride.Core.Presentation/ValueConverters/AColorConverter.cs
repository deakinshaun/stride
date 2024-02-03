// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Globalization;
using Avalonia;
using Avalonia.Media;

//using System.Windows;
//using System.Windows.Media;

using Stride.Core.Mathematics;
using Stride.Core.Presentation.Extensions;

using Color = Stride.Core.Mathematics.Color;

namespace Stride.Core.Presentation.ValueConverters
{
    /// <summary>
    /// This converter will convert any known type of color value to the target type, if the conversion is possible. Otherwise, a <see cref="NotSupportedException"/> will be thrown.
    /// </summary>
    public class AColorConverter : AValueConverterBase<AColorConverter>
    {
        /// <inheritdoc/>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = value as SolidColorBrush;
            if (brush != null)
                value = brush.Color;
            
            if (value is Color)
            {
                var color = (Color)value;
                if (targetType == typeof(Avalonia.Media.Color))
                    return color.AToSystemColor();
                if (targetType == typeof(Color))
                    return color;
                if (targetType == typeof(Color3))
                    return color.ToColor3();
                if (targetType == typeof(Color4))
                    return color.ToColor4();
                if (targetType.IsAssignableFrom(typeof(SolidColorBrush)))
                    return new SolidColorBrush(color.AToSystemColor());
                if (targetType == typeof(string))
                    return ColorExtensions.RgbaToString(color.ToRgba());
            }
            if (value is Color3)
            {
                var color = (Color3)value;
                if (targetType == typeof(Avalonia.Media.Color))
                    return color.AToSystemColor();
                if (targetType == typeof(Color))
                    return (Color)color;
                if (targetType == typeof(Color3))
                    return color;
                if (targetType == typeof(Color4))
                    return color.ToColor4();
                if (targetType.IsAssignableFrom(typeof(SolidColorBrush)))
                    return new SolidColorBrush(color.AToSystemColor());
                if (targetType == typeof(string))
                    return ColorExtensions.RgbToString(color.ToRgb());
            }
            if (value is Color4)
            {
                var color = (Color4)value;
                if (targetType == typeof(Avalonia.Media.Color))
                    return color.AToSystemColor();
                if (targetType == typeof(Color))
                    return (Color)color;
                if (targetType == typeof(Color3))
                    return color.ToColor3();
                if (targetType == typeof(Color4))
                    return color;
                if (targetType.IsAssignableFrom(typeof(SolidColorBrush)))
                    return new SolidColorBrush(color.AToSystemColor());
                if (targetType == typeof(string))
                    return ColorExtensions.RgbaToString(color.ToRgba());
            }
            if (value is Avalonia.Media.Color)
            {
                var wpfColor = (Avalonia.Media.Color)value;
                if (targetType.IsAssignableFrom(typeof(SolidColorBrush)))
                    return new SolidColorBrush(wpfColor);

                var color = new Color(wpfColor.R, wpfColor.G, wpfColor.B, wpfColor.A);
                if (targetType == typeof(Avalonia.Media.Color))
                    return color;
                if (targetType == typeof(Color))
                    return color;
                if (targetType == typeof(Color3))
                    return color.ToColor3();
                if (targetType == typeof(Color4))
                    return color.ToColor4();
                if (targetType == typeof(string))
                    return ColorExtensions.RgbaToString(color.ToRgba());
            }
            var stringColor = value as string;
            if (stringColor != null)
            {
                var intValue = ColorExtensions.StringToRgba(stringColor);
                if (targetType == typeof(Color))
                    return Color.FromRgba(intValue);
                if (targetType == typeof(Color3))
                    return new Color3(intValue);
                if (targetType == typeof(Color4))
                    return new Color4(intValue);
                if (targetType == typeof(Avalonia.Media.Color))
                {
                    return Avalonia.Media.Color.FromArgb(
                        (byte)((intValue >> 24) & 255),
                        (byte)(intValue & 255),
                        (byte)((intValue >> 8) & 255),
                        (byte)((intValue >> 16) & 255));
                }
                if (targetType.IsAssignableFrom(typeof(SolidColorBrush)))
                {
                    return new SolidColorBrush(Avalonia.Media.Color.FromArgb(
                        (byte)((intValue >> 24) & 255),
                        (byte)(intValue & 255),
                        (byte)((intValue >> 8) & 255),
                        (byte)((intValue >> 16) & 255)));
                }
                if (targetType == typeof(string))
                    return stringColor;
            }

#if DEBUG
            if (value == null || value == AvaloniaProperty.UnsetValue)
                return AvaloniaProperty.UnsetValue;

            throw new NotSupportedException("Requested conversion is not supported.");
#else
            return DependencyProperty.UnsetValue;
#endif
        }

        /// <inheritdoc/>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(object))
                return value;

            return Convert(value, targetType, parameter, culture);
        }
    }
}
