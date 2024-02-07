// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Stride.Core.Assets.Editor.View;
using Stride.Core.Extensions;
using Stride.Core.Presentation.Services;
using Stride.GameStudio.View;

namespace Stride.GameStudio.Services
{
    public class AStrideDialogService : AEditorDialogService, IStrideDialogService
    {
        public AStrideDialogService(IDispatcherService dispatcher, string applicationName)
            : base(dispatcher, applicationName)
        {
        }

        public ICredentialsDialog CreateCredentialsDialog()
        {
            //           return new CredentialsDialog(this);
            return null;
        }

        public void ShowAboutPage()
        {
 //           var page = new AboutPage(this);
 //           page.ShowModal().Forget();
        }

    }
}
