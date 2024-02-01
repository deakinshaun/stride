// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
//using System.Windows;

using Avalonia.Interactivity;

namespace Stride.Core.Presentation.Controls
{
    public class APropertyViewItemEventArgs : RoutedEventArgs
    {
        public APropertyViewItem Container { get; private set; }

        public object Item { get; private set; }

        public APropertyViewItemEventArgs(RoutedEvent routedEvent, object source, APropertyViewItem container, object item)
            : base(routedEvent, source)
        {
            Container = container;
            Item = item;
        }
    }
}
