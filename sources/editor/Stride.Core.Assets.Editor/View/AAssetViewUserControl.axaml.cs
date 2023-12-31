// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections;
using System.Linq;
using System.Windows.Input;
using Avalonia;

//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Stride.Core.Assets.Editor.View.Controls;
using Stride.Core.Assets.Editor.ViewModel;
using Stride.Core.Assets.Quantum.Visitors;
using Stride.Core.Presentation.Collections;
using Stride.Core.Presentation.Extensions;


namespace Stride.Core.Assets.Editor.View
{
    /// <summary>
    /// Interaction logic for AssetViewUserControl.xaml
    /// </summary>
    public partial class AAssetViewUserControl : UserControl
    {
        /// <summary>
        /// Identifies the <see cref="AssetCollection"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<AssetCollectionViewModel> AssetCollectionProperty = StyledProperty<AssetCollectionViewModel>.Register<AAssetViewUserControl,AssetCollectionViewModel>(nameof(AssetCollection), null);

        /// <summary>
        /// Identifies the <see cref="AssetContextMenu"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<Control> AssetContextMenuProperty = StyledProperty<Control>.Register<AAssetViewUserControl,Control>(nameof(AssetContextMenu), null);

        /// <summary>
        /// Identifies the <see cref="CanEditAssets"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> CanEditAssetsProperty = StyledProperty<bool>.Register<AAssetViewUserControl,bool>(nameof(CanEditAssets), true);

        /// <summary>
        /// Identifies the <see cref="CanAddAssets"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> CanAddAssetsProperty = StyledProperty<bool>.Register<AAssetViewUserControl,bool>(nameof(CanAddAssets), true);

        /// <summary>
        /// Identifies the <see cref="CanDeleteAssets"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> CanDeleteAssetsProperty = StyledProperty<bool>.Register<AAssetViewUserControl,bool>(nameof(CanDeleteAssets), true);

        /// <summary>
        /// Identifies the <see cref="CanRecursivelyDisplayAssets"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> CanRecursivelyDisplayAssetsProperty = StyledProperty<bool>.Register<AAssetViewUserControl,bool>(nameof(CanRecursivelyDisplayAssets), true);

        /// <summary>
        /// Identifies the <see cref="CanRecursivelyDisplayAssets"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<bool> GiveFocusOnSelectionChangeProperty = StyledProperty<bool>.Register<AAssetViewUserControl,bool>(nameof(GiveFocusOnSelectionChange), true);

        /// <summary>
        /// Identifies the <see cref="ThumbnailZoomIncrement"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> ThumbnailZoomIncrementProperty = StyledProperty<double>.Register<AAssetViewUserControl,double>(nameof(ThumbnailZoomIncrement), 16.0);

        /// <summary>
        /// Identifies the <see cref="ThumbnailMinimumSize"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> ThumbnailMinimumSizeProperty = StyledProperty<double>.Register<AAssetViewUserControl,double>(nameof(ThumbnailMinimumSize), 16.0);

        /// <summary>
        /// Identifies the <see cref="ThumbnailMaximumSize"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> ThumbnailMaximumSizeProperty = StyledProperty<double>.Register<AAssetViewUserControl,double>(nameof(ThumbnailMaximumSize), 128.0);

        /// <summary>
        /// Identifies the <see cref="TileThumbnailSize"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> TileThumbnailSizeProperty = StyledProperty<double>.Register<AAssetViewUserControl,double>(nameof(TileThumbnailSize), 96.0);

        /// <summary>
        /// Identifies the <see cref="GridThumbnailSize"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<double> GridThumbnailSizeProperty = StyledProperty<double>.Register<AAssetViewUserControl,double>(nameof(GridThumbnailSize), 16.0);

        /// <summary>
        /// Identifies the <see cref="AssetDoubleClick"/> dependency property.
        /// </summary>
        public static readonly StyledProperty<ICommand> AssetDoubleClickProperty = StyledProperty<ICommand>.Register<AAssetViewUserControl,ICommand>(nameof(AssetDoubleClick), null);

        /// <summary>
        /// Gets the command that initiate the edition of the currently selected item.
        /// </summary>
        public static RoutedCommand BeginEditCommand { get; }

        /// <summary>
        /// Gets the command that will increase the size of thumbnails.
        /// </summary>
        public static RoutedCommand ZoomInCommand { get; }

        /// <summary>
        /// Gets the command that will decrease the size of thumbnails.
        /// </summary>
        public static RoutedCommand ZoomOutCommand { get; }

