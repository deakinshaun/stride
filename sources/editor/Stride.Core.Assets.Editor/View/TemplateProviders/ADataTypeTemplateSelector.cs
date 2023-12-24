// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;



//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Markup;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Styling;

namespace Stride.Core.Assets.Editor.View.TemplateProviders
{
    public class ADataTemplateSelector : IDataTemplate
    {
        public virtual Control? Build(object? param)
        {
            return null;
        }

        public virtual bool Match(object? data)
        {
            return false;
        }

  /*      public virtual DataTemplate SelectTemplate(object item, AvaloniaObject container)
        {
            return null;
        }*/

    }


    //[ContentProperty("TemplateDefinitions")]
    public class ADataTypeTemplateSelector : ADataTemplateSelector
    {
        public ATemplateDefinitionCollection TemplateDefinitions { get; } = new ATemplateDefinitionCollection();

        public override Control? Build(object? param)
        {
            var templates = TemplateDefinitions;
            var template = templates.FirstOrDefault(t => t.DataType.IsInstanceOfType(param));
           /* var type = template?.DataType;
            var ctors = type.GetConstructors(BindingFlags.Public);
            var obj = (Control) ctors[0].Invoke(new object[] { });
            return obj;*/
           return template.DataTemplate.Build(param);
        }

        public override bool Match(object? data)
        {
            var templates = TemplateDefinitions;
            var template = templates.FirstOrDefault(t => t.DataType.IsInstanceOfType(data));
            return template != null;
        }

  /*      public override DataTemplate SelectTemplate(object item, AvaloniaObject container)
        {
            var uiElement = container as Control;
            if (uiElement == null)
            {
                return base.SelectTemplate(item, container);
            }

            var templates = TemplateDefinitions;
            if (templates == null || templates.Count == 0)
            {
                return base.SelectTemplate(item, container);
            }

            var template = templates.FirstOrDefault(t => t.DataType.IsInstanceOfType(item));
            return template?.DataTemplate ?? base.SelectTemplate(item, container);
        }*/
    }

    public class ATemplateDefinitionCollection : Collection<ATemplateDefinition> { }

    public class ATemplateDefinition : AvaloniaObject
    {
        public static readonly StyledProperty<Type> DataTypeProperty =
            StyledProperty<Type>.Register<ATemplateDefinition, Type>("DataType");
        
        public static readonly StyledProperty<DataTemplate> DataTemplateProperty =
           StyledProperty<DataTemplate>.Register<ATemplateDefinition, DataTemplate>("DataTemplate");

        public Type DataType
        {
            get { return (Type)GetValue(DataTypeProperty); }
            set { SetValue(DataTypeProperty, value); }
        }

        public DataTemplate DataTemplate
        {
            get { return (DataTemplate)GetValue(DataTemplateProperty); }
            set { SetValue(DataTemplateProperty, value); }
        }
    }
}
