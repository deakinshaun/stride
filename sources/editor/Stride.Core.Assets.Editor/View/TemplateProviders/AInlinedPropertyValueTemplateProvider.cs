// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Stride.Core.Presentation.Quantum;
using Stride.Core.Presentation.Quantum.View;
using Stride.Core.Presentation.Quantum.ViewModels;

namespace Stride.Core.Assets.Editor.View.TemplateProviders
{
    public class AInlinedPropertyValueTemplateProvider : ANodeViewModelTemplateProvider
    {
        public override string Name => "InlinedPropertyValue";

        public override bool MatchNode(NodeViewModel node)
        {
            return node.Name == "InlinedProperty";
        }
    }
}