        static AAssetViewUserControl()
        {
            AssetCollectionProperty.Changed.AddClassHandler<AAssetViewUserControl>(AssetCollectionChanged);
            AssetContextMenuProperty.Changed.AddClassHandler<AAssetViewUserControl>(OnAssetContextMenuChanged);

            BeginEditCommand = new RoutedCommand(nameof(BeginEditCommand), typeof(AssetViewUserControl));
            CommandManager.RegisterClassCommandBinding(typeof(AssetViewUserControl), new CommandBinding(BeginEditCommand, BeginEdit, CanBeginEditCommand));
            CommandManager.RegisterClassInputBinding(typeof(AssetViewUserControl), new InputBinding(BeginEditCommand, new KeyGesture(Key.F2)));

            ZoomInCommand = new RoutedCommand(nameof(ZoomInCommand), typeof(AssetViewUserControl));
            var zoomInCommandBinding = new CommandBinding(ZoomInCommand, ZoomIn);
            zoomInCommandBinding.PreviewCanExecute += (s, e) => e.CanExecute = true;
            zoomInCommandBinding.PreviewExecuted += ZoomIn;
            CommandManager.RegisterClassCommandBinding(typeof(AssetViewUserControl), zoomInCommandBinding);

            ZoomOutCommand = new RoutedCommand(nameof(ZoomOutCommand), typeof(AssetViewUserControl));
            var zoomOutCommandBinding = new CommandBinding(ZoomOutCommand, ZoomOut);
            zoomOutCommandBinding.PreviewCanExecute += (s, e) => e.CanExecute = true;
            zoomOutCommandBinding.PreviewExecuted += ZoomOut;
            CommandManager.RegisterClassCommandBinding(typeof(AssetViewUserControl), zoomOutCommandBinding);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetViewUserControl"/> class.
        /// </summary>
        public AAssetViewUserControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Gets or sets the <see cref="AssetCollectionViewModel"/> to display in this control.
        /// </summary>
        public AssetCollectionViewModel AssetCollection { get => (AssetCollectionViewModel)GetValue(AssetCollectionProperty); set => SetValue(AssetCollectionProperty, value); }

        /// <summary>
        /// Gets or sets the control to use as context menu for assets.
        /// </summary>
        public Control AssetContextMenu { get => (Control)GetValue(AssetContextMenuProperty); set => SetValue(AssetContextMenuProperty, value); }

        /// <summary>
        /// Gets the list of items to display in the primary tool bar. The primary tool bar won't be displayed if this list is empty.
        /// </summary>
        public IList PrimaryToolBarItems { get; } = new NonGenericObservableListWrapper<object>(new ObservableList<object>());

        /// <summary>
        /// Gets or sets whether it is possible to edit assets with this <see cref="AssetViewUserControl"/>.
        /// </summary>
        public bool CanEditAssets { get => (bool)GetValue(CanEditAssetsProperty); set => SetValue(CanEditAssetsProperty, value); }

        /// <summary>
        /// Gets or sets whether it is possible to add assets with this <see cref="AssetViewUserControl"/>.
        /// </summary>
        public bool CanAddAssets { get => (bool)GetValue(CanAddAssetsProperty); set => SetValue(CanAddAssetsProperty, value); }

        /// <summary>
        /// Gets or sets whether it is possible to delete assets with this <see cref="AssetViewUserControl"/>.
        /// </summary>
        public bool CanDeleteAssets { get => (bool)GetValue(CanDeleteAssetsProperty); set => SetValue(CanDeleteAssetsProperty, value); }

        /// <summary>
        /// Gets or sets whether it is possible to select to display asset recursively from selected locations.
        /// </summary>
        public bool CanRecursivelyDisplayAssets { get => (bool)GetValue(CanRecursivelyDisplayAssetsProperty); set => SetValue(CanRecursivelyDisplayAssetsProperty, value); }

        /// <summary>
        /// Gets or sets whether the control should get the focus when its selection changes. The focus is not given if the selection is cleared.
        /// </summary>
        public bool GiveFocusOnSelectionChange { get => (bool)GetValue(GiveFocusOnSelectionChangeProperty); set => SetValue(GiveFocusOnSelectionChangeProperty, value); }

        /// <summary>
        /// Gets or sets the zoom increment value.
        /// </summary>
        public double ThumbnailZoomIncrement { get => (double)GetValue(ThumbnailZoomIncrementProperty); set => SetValue(ThumbnailZoomIncrementProperty, value); }

        /// <summary>
        /// Gets or sets the minimum size of thumbnails.
        /// </summary>
        public double ThumbnailMinimumSize { get => (double)GetValue(ThumbnailMinimumSizeProperty); set => SetValue(ThumbnailMinimumSizeProperty, value); }

        /// <summary>
        /// Gets or sets the maximum size of thumbnails.
        /// </summary>
        public double ThumbnailMaximumSize { get => (double)GetValue(ThumbnailMaximumSizeProperty); set => SetValue(ThumbnailMaximumSizeProperty, value); }

        /// <summary>
        /// Gets or sets the size of thumbnails in tile view.
        /// </summary>
        public double TileThumbnailSize { get => (double)GetValue(TileThumbnailSizeProperty); set => SetValue(TileThumbnailSizeProperty, value); }

        /// <summary>
        /// Gets or sets the size of thumbnails in grid view.
        /// </summary>
        public double GridThumbnailSize { get => (double)GetValue(GridThumbnailSizeProperty); set => SetValue(GridThumbnailSizeProperty, value); }

        /// <summary>
        /// Gets or sets the command to execute when user double-clicks an asset.
        /// </summary>
        public ICommand AssetDoubleClick { get => (ICommand)GetValue(AssetDoubleClickProperty); set => SetValue(AssetDoubleClickProperty, value); }

        /// <summary>
        /// Begins edition of the currently selected content.
        /// </summary>
        public void BeginEdit()
        {
            if (!CanEditAssets || AssetCollection == null)
                return;

            var selectedAsset = AssetCollection.SelectedContent.LastOrDefault();
            if (selectedAsset == null)
                return;

 /*           var listBox = AssetViewPresenter.FindVisualChildOfType<EditableContentListBox>();
            listBox?.BeginEdit();

            var gridView = AssetViewPresenter.FindVisualChildOfType<DataGridEx>();
            gridView?.BeginEdit();*/
        }

        private void ZoomIn()
        {
/*            var listBox = AssetViewPresenter.FindVisualChildOfType<EditableContentListBox>();
            if (listBox != null)
            {
                TileThumbnailSize += ThumbnailZoomIncrement;
                if (TileThumbnailSize >= ThumbnailMaximumSize)
                {
                    TileThumbnailSize = ThumbnailMaximumSize;
                }
            }

            var gridView = AssetViewPresenter.FindVisualChildOfType<DataGridEx>();
            if (gridView != null)
            {
                GridThumbnailSize += ThumbnailZoomIncrement;
                if (GridThumbnailSize >= ThumbnailMaximumSize)
                {
                    GridThumbnailSize = ThumbnailMaximumSize;
                }
            }*/
        }

        private void ZoomOut()
        {
            foreach (var child in AssetViewPresenter.GetVisualChildren())
            {
                var listBox = child as AEditableContentListBox;
                if (listBox != null)
                {
                    TileThumbnailSize -= ThumbnailZoomIncrement;
                    if (TileThumbnailSize <= ThumbnailMinimumSize)
                    {
                        TileThumbnailSize = ThumbnailMinimumSize;
                    }
                }

/*                var gridView = child as DataGridEx;
                if (gridView != null)
                {
                    GridThumbnailSize -= ThumbnailZoomIncrement;
                    if (GridThumbnailSize <= ThumbnailMinimumSize)
                    {
                        GridThumbnailSize = ThumbnailMinimumSize;
                    }
                }*/
            }
        }

 /*       protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Delta > 0)
                {
                    ZoomIn();
                    e.Handled = true;
                }
                if (e.Delta < 0)
                {
                    ZoomOut();
                    e.Handled = true;
                }
            }
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            if (!IsFocused && !IsKeyboardFocusWithin)
                Focus();
        }*/

        private bool CanBeginEdit()
        {
            if (!CanEditAssets || AssetCollection == null)
                return false;

            // Special case to under edition state restoration in the DataGrid
            if (AssetCollection.SelectedContent.Count == 0)
                return true;

            if (AssetCollection.SelectedContent.Count != 1)
                return false;

            // HACK: might be a better way to check that
            var asset = AssetCollection.SelectedContent.Last() as AssetViewModel;
            return !asset?.IsLocked ?? true;
        }

        private static void CanBeginEditCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            var control = (AAssetViewUserControl)sender;
            e.CanExecute = control.CanBeginEdit();
        }

