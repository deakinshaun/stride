// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Stride.Core.Annotations;
using Stride.Core.Presentation.Extensions;

namespace Stride.Core.Presentation.ValueConverters
{
    /// <summary>
    /// This converter convert any object to a string representing the name of its type (without assembly or namespace qualification).
    /// It accepts null and will convert it to a string representation of null.
    /// </summary>
    /// <seealso cref="ObjectToFullTypeName"/>
    /// <seealso cref="ObjectToType"/>
    public class AObjectToTypeName : AOneWayValueConverter<AObjectToTypeName>
    {
        /// <summary>
        /// The string representation of the type of a null object
        /// </summary>
        public const string NullObjectType = "(None)";

        /// <inheritdoc/>
        [NotNull]
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.GetType().ToSimpleCSharpName() ?? NullObjectType;
        }
    }
}
