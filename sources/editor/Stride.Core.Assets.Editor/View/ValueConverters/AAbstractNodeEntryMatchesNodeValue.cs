// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;


//using System.Windows;
//using System.Windows.Data;
//using System.Windows.Media;
using Stride.Core.Assets.Editor.Quantum.NodePresenters.Commands;
using Stride.Core.Presentation.ValueConverters;

namespace Stride.Core.Assets.Editor.View.ValueConverters
{
    public abstract class AAbstractNodeEntryMatchesNodeValue<T> : AOneWayMultiValueConverter<T> where T : class, IMultiValueConverter, new()
    {
        protected abstract object ReturnValue { get; }

        public override object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count != 2)
                throw new InvalidOperationException("This multi converter must be invoked with two elements");

            var entry = values[0] as AbstractNodeEntry;
            var value = values[1];

            return entry != null && entry.IsMatchingValue(value) ? ReturnValue : AvaloniaProperty.UnsetValue;
        }
    }

    public class AAbstractNodeEntryToFontWeight : AAbstractNodeEntryMatchesNodeValue<AAbstractNodeEntryToFontWeight>
    {
        public static readonly FontWeight FontWeight = FontWeight.ExtraBold;

        protected override object ReturnValue { get { return FontWeight; } }
    }

    public class AAbstractNodeEntryToBrush : AAbstractNodeEntryMatchesNodeValue<AAbstractNodeEntryToBrush>
    {
        public static readonly Brush Brush = new SolidColorBrush(Colors.SkyBlue);

        protected override object ReturnValue { get { return Brush; } }
    }
}
