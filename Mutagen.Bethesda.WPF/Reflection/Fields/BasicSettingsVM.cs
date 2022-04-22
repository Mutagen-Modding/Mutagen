using Newtonsoft.Json.Linq;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Text.Json;

namespace Mutagen.Bethesda.WPF.Reflection.Fields;

public interface IBasicSettingsNodeVM : INotifyPropertyChanged
{
    string DisplayName { get; }

    object Value { get; }

    bool IsSelected { get; set; }

    void WrapUp();
}

public abstract class BasicSettingsVM<T> : SettingsNodeVM, IBasicSettingsNodeVM
{
    public T DefaultValue { get; }

    [Reactive]
    public T Value { get; set; }

    object IBasicSettingsNodeVM.Value => this.Value!;

    [Reactive]
    public bool IsSelected { get; set; }

    private readonly ObservableAsPropertyHelper<string> _DisplayName;
    public string DisplayName => _DisplayName.Value;

    public BasicSettingsVM(FieldMeta fieldMeta, object? defaultVal)
        : base(fieldMeta)
    {
        if (defaultVal is T item)
        {
            Value = item;
            DefaultValue = item;
        }
        else
        {
            Value = default!;
            DefaultValue = default!;
        }
        _DisplayName = this.WhenAnyValue(x => x.Value)
            .Select(x => x?.ToString() ?? "Name")
            .ToGuiProperty(this, nameof(DisplayName), string.Empty, deferSubscription: true);
    }

    public override void Import(JsonElement property, Action<string> logger)
    {
        Value = Get(property);
    }

    public override void Persist(JObject obj, Action<string> logger)
    {
        if (Value == null) return;
        obj[Meta.DiskName] = JToken.FromObject(Value);
    }

    public abstract T Get(JsonElement property);

    public abstract T GetDefault();
}

public class UnknownBasicSettingsVM : ViewModel, IBasicSettingsNodeVM
{
    public static readonly UnknownBasicSettingsVM Empty = new();

    public object Value => "Unknown";

    public bool IsSelected { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public string DisplayName => "Unknown";

    public void WrapUp()
    {
        throw new System.NotImplementedException();
    }
}