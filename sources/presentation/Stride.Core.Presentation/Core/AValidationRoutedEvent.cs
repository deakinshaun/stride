// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
//using System.Windows;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Stride.Core.Presentation.Core
{
    public class AValidationRoutedEventArgs : RoutedEventArgs
    {
        public object Value { get; }

        public AValidationRoutedEventArgs(RoutedEvent routedEvent, object value)
            : base(routedEvent)
        {
            Value = value;
        }
    }

    public class AValidationRoutedEventArgs<T> : AValidationRoutedEventArgs
    {
        public new T Value => (T)base.Value;

        public AValidationRoutedEventArgs(RoutedEvent routedEvent, T value)
            : base(routedEvent, value)
        {
        }
    }

    public delegate void VAalidationRoutedEventHandler(object sender, AValidationRoutedEventArgs e);

    public delegate void AValidationRoutedEventHandler<T>(object sender, AValidationRoutedEventArgs<T> e);
}