        private static void BeginEdit(object sender, ExecutedRoutedEventArgs e)
        {
            var assetView = (AAssetViewUserControl)sender;
            assetView.BeginEdit();
        }

        private static void ZoomIn(object sender, ExecutedRoutedEventArgs e)
        {
            var assetView = (AAssetViewUserControl)sender;
            assetView.ZoomIn();
        }

        private static void ZoomOut(object sender, ExecutedRoutedEventArgs e)
        {
            var assetView = (AAssetViewUserControl)sender;
            assetView.ZoomOut();
        }

        /// <summary>
        /// Raised when the <see cref="AssetCollection"/> dependency property changes.
        /// </summary>
        /// <param name="d">The event sender.</param>
        /// <param name="e">The event argument.</param>
        private static void AssetCollectionChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            var assetViewControl = (AAssetViewUserControl)d;
            assetViewControl.RootContainer.DataContext = e.NewValue;
        }

        /// <summary>
        /// Raised when the <see cref="AssetContextMenu"/> dependency property changes.
        /// </summary>
        /// <param name="d">The event sender.</param>
        /// <param name="e">The event argument.</param>
        private static void OnAssetContextMenuChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
//                NameScope.SetNameScope((AvaloniaObject)e.NewValue, NameScope.GetNameScope(d));
            }
        }
    }
}
