// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
using Avalonia.Media.Imaging;
using Stride.Core.Storage;
using Stride.Core.Presentation.Services;
using Stride.Core.Presentation.ViewModel;
using Avalonia.Media;

namespace Stride.Core.Assets.Editor.Services
{
    public abstract class AThumbnailData : IThumbnailData
    {
        protected readonly ObjectId thumbnailId;
        private static readonly Dictionary<ObjectId, Task<IImage>> ComputingThumbnails = new Dictionary<ObjectId, Task<IImage>>();
        private object presenter;

        protected AThumbnailData(ObjectId thumbnailId)
        {
            this.thumbnailId = thumbnailId;
        }

        public object Presenter { get { return presenter; } set { SetValue(ref presenter, value); } }

        public override async Task PrepareForPresentation(IDispatcherService dispatcher)
        {
            Task<IImage> task;
            lock (ComputingThumbnails)
            {
                if (!ComputingThumbnails.TryGetValue(thumbnailId, out task))
                {
                    task = Task.Run(() => BuildImageSource());
                    ComputingThumbnails.Add(thumbnailId, task);
                }
            }

            var result = await task;
            dispatcher.Invoke(() => Presenter = result);
            FreeBuildingResources();

            lock (ComputingThumbnails)
            {
                ComputingThumbnails.Remove(thumbnailId);
            }
        }

        /// <summary>
        /// Fetches and prepare the image source instance to be displayed.
        /// </summary>
        /// <returns></returns>
        protected abstract IImage BuildImageSource();

        /// <summary>
        /// Clears the resources required to build the image source.
        /// </summary>
        protected override abstract void FreeBuildingResources();
    }

    /// <summary>
    /// Generic ImageSource resources, DrawingImage vectors, etc. support for thumbnails.
    /// </summary>
    public sealed class AResourceThumbnailData : AThumbnailData
    {
        object resourceKey;
        
        /// <param name="resourceKey">The key used to fetch the resource, most likely a string.</param>
        public AResourceThumbnailData(ObjectId thumbnailId, object resourceKey) : base(thumbnailId)
        {
            this.resourceKey = resourceKey;
        }

        /// <inheritdoc />
        protected override IImage BuildImageSource()
        {
            if (resourceKey == null)
                return null;
            try
            {
                return System.Windows.Application.Current.TryFindResource(resourceKey) as IImage;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <inheritdoc />
        protected override void FreeBuildingResources()
        {
            resourceKey = null;
        }
    }

    /// <summary>
    /// Byte streams bitmap support for thumbnails.
    /// </summary>
    public sealed class ABitmapThumbnailData : AThumbnailData
    {
        private static readonly ObjectCache<ObjectId, IImage> Cache = new ObjectCache<ObjectId, IImage>(512);
        private Stream thumbnailBitmapStream;

        public ABitmapThumbnailData(ObjectId thumbnailId, Stream thumbnailBitmapStream) : base(thumbnailId)
        {
            this.thumbnailBitmapStream = thumbnailBitmapStream;
        }

        /// <inheritdoc />
        protected override IImage BuildImageSource()
        {
            return BuildAsBitmapImage(thumbnailId, thumbnailBitmapStream);
        }

        /// <inheritdoc />
        protected override void FreeBuildingResources()
        {
            thumbnailBitmapStream = null;
        }

        private static IImage BuildAsBitmapImage(ObjectId thumbnailId, Stream thumbnailStream)
        {
            if (thumbnailStream == null)
                return null;

            var stream = thumbnailStream;
            if (!stream.CanRead)
                return null;

            var result = Cache.TryGet(thumbnailId);
            if (result != null)
                return result;

            try
            {
                stream.Position = 0;
                var bitmap = new Avalonia.Media.Imaging.Bitmap(stream);
  /*              
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();*/
                thumbnailStream.Close();
                Cache.Cache(thumbnailId, bitmap);
                return bitmap;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
