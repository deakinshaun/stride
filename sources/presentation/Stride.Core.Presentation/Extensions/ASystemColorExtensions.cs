// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Stride.Core.Annotations;
using Stride.Core.Mathematics;

namespace Stride.Core.Presentation.Extensions
{
    using SystemColor = Avalonia.Media.Color;

    public static class ASystemColorExtensions
    {
        public static SystemColor AToSystemColor(this ColorHSV color)
        {
            return AToSystemColor(color.ToColor());
        }

        public static SystemColor AToSystemColor(this Color color)
        {
            return SystemColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static SystemColor AToSystemColor(this Color4 color4)
        {
            var color = (Color)color4;
            return SystemColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static SystemColor AToSystemColor(this Color3 color3)
        {
            var color = (Color)color3;
            return SystemColor.FromArgb(255, color.R, color.G, color.B);
        }
    }
}
