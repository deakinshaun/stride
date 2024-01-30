// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using Stride.Core.Presentation.ViewModel;

namespace Stride.Core.Assets.Editor.Services
{
    public class ThumbnailCompletedArgs : EventArgs
    {
        public ThumbnailCompletedArgs(AssetId assetId, IThumbnailData data)
        {
            AssetId = assetId;
            Data = data;
        }

        public AssetId AssetId { get; private set; }

        public IThumbnailData Data { get; private set; }
    }
}
