// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
using System.Windows.Markup;

using Avalonia.Markup;
using Avalonia.Metadata;


// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]
[assembly: InternalsVisibleTo("Stride.GameStudio")]
[assembly: InternalsVisibleTo("Stride.Core.Assets.Editor.Tests")]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]

[assembly: System.Windows.Markup.XmlnsPrefix("http://schemas.stride3d.net/xaml/presentation", "sd")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor.Components.Status.Views")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor.Quantum.ViewModels")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor.Settings.ViewModels")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor.View")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor.View.Behaviors")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor.View.Controls")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor.View.TemplateProviders")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor.View.ValueConverters")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor.ViewModel")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor.ViewModel.Logs")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor.ViewModel.Progress")]

[assembly: Avalonia.Metadata.XmlnsPrefix("http://schemas.stride3d.net/xaml/presentation", "sd")]
[assembly: Avalonia.Metadata.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor")]
[assembly: Avalonia.Metadata.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor.Components.Status.Views")]
[assembly: Avalonia.Metadata.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor.Quantum.ViewModels")]
[assembly: Avalonia.Metadata.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor.Settings.ViewModels")]
[assembly: Avalonia.Metadata.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor.View")]
[assembly: Avalonia.Metadata.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor.View.Behaviors")]
[assembly: Avalonia.Metadata.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor.View.Controls")]
[assembly: Avalonia.Metadata.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor.View.TemplateProviders")]
[assembly: Avalonia.Metadata.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor.View.ValueConverters")]
[assembly: Avalonia.Metadata.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor.ViewModel")]
[assembly: Avalonia.Metadata.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor.ViewModel.Logs")]
[assembly: Avalonia.Metadata.XmlnsDefinition("http://schemas.stride3d.net/xaml/presentation", "Stride.Core.Assets.Editor.ViewModel.Progress")]
