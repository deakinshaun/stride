// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Data;
//using System.Windows.Input;
using Stride.Core.Assets.Editor.Extensions;
using Stride.Core.Assets.Editor.Services;
using Stride.Core.Assets.Editor.ViewModel;
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Extensions;
using Stride.Core.Mathematics;
using Stride.Core.Threading;
using Stride.Animations;
using Stride.Assets.Presentation.CurveEditor.ViewModels;
using Stride.Assets.Presentation.CurveEditor.Views;
using AvalonDock.Layout;
using Stride.GameStudio.Helpers;
using Stride.GameStudio.Layout;
using Avalonia.Data;
using Avalonia.Controls;
using Avalonia.Interactivity;
using static Stride.Core.Assets.Yaml.YamlAssetPath;
using Avalonia;
using DynamicData;
using Dock.Model.Avalonia.Controls;
using Avalonia.LogicalTree;
using Stride.GameStudio.View;

namespace Stride.GameStudio.AssetsEditors
{
    internal sealed class AAssetEditorsManager : IAssetEditorsManager, IDestroyable
    {
        private readonly ConditionalWeakTable<IMultipleAssetEditorViewModel, NotifyCollectionChangedEventHandler> registeredHandlers = new();
        private readonly Dictionary<IAssetEditorViewModel, ALayoutAnchorable> assetEditors = new();
        private readonly HashSet<AssetViewModel> openedAssets = new HashSet<AssetViewModel>();
        // TODO have a base interface for all editors and factorize to make curve editor not be a special case anymore
        private Tuple<CurveEditorViewModel, ALayoutAnchorable> curveEditor;

        private readonly AsyncLock mutex = new AsyncLock();
        private readonly ADockingLayoutManager dockingLayoutManager;
        private readonly SessionViewModel session;

        public AAssetEditorsManager([NotNull] ADockingLayoutManager dockingLayoutManager, [NotNull] SessionViewModel session)
        {
            this.dockingLayoutManager = dockingLayoutManager ?? throw new ArgumentNullException(nameof(dockingLayoutManager));
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            session.DeletedAssetsChanged += AssetsDeleted;
        }

        /// <summary>
        /// Gets the list of assets that are currently opened in an editor.
        /// </summary>
        /// <remarks>
        /// This does not include all assets in <see cref="IMultipleAssetEditorViewModel"/> but rather those that were explicitly opened.
        /// </remarks>
        public IReadOnlyCollection<AssetViewModel> OpenedAssets => openedAssets;

        /// <inheritdoc />
        void IDestroyable.Destroy()
        {
            session.DeletedAssetsChanged -= AssetsDeleted;
        }

        /// <inheritdoc/>
        public void OpenCurveEditorWindow([NotNull] object curve, string name)
        {
            if (curve == null) throw new ArgumentNullException(nameof(curve));
            if (dockingLayoutManager == null) throw new InvalidOperationException("This method can only be invoked on the IEditorDialogService that has the editor main window as parent.");

            CurveEditorViewModel editorViewModel = null;
            ALayoutAnchorable editorPane = null;

            if (curveEditor != null)
            {
                // curve editor already exists
                editorViewModel = curveEditor.Item1;
                editorPane = curveEditor.Item2;
            }

            // Create the editor view model if needed
            editorViewModel ??= new CurveEditorViewModel(session.ServiceProvider, session);

            // Populate the editor view model
            if (curve is IComputeCurve<Color4> color4curve)
            {
                editorViewModel.AddCurve(color4curve, name);
            }
            else if (curve is IComputeCurve<float> floatCurve)
            {
                editorViewModel.AddCurve(floatCurve, name);
            }
            else if (curve is IComputeCurve<Quaternion> quaternionCurve)
            {
                editorViewModel.AddCurve(quaternionCurve, name);
            }
            else if (curve is IComputeCurve<Vector2> vec2curve)
            {
                editorViewModel.AddCurve(vec2curve, name);
            }
            else if (curve is IComputeCurve<Vector3> vec3curve)
            {
                editorViewModel.AddCurve(vec3curve, name);
            }
            else if (curve is IComputeCurve<Vector4> vec4curve)
            {
                editorViewModel.AddCurve(vec4curve, name);
            }

            editorViewModel.Focus();

            // Create the editor pane if needed
            if (editorPane == null)
            {
                editorPane = new ALayoutAnchorable
                {
                    Content = new CurveEditorView { DataContext = editorViewModel },
                    Title = "Curve Editor"//,
 //                   CanClose = true,
                };

 //               editorPane.Closed += CurveEditorClosed;
                editorPane.Factory.DockableClosed += CurveEditorClosed;

                //               AvalonDockHelper.GetDocumentPane(dockingLayoutManager.DockingManager).Children.Add(editorPane);
            }

            curveEditor = Tuple.Create(editorViewModel, editorPane);

            MakeActiveVisible(editorPane);
        }

