// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using Avalonia.Markup.Xaml;

//using System.Windows.Markup;
using Stride.Core.Presentation.Internal;

namespace Stride.Core.Presentation.MarkupExtensions
{
 //   [MarkupExtensionReturnType(typeof(bool))]
    public sealed class ATrueExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return BooleanBoxes.TrueBox;
        }
    }
}
