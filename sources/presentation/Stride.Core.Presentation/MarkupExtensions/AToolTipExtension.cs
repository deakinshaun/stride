// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia.Markup.Xaml;

//using System.Windows.Markup;
using Stride.Core.Annotations;

namespace Stride.Core.Presentation.MarkupExtensions
{
    /// <summary>
    /// This markup extension allows to format the text of a tooltip a text and a gesture.
    /// </summary>
    public class AToolTipExtension : MarkupExtension
    {
        private readonly string content;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolTipExtension"/> class.
        /// </summary>
        /// <param name="text">A string representing the tooltip text</param>
        /// <param name="gesture">A string representing the gesture.</param>
        public AToolTipExtension(object text, object gesture)
        {
            content = !string.IsNullOrEmpty((string) gesture) ? $"{(string)text} ({(string)gesture})" : (string)text;
        }

        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return content;
        }
    }
}