        /// <inheritdoc/>
        public void CloseCurveEditorWindow()
        {
            RemoveCurveEditor(true);
        }

        private void RemoveCurveEditor(bool removePane)
        {
            if (curveEditor == null)
                return;

            var editor = curveEditor.Item1;
            var pane = curveEditor.Item2;
            curveEditor = null;
            // clean view model
            editor.Destroy();

            CleanEditorPane(pane);
            if (removePane)
            {
                RemoveEditorPane(pane);
            }
        }

        private void CurveEditorClosed(object sender, EventArgs eventArgs)
        {
            RemoveCurveEditor(true);
        }

        /// <inheritdoc/>
        [NotNull]
        public Task OpenAssetEditorWindow([NotNull] AssetViewModel asset)
        {
            return OpenAssetEditorWindow(asset, true);
        }

        /// <inheritdoc/>
        public bool CloseAllEditorWindows(bool? save)
        {
            // Attempt to close all opened assets
            if (!openedAssets.ToList().All(asset => CloseAssetEditorWindow(asset, save)))
                return false;

            // Then check that they are no remaining editor
            if (assetEditors.Count > 0)
            {
                // Nicolas: this case should not happen. Please let me know if it happens to you.
                // Note: this likely means that some editors leaked (esp. in the case of multi-asset editors), but force removing should be enough.
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();

                assetEditors.Keys.ToList().ForEach(RemoveEditor);
            }

            CloseCurveEditorWindow();

            return true;
        }

        /// <inheritdoc/>
        public void CloseAllHiddenWindows()
        {
 /*           foreach (var pane in AvalonDockHelper.GetAllAnchorables(dockingLayoutManager.DockingManager).Where(p => string.IsNullOrEmpty(p.ContentId) && p.IsHidden).ToList())
            {
                CleanEditorPane(pane);
                RemoveEditorPane(pane);
            }*/
        }

        /// <inheritdoc/>
        public bool CloseAssetEditorWindow([NotNull] AssetViewModel asset, bool? save)
        {
            var canClose = asset.Editor?.PreviewClose(save) ?? true;
            if (canClose)
                CloseEditorWindow(asset);

            return canClose;
        }

        /// <inheritdoc/>
        public void HideAllAssetEditorWindows()
        {
            foreach (var editorPane in assetEditors.Values)
            {
//                editorPane.Hide();
            }
        }

        /// <summary>
        /// Retrieves the list of all assets that are currently opened in an editor.
        /// </summary>
        /// <returns>A list of all assets currently opened.</returns>
        /// <remarks>
        /// This includes all assets in <see cref="IMultipleAssetEditorViewModel"/> even those that were not explicitly opened.
        /// </remarks>
        /// <seealso cref="OpenedAssets"/>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal IReadOnlyCollection<AssetViewModel> GetCurrentlyOpenedAssets()
        {
            var hashSet = new HashSet<AssetViewModel>(openedAssets);
            assetEditors.Keys.OfType<IMultipleAssetEditorViewModel>().ForEach(x => hashSet.AddRange(x.OpenedAssets));
            return hashSet;
        }

