using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using Avalonia.Styling;
using Dock.Model.Avalonia.Core;
using Dock.Model.Controls;

namespace Stride.GameStudio.AssetsEditors;

/// <summary>
/// Document.
/// </summary>
[DataContract(IsReference = true)]
public class ALayoutAnchorable : DockableBase, IDocument, IDocumentContent, ITemplate<Control?>, IRecyclingDataTemplate
{
    /// <summary>
    /// Defines the <see cref="Content"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<ALayoutAnchorable, object?>(nameof(Content));

    /// <summary>
    /// Initializes new instance of the <see cref="Document"/> class.
    /// </summary>
    public ALayoutAnchorable()
    {
    }

    /// <summary>
    /// Gets or sets the content to display.
    /// </summary>
    [Content]
    [TemplateContent]
    [ResolveByName]
    [IgnoreDataMember]
    [JsonIgnore]
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public Type? DataType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Control? Build()
    {
        // return Dock.Model.Avalonia.Controls.TemplateHelper.Load(Content)?.Result;
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    object? ITemplate.Build() => Build();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool Match(object? data)
    {
        if (DataType == null)
        {
            return true;
        }

        return DataType.IsInstanceOfType(data);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public Control? Build(object? data) => Build(data, null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="existing"></param>
    /// <returns></returns>
    public Control? Build(object? data, Control? existing)
    {
        //return TemplateHelper.Build(Content, this);
        return (Control) Content;
    }
}
