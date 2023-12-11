// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.Linq;
using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Interop;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Stride.Core.Presentation.Interop;
using Stride.Core.Presentation.Services;
using Stride.Core.Presentation.Windows;

namespace Stride.Core.Presentation.Controls
{
    public abstract class AModalWindow : Window, IModalDialogInternal
    {
        public virtual async Task<DialogResult> ShowModal()
        {
			Loaded += (sender, e) =>
            {
                // Disable minimize on modal windows
/*                var handle = new WindowInteropHelper(this).Handle;
                if (handle != IntPtr.Zero)
                {
                    NativeHelper.DisableMinimizeButton(handle);
                }*/
            };
            Owner = AWindowManager.MainWindow?.Window ?? AWindowManager.BlockingWindows.LastOrDefault()?.Window;
            WindowStartupLocation = Owner != null ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen;
            await Dispatcher.UIThread.InvokeAsync(() => ShowDialog (Owner as Window));
            return Result;
        }

        public void RequestClose(DialogResult result)
        {
            Result = result;
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (Result == Services.DialogResult.None)
                Result = Services.DialogResult.Cancel;
        }

        public DialogResult Result { get; set; } = Services.DialogResult.None;
    }
}