        /// <summary>
        /// Opens (and activates) an editor window for the given asset. If an editor window for this asset already exists, simply activates it.
        /// </summary>
        /// <param name="asset">The asset for which to show an editor window.</param>
        /// <param name="saveSettings">True if <see cref="MRUAdditionalData.OpenedAssets"/> should be updated, false otherwise. Note that if the editor fail to load it will not be updated.</param>
        /// <returns></returns>
        internal async Task OpenAssetEditorWindow([NotNull] AssetViewModel asset, bool saveSettings)
        {
            if (asset == null) throw new ArgumentNullException(nameof(asset));
            if (dockingLayoutManager == null) throw new InvalidOperationException("This method can only be invoked on the IEditorDialogService that has the editor main window as parent.");

            // Switch to the editor layout before adding any Pane
            if (assetEditors.Count == 0)
            {
                dockingLayoutManager.SwitchToEditorLayout();
            }

            using (await mutex.LockAsync())
            {
                ALayoutAnchorable editorPane = null;
                IEditorView view;
                // Asset already has an editor? Then, Look for the corresponding panel
                if (asset.Editor != null && !assetEditors.TryGetValue(asset.Editor, out editorPane))
                {
                    // Inconsistency, clean leaking editor
                    RemoveAssetEditor(asset);
                    // Try to find if another editor currently has this asset
                    var editor = assetEditors.Keys.OfType<IMultipleAssetEditorViewModel>().FirstOrDefault(x => x.OpenedAssets.Contains(asset));
                    if (editor != null)
                    {
                        editorPane = assetEditors[editor];
                        asset.Editor = editor;
                    }
                }
                // Existing editor?
                if (editorPane != null)
                {
                    // Make the pane visible immediately
                    MakeActiveVisible(editorPane);
                    view = editorPane.Content as IEditorView;
                    if (view?.EditorInitialization != null)
                    {
                        // Wait for the end of the initialization
                        await view.EditorInitialization;
                    }
                    return;
                }

                // Create a new editor view
                view = asset.ServiceProvider.Get<IAssetsPluginService>().ConstructEditionView(asset);
                if (view != null)
                {
                    // Pane may already exists (e.g. created from layout saving)
//                    editorPane = AvalonDockHelper.GetAllAnchorables(dockingLayoutManager.DockingManager).FirstOrDefault(p => p.Title == asset.Url);
                    if (editorPane == null)
                    {
                        //editorPane = new LayoutAnchorable { CanClose = true };
                        editorPane = new ALayoutAnchorable { Title = $"Document999", Content = view };
                        //                    editorPane.AttachDevTools();
                        // Stack the asset in the dictionary of editor to prevent double-opening while double-clicking twice on the asset, since the initialization is async
                        //                        AvalonDockHelper.GetDocumentPane(dockingLayoutManager.DockingManager).Children.Add(editorPane);
                        
                        var x = dockingLayoutManager.DockingManager.Layout.VisibleDockables;
                        //                        dockingLayoutManager.DockingManager.Layout.Factory.AddDockable (dockingLayoutManager.DockingManager.Layout, editorPane);


                        //                        var d = dockingLayoutManager.DockingManager.Factory.VisibleDockableControls[dockingLayoutManager.DockingManager.Factory.VisibleDockableControls.Keys.First()];
                        DocumentDock d = (DocumentDock)((ProportionalDock)((ProportionalDock)dockingLayoutManager.DockingManager.Layout.VisibleDockables[0]).VisibleDockables[0]).VisibleDockables[0];
                        ProportionalDock d1 = (ProportionalDock)((ProportionalDock)dockingLayoutManager.DockingManager.Layout.VisibleDockables[0]).VisibleDockables[0];
                        var e1 = new ALayoutAnchorablePane
                        {
                            Title = $"Document777",
                            //     Content = view,
                        };
                        var document = new Document
                        {
                            Title = $"Document888",
                       //     Content = view,
                        };

                        //                        var e = dockingLayoutManager.DockingManager.Layout.VisibleDockables[0];
                        d1.Factory.RemoveDockable(d1.VisibleDockables[0], true); 
                        d1.Factory?.InsertDockable(d1, e1, 0);
                        d1.Factory.OnDockableAdded(e1);

                        e1.Factory?.AddDockable(e1, editorPane);
                        e1.Factory.OnDockableAdded(editorPane);
                        e1.Factory?.AddDockable(e1, document);
                        e1.Factory.OnDockableAdded(document);


                        //                        d.Factory?.AddDockable(d, document);
                        //                        d.Factory?.AddDockable(d, editorPane);
                        //                        d.Factory.RemoveDockable(d.VisibleDockables[0], true);
                        /*                      var f = new ProportionalDock();
                                              e.Factory?.InsertDockable((ProportionalDock)e, f, 1);

                                              var e1 = new DocumentDock
                                              {
                                                  Title = $"Document777",
                                                  //     Content = view,
                                              };
                                              f.Factory?.InsertDockable(((ProportionalDock)f), e1, 0);
                                              f.Factory?.SetActiveDockable(e1);
                                              f.Factory?.SetFocusedDockable(dockingLayoutManager.DockingManager.Layout, e1);
                                              e1.Factory?.AddDockable((e1), document);
                                              e1.Factory?.SetActiveDockable(document);
                                              e1.Factory?.SetFocusedDockable(dockingLayoutManager.DockingManager.Layout, document);*/

                        //             d.Factory?.SetActiveDockable(document);
                        //                        d.Factory?.SetFocusedDockable(dockingLayoutManager.DockingManager.Layout, document);

                        // document.Content = view;
                    }
                    //                    editorPane.IsActiveChanged += EditorPaneIsActiveChanged;
                    //                   editorPane.IsSelectedChanged += EditorPaneIsSelectedChanged;
                    //                                        editorPane.Factory.ActiveDockableChanged += EditorPaneIsActiveChanged;
                    //                    editorPane.Factory.WindowClosing += EditorPaneClosing;
                    //                                        editorPane.Factory.DockableClosed += EditorPaneClosed;
                    //                                        editorPane.Content = view;
                    // Make the pane visible immediately
                    MakeActiveVisible(editorPane);
                    // Initialize the editor view
                    view.DataContext = asset;

                            // Create a binding for the title
                            var binding = new Binding(nameof(AssetViewModel.Url)) { Mode = BindingMode.OneWay, Source = asset };
//                    BindingOperations.SetBinding(editorPane, LayoutContent.TitleProperty, binding);

                    var viewModel = await view.InitializeEditor(asset);
                    if (viewModel == null)
                    {
                        // Could not initialize editor
                        CleanEditorPane(editorPane);
                        RemoveEditorPane(editorPane);
                    }
                    else
                    {
                        assetEditors[viewModel] = editorPane;
                        openedAssets.Add(asset);
                        if (viewModel is IMultipleAssetEditorViewModel multiEditor)
                        {
                            foreach (var item in multiEditor.OpenedAssets)
                            {
                                if (item.Editor != null)
                                {
                                    // Note: this could happen in some case after undo/redo that involves parenting of scenes
                                    RemoveAssetEditor(item);
                                }
                                item.Editor = viewModel;
                            }
                            NotifyCollectionChangedEventHandler handler = (_, e) => MultiEditorOpenAssetsChanged(multiEditor, e);
                            registeredHandlers.Add(multiEditor, handler);
                            multiEditor.OpenedAssets.CollectionChanged += handler;
                        }
                        else
                        {
                            asset.Editor = viewModel;
                        }
                    }
                }
            }

            // If the opening of the editor failed, go back to normal layout
            if (assetEditors.Count == 0)
            {
                dockingLayoutManager.SwitchToNormalLayout();
                return;
            }

            if (saveSettings)
            {
                dockingLayoutManager.SaveOpenAssets(OpenedAssets);
            }
        }

