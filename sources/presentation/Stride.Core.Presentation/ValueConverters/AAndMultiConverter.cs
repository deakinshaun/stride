// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;

//using System.Windows;
using Stride.Core.Annotations;
using Stride.Core.Presentation.Internal;

namespace Stride.Core.Presentation.ValueConverters
{
    public class AAndMultiConverter : AOneWayMultiValueConverter<AAndMultiConverter>
    {
        [NotNull]
        public override object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count < 2)
                throw new InvalidOperationException("This multi converter must be invoked with at least two elements");

            var fallbackValue = parameter is bool && (bool)parameter;
            var result = values.All(x => x == AvaloniaProperty.UnsetValue ? fallbackValue : (bool)x);
            return result.Box();
        }
    }
}
