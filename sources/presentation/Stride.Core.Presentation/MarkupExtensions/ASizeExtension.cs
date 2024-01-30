// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia;
using Avalonia.Markup.Xaml;

//using System.Windows;
//using System.Windows.Markup;
using Stride.Core.Annotations;

namespace Stride.Core.Presentation.MarkupExtensions
{
//    [MarkupExtensionReturnType(typeof(SizeExtension))]
    public class ASizeExtension : MarkupExtension
    {
        public ASizeExtension(double uniformLength)
        {
            Value = new Size(uniformLength, uniformLength);
        }

        public ASizeExtension(double width, double height)
        {
            Value = new Size(width, height);
        }

        public ASizeExtension(Size value)
        {
            Value = value;
        }

        public Size Value { get; set; }

        [NotNull]
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Value;
        }
    }
}