        private void CloseEditorWindow([NotNull] AssetViewModel asset)
        {
            // make asset view active
            asset.Session.ActiveProperties = asset.Session.AssetViewProperties;
            // remove editor
            RemoveAssetEditor(asset);
            // if no more editor open, change layout
            if (assetEditors.Count == 0)
            {
                dockingLayoutManager.SwitchToNormalLayout();
            }
        }

        private void MultiEditorOpenAssetsChanged([NotNull] IMultipleAssetEditorViewModel multiEditor, [NotNull] NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems?.Count > 0)
                    {
                        foreach (AssetViewModel item in e.OldItems)
                        {
                            item.Editor = null;
                        }
                    }
                    if (e.NewItems?.Count > 0)
                    {
                        foreach (AssetViewModel item in e.NewItems)
                        {
                            if (item.Editor != null && assetEditors.ContainsKey(item.Editor))
                            {
                                RemoveAssetEditor(item);
                            }
                            item.Editor = multiEditor;
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    // nothing to do
                    break;
                case NotifyCollectionChangedAction.Reset:
                    throw new InvalidOperationException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Removes the editor for the given <paramref name="asset"/>.
        /// </summary>
        /// <param name="asset">The asset.</param>
        private void RemoveAssetEditor([NotNull] AssetViewModel asset)
        {
            openedAssets.Remove(asset);

            var editor = asset.Editor;
            if (editor == null)
                return;

            asset.Editor = null;
            RemoveEditor(editor);
        }

        private void RemoveEditor([NotNull] IAssetEditorViewModel editor)
        {
            assetEditors.TryGetValue(editor, out var editorPane);

            if (editor is IMultipleAssetEditorViewModel multiEditor)
            {
                if (registeredHandlers.TryGetValue(multiEditor, out var handler))
                {
                    multiEditor.OpenedAssets.CollectionChanged -= handler;
                    registeredHandlers.Remove(multiEditor);
                }
                else
                {
                    throw new InvalidOperationException($"Expected {multiEditor} to have a handler set up");
                }

                // Clean asset view models
                foreach (var item in multiEditor.OpenedAssets)
                {
                    item.Editor = null;
                }
            }
            // Remove editor
            assetEditors.Remove(editor);
            // Attempt to destroy the editor
            try
            {
                editor.Destroy();
            }
            catch (ObjectDisposedException)
            {
            }
            // Clean and remove editor pane
            if (editorPane != null)
            {
                CleanEditorPane(editorPane);
                RemoveEditorPane(editorPane);
            }
        }

        private void AssetsDeleted(object sender, [NotNull] NotifyCollectionChangedEventArgs e)
        {
            e.NewItems?.Cast<AssetViewModel>().Where(x => openedAssets.Contains(x)).ForEach(CloseEditorWindow);
        }

        /// <summary>
        /// Cleans the editor pane.
        /// </summary>
        /// <param name="editorPane">The editor pane.</param>
        /// <seealso cref="RemoveEditorPane"/>
        private static void CleanEditorPane([NotNull] ALayoutAnchorable editorPane)
        {
            // Destroy the editor view
            (editorPane.Content as IDestroyable)?.Destroy();
            editorPane.Content = null;
            editorPane.Title = null;
        }

        /// <summary>
        /// Removes the editor pane.
        /// </summary>
        /// <param name="editorPane">The editor pane.</param>
        /// <seealso cref="CleanEditorPane"/>
        private void RemoveEditorPane([NotNull] ALayoutAnchorable editorPane)
        {
 //           editorPane.IsActiveChanged -= EditorPaneIsActiveChanged;
 //           editorPane.IsSelectedChanged -= EditorPaneIsSelectedChanged;
            editorPane.Factory.WindowClosing -= EditorPaneClosing;
            editorPane.Factory.DockableClosed -= EditorPaneClosed;

            // If this editor pane was closed by user, no need to do that; it is necessary for closing programmatically
 //           if (editorPane.Root != null)
//                editorPane.Close();
        }

        private static void EditorPaneClosing(object sender, Dock.Model.Core.Events.WindowClosingEventArgs e)
        {
            var editorPane = (ALayoutAnchorable)sender;

            var element = editorPane.Content as Control;
            var asset = element?.DataContext as AssetViewModel;

            // If any editor couldn't close, cancel the sequence
            if (!(asset?.Editor?.PreviewClose(null) ?? true))
            {
                e.Cancel = true;
            }
        }

        private void EditorPaneClosed(object sender, EventArgs eventArgs)
        {
            var editorPane = (ALayoutAnchorable)sender;

            var element = editorPane.Content as Control;
            if (element?.DataContext is AssetViewModel asset)
            {
                CloseEditorWindow(asset);
            }
        }

        private static void EditorPaneContentLoaded(object sender, RoutedEventArgs e)
        {
            // Give focus to element
            var element = (Control)sender;
 //           if (!element.IsKeyboardFocusWithin)
 //               Keyboard.Focus(element);
        }

        private static void EditorPaneIsActiveChanged(object sender, EventArgs e)
        {
            var editorPane = (ALayoutAnchorable)sender;

            if (editorPane.Content is Control element)
            {
                // FIXME Gets set after call - need a handler for deactivate for else condition.
//                if (editorPane.IsActive)
                {
                    if (element.IsLoaded)
                    {
                        // Give focus to element
                        //                        if (!element.IsKeyboardFocusWithin)
                        //                            Keyboard.Focus(element);
                        var assetViewModel = element?.DataContext as AssetViewModel;
                        if (assetViewModel?.Editor is Assets.Presentation.AssetEditors.GameEditor.ViewModels.GameEditorViewModel gameEditor)
                        {
                            // A tab/sub-window is visible via IsSelected, not IsVisible
                                gameEditor.ShowGame();
                        }

                        }
                        else
                    {
                        // Not loaded yet, let's defer the focus until loaded
                        element.Loaded += EditorPaneContentLoaded;
                    }
                }
/*                else
                {
                    element.Loaded -= EditorPaneContentLoaded;
                }*/
            }
        }

        /*       private static void EditorPaneIsSelectedChanged(object sender, EventArgs e)
               {
                   var editorPane = (ALayoutAnchorable)sender;

                   if (editorPane.Content is Control element)
                   {
                       var assetViewModel = element?.DataContext as AssetViewModel;
                       if (assetViewModel?.Editor is Assets.Presentation.AssetEditors.GameEditor.ViewModels.GameEditorViewModel gameEditor)
                       {
                           // A tab/sub-window is visible via IsSelected, not IsVisible
                           if (editorPane.IsSelected)
                           {
                               gameEditor.ShowGame();
                           }
                           else
                           {
                               gameEditor.HideGame();
                           }
                       }
                   }
               }*/

        /// <summary>
        /// Makes the editor pane active and visible.
        /// </summary>
        /// <param name="editorPane"></param>
        private static void MakeActiveVisible([NotNull] ALayoutAnchorable editorPane)
        {
//            editorPane.IsActive = true;
//            editorPane.Show();
//            editorPane.Activate ();
        }
    }
}
