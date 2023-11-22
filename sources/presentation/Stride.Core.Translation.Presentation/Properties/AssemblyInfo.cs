// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Markup;
using Avalonia.Markup;
using Avalonia.Metadata;

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

[assembly: System.Windows.Markup.XmlnsPrefix("http://schemas.stride3d.net/xaml/presentation", "sd")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Translation.Presentation")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Translation.Presentation.MarkupExtensions")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Translation.Presentation.ValueConverters")]

[assembly: Avalonia.Metadata.XmlnsPrefix("http://schemas.stride3d.net/xaml/presentation", "sd")]
[assembly: Avalonia.Metadata.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Translation.Presentation")]
[assembly: Avalonia.Metadata.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Translation.Presentation.MarkupExtensions")]
[assembly: Avalonia.Metadata.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Translation.Presentation.ValueConverters")]
