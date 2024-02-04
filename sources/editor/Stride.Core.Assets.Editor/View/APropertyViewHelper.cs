// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;

//using System.Windows;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Stride.Core.Presentation.Controls;
using Stride.Core.Presentation.Extensions;
using Stride.Core.Presentation.View;

namespace Stride.Core.Assets.Editor.View
{
    /// <summary>
    /// This static class contains helper dependency properties that allows to override some properties of the parent <see cref="PropertyViewItem"/> of a control.
    /// </summary>
    public class APropertyViewHelper : Control
    {
        public enum Category
        {
            PropertyHeader,
            PropertyFooter,
            PropertyEditor,
        };

        static APropertyViewHelper()
        {
        IncrementProperty.Changed.AddClassHandler<APropertyViewHelper>(OnIncrementChanged);
        IsExpandedProperty.Changed.AddClassHandler<APropertyViewHelper>(OnIsExpandedChanged);

        ToggleNestedPropertiesCommand = new RoutedCommand("ToggleNestedPropertiesCommand", typeof(PropertyViewHelper));
            CommandManager.RegisterClassCommandBinding(typeof(PropertyViewItem), new CommandBinding(ToggleNestedPropertiesCommand, OnToggleNestedProperties));
        }

        public static readonly StyledProperty<double> IncrementProperty = StyledProperty<double>.Register<APropertyViewHelper, double>("Increment");

        public static readonly StyledProperty<bool> IsExpandedProperty = StyledProperty<bool>.Register<APropertyViewHelper, bool>("IsExpanded");

        public static readonly StyledProperty<Category> TemplateCategoryProperty = StyledProperty<Category>.Register<APropertyViewHelper, Category>("TemplateCategory", Category.PropertyHeader);

        public static RoutedCommand ToggleNestedPropertiesCommand { get; }

        public static readonly ATemplateProviderSelector HeaderProviders = new ATemplateProviderSelector();

        public static readonly ATemplateProviderSelector EditorProviders = new ATemplateProviderSelector();

        public static readonly ATemplateProviderSelector FooterProviders = new ATemplateProviderSelector();

        public static double GetIncrement(AvaloniaObject target)
        {
            return (double)target.GetValue(IncrementProperty);
        }

        public static void SetIncrement(AvaloniaObject target, double value)
        {
            target.SetValue(IncrementProperty, value);
        }

        public static Category GetTemplateCategory(AvaloniaObject target)
        {
            return (Category)target.GetValue(TemplateCategoryProperty);
        }

        public static void SetTemplateCategory(AvaloniaObject target, Category value)
        {
            target.SetValue(TemplateCategoryProperty, value);
        }

        public static bool GetIsExpanded(AvaloniaObject target)
        {
            return (bool)target.GetValue(IsExpandedProperty);
        }

        [Obsolete("Use the DisplayAttribute on the properties")]
        public static void SetIsExpanded(AvaloniaObject target, bool value)
        {
            target.SetValue(IsExpandedProperty, value);
        }

        private static void OnPropertyChanged(AvaloniaObject d, AvaloniaProperty property, object newValue)
        {
            if (newValue == null)
                return;

  /*          var target = d as PropertyViewItem ?? d.FindVisualParentOfType<PropertyViewItem>();
            target?.SetCurrentValue(property, newValue);*/
        }

        private static void OnIncrementChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            OnPropertyChanged(d, APropertyViewItem.IncrementProperty, e.NewValue);
        }

        private static void OnIsExpandedChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            OnPropertyChanged(d, APropertyViewItem.IsExpandedProperty, e.NewValue);
        }

        private static void OnToggleNestedProperties(object sender, ExecutedRoutedEventArgs e)
        {
            var d = sender as AvaloniaObject;
/*            if (d != null)
            {
                var target = sender as PropertyViewItem ?? d.FindVisualParentOfType<PropertyViewItem>();
                if (target != null)
                {
                    var currentValue = true;

                    for (var i = 0; i < target.Items.Count; i++)
                    {
                        var container = (PropertyViewItem)target.ItemContainerGenerator.ContainerFromIndex(i);
                        if (!container.IsExpanded)
                        {
                            currentValue = false;
                            break;
                        }
                    }
                    for (var i = 0; i < target.Items.Count; i++)
                    {
                        var container = (PropertyViewItem)target.ItemContainerGenerator.ContainerFromIndex(i);
                        container.SetCurrentValue(PropertyViewItem.IsExpandedProperty, !currentValue);
                    }
                }
            }*/
        }
    }
}

