// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stride.Core.Assets.Editor.Extensions;
using Stride.Core.Assets.Editor.ViewModel;
using Stride.Core;
using Stride.Core.Diagnostics;
using Stride.Core.Extensions;
using Stride.Core.Presentation.View;

namespace Stride.Core.Assets.Editor.Services
{
    public class APluginService : PluginService
    {
        public override IEditorView ConstructEditionView(AssetViewModel asset)
        {
            if (asset == null) throw new ArgumentNullException(nameof(asset));
            if (asset.Session == null) throw new ArgumentException(@"The asset is currently deleted.", nameof(asset));

            var currentMatch = new KeyValuePair<Type, Type>();
            foreach (var keyValuePair in assetEditorViewModelTypes)
            {
                // Exact type match, we stop here.
                if (keyValuePair.Key == asset.AssetType)
                {
                    currentMatch = keyValuePair;
                    break;
                }
                // Parent type...
                if (keyValuePair.Key.IsAssignableFrom(asset.AssetType))
                {
                    // ... we keep it only if we have no match yet, or if it is closer to the asset type in the inheritance hierarchy
                    if (currentMatch.Key == null || currentMatch.Key.IsAssignableFrom(keyValuePair.Key))
                    {
                        currentMatch = keyValuePair;
                    }
                }
            }

            if (currentMatch.Key == null)
                return null;

            var attribute = currentMatch.Value.GetCustomAttribute<AAssetEditorViewModelAttribute>();
            if (attribute == null)
                return null;

            return (IEditorView)Activator.CreateInstance(attribute.EditorViewType);
        }
    }
}
