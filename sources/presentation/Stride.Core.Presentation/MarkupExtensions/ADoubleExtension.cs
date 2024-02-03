// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia.Markup.Xaml;

//using System.Windows.Markup;
using Stride.Core.Annotations;

namespace Stride.Core.Presentation.MarkupExtensions
{
 //   [MarkupExtensionReturnType(typeof(double))]
    public class ADoubleExtension : MarkupExtension
    {
        public double Value { get; set; }

        public ADoubleExtension(object value)
        {
            Value = Convert.ToDouble(value);
        }

        [NotNull]
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Value;
        }
    }
}
