// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Avalonia;

//using System.Windows;
//using System.Windows.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Templates;
using Stride.Core.Assets.Editor.ViewModel;

namespace Stride.Core.Assets.Editor.View
{
    public class ATreeViewTemplateSelector : IDataTemplate
    {
        public DataTemplate AssetMountPointTemplate { get; set; }

        public DataTemplate DirectoryTemplate { get; set; }

        public DataTemplate PackageTemplate { get; set; }

        public DataTemplate DependencyCategoryTemplate { get; set; }

        public DataTemplate ProjectTemplate { get; set; }

        public DataTemplate ProjectCodeTemplate { get; set; }

        public DataTemplate PackageReferenceTemplate { get; set; }

        public bool Match(object? item)
        {
            return true;
        }
     
        public Control Build(object? item)
        {
            if (item == null)
            {
                return null;
            }
            if (item is ProjectViewModel)
            {
                return ProjectTemplate.Build (item);
            }
            if (item is PackageViewModel)
            {
                return PackageTemplate.Build(item);
            }
            if (item is DirectoryViewModel)
            {
                return DirectoryTemplate.Build(item);
            }
            if (item is AssetMountPointViewModel)
            {
                return AssetMountPointTemplate.Build(item);
            }
            if (item is DependencyCategoryViewModel)
            {
                return DependencyCategoryTemplate.Build(item);
            }
            if (item is ProjectCodeViewModel)
            {
                return ProjectCodeTemplate.Build(item);
            }
            if (item is PackageReferenceViewModel)
            {
                return PackageReferenceTemplate.Build(item);
            }
            throw new ArgumentException("The type of item is unsupported.");
        }
    }
}
