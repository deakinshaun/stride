// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
//using System.Windows.Controls;
//using System.Windows.Markup;
//using System.Windows.Media;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Stride.Core.Annotations;
using Stride.Core.Presentation.Themes;

namespace Stride.Core.Presentation.MarkupExtensions
{
    //[MarkupExtensionReturnType(typeof(Image))]
    public class AImageExtension : MarkupExtension
    {
        private readonly IImage source;
        private readonly int width;
        private readonly int height;
        private readonly BitmapInterpolationMode scalingMode;

        public AImageExtension(object source)
        {
            this.source = (IImage) source;
            width = -1;
            height = -1;
        }

        public AImageExtension(object source, int width, int height)
            : this(source, width, height, BitmapInterpolationMode.Unspecified)
        {
        }

        public AImageExtension(object source, int width, int height, BitmapInterpolationMode scalingMode)
        {
            if (width < 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 0) throw new ArgumentOutOfRangeException(nameof(height));
            this.source = (IImage) source;
            this.width = width;
            this.height = height;
            this.scalingMode = scalingMode;
        }

        [NotNull]
        public override Image ProvideValue(IServiceProvider serviceProvider)
        {
            var image = new Image { Source = source };
            if (source is DrawingImage drawingImage)
            {
                image.Source = new DrawingImage()
                {
                    Drawing = AImageThemingUtilities.TransformDrawing((source as DrawingImage)?.Drawing, ThemeController.CurrentTheme.GetThemeBase().GetIconTheme())
                };
            }

            RenderOptions.SetBitmapInterpolationMode(image, scalingMode);
            if (width >= 0 && height >= 0)
            {
                image.Width = width;
                image.Height = height;
            }
            return image;
        }
    }
}
