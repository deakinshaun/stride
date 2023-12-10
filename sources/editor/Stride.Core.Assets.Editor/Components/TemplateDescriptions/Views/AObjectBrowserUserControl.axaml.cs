// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.Collections;
using Avalonia.Controls;
//using System.Windows.Controls;
//using System.Windows.Data;
using Avalonia;
using Avalonia.Styling;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Markup.Xaml;
using Avalonia.Data;

namespace Stride.Core.Assets.Editor.Components.TemplateDescriptions.Views
{
    /// <summary>
    /// Interaction logic for ObjectBrowserUserControl.xaml
    /// </summary>
    public partial class AObjectBrowserUserControl : Avalonia.Controls.UserControl
    {
        public static readonly StyledProperty<IEnumerable> HierarchyItemsSourceProperty = StyledProperty<IEnumerable>.Register<AObjectBrowserUserControl, IEnumerable>("HierarchyItemsSource");

        public static readonly StyledProperty<object> SelectedHierarchyItemProperty = StyledProperty<object>.Register<AObjectBrowserUserControl, object>("SelectedHierarchyItem", defaultBindingMode : BindingMode.TwoWay/*,  new FrameworkPropertyMetadata { BindsTwoWayByDefault = true, DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged }*/);

        public static readonly StyledProperty<DataTemplate> HierarchyItemTemplateProperty = StyledProperty<DataTemplate>.Register<AObjectBrowserUserControl, DataTemplate>("HierarchyItemTemplate");

        public static readonly StyledProperty<Style> HierarchyItemContainerStyleProperty = StyledProperty<Style>.Register<AObjectBrowserUserControl, Style>("HierarchyItemContainerStyle");

        public static readonly StyledProperty<IEnumerable> ObjectItemsSourceProperty = StyledProperty<IEnumerable>.Register<AObjectBrowserUserControl, IEnumerable>("ObjectItemsSource");

        public static readonly StyledProperty<object> SelectedObjectItemProperty = StyledProperty<object>.Register<AObjectBrowserUserControl, object>("SelectedObjectItem", defaultBindingMode: BindingMode.TwoWay/*typeof(object), typeof(ObjectBrowserUserControl), new FrameworkPropertyMetadata { BindsTwoWayByDefault = true, DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged }*/);

        public static readonly StyledProperty<DataTemplate> ObjectItemTemplateProperty = StyledProperty<DataTemplate>.Register<AObjectBrowserUserControl, DataTemplate>("ObjectItemTemplate");

        //public static readonly StyledProperty<DataTemplateSelector> ObjectItemTemplateSelectorProperty = StyledProperty<DataTemplateSelector>.Register<AObjectBrowserUserControl, DataTemplateSelector>("ObjectItemTemplateSelector");

        public static readonly StyledProperty<Style> ObjectItemContainerStyleProperty = StyledProperty<Style>.Register<AObjectBrowserUserControl, Style>("ObjectItemContainerStyle");

        public static readonly StyledProperty<DataTemplate> ObjectDescriptionTemplateProperty = StyledProperty<DataTemplate>.Register<AObjectBrowserUserControl, DataTemplate>("ObjectDescriptionTemplate");

       /* public static readonly StyledProperty<DataTemplateSelector> ObjectDescriptionTemplateSelectorProperty = StyledProperty<DataTemplateSelector>.Register<AObjectBrowserUserControl, DataTemplateSelector>("ObjectDescriptionTemplateSelector", typeof(DataTemplateSelector), typeof(ObjectBrowserUserControl));*/

        public AObjectBrowserUserControl()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public IEnumerable HierarchyItemsSource { get { return (IEnumerable)GetValue(HierarchyItemsSourceProperty); } set { SetValue(HierarchyItemsSourceProperty, value); } }

        public object SelectedHierarchyItem { get { return GetValue(SelectedHierarchyItemProperty); } set { SetValue(SelectedHierarchyItemProperty, value); } }

        public DataTemplate HierarchyItemTemplate { get { return (DataTemplate)GetValue(HierarchyItemTemplateProperty); } set { SetValue(HierarchyItemTemplateProperty, value); } }

        public Style HierarchyItemContainerStyle { get { return (Style)GetValue(HierarchyItemContainerStyleProperty); } set { SetValue(HierarchyItemContainerStyleProperty, value); } }

        public IEnumerable ObjectItemsSource { get { return (IEnumerable)GetValue(ObjectItemsSourceProperty); } set { SetValue(ObjectItemsSourceProperty, value); } }

        public object SelectedObjectItem { get { return GetValue(SelectedObjectItemProperty); } set { SetValue(SelectedObjectItemProperty, value); } }

        public DataTemplate ObjectItemTemplate { get { return (DataTemplate)GetValue(ObjectItemTemplateProperty); } set { SetValue(ObjectItemTemplateProperty, value); } }

        //public DataTemplateSelector ObjectItemTemplateSelector { get { return (DataTemplateSelector)GetValue(ObjectItemTemplateSelectorProperty); } set { SetValue(ObjectItemTemplateSelectorProperty, value); } }

        public Style ObjectItemContainerStyle { get { return (Style)GetValue(ObjectItemContainerStyleProperty); } set { SetValue(ObjectItemContainerStyleProperty, value); } }

        public DataTemplate ObjectDescriptionTemplate { get { return (DataTemplate)GetValue(ObjectDescriptionTemplateProperty); } set { SetValue(ObjectDescriptionTemplateProperty, value); } }
        
        //public DataTemplateSelector ObjectDescriptionTemplateSelector { get { return (DataTemplateSelector)GetValue(ObjectDescriptionTemplateSelectorProperty); } set { SetValue(ObjectDescriptionTemplateSelectorProperty, value); } }

    /*    private void SelectedObjectUpdated(object sender, DataTransferEventArgs e)
        {
            DescriptionScrollViewer.ScrollToTop();
        }*/
    }
}
