using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.WPF.Plugins;
using Newtonsoft.Json.Linq;
using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text.Json;

namespace Mutagen.Bethesda.WPF.Reflection.Fields;

public class EnumerableModKeySettingsVM : SettingsNodeVM
{
    private readonly ModKey[] _defaultVal;
    public ObservableCollection<ModKey> Values { get; } = new();
    public IObservable<IChangeSet<ModKey>> DetectedLoadOrder { get; }

    public EnumerableModKeySettingsVM(
        IObservable<IChangeSet<ModKey>> detectedLoadOrder,
        FieldMeta fieldMeta,
        IEnumerable<ModKey> defaultVal)
        : base(fieldMeta)
    {
        _defaultVal = defaultVal.ToArray();
        DetectedLoadOrder = detectedLoadOrder;
        Values.SetTo(_defaultVal);
    }

    public static EnumerableModKeySettingsVM Factory(ReflectionSettingsParameters param, FieldMeta fieldMeta, object? defaultVal)
    {
        var defaultKeys = new List<ModKey>();
        if (defaultVal is IEnumerable e)
        {
            foreach (var item in e)
            {
                defaultKeys.Add(ModKey.FromNameAndExtension(item.ToString()));
            }
        }
        return new EnumerableModKeySettingsVM(
            param.DetectedLoadOrder.Transform(x => x.ModKey),
            fieldMeta,
            defaultKeys);
    }

    public override void Import(JsonElement property, Action<string> logger)
    {
        Values.Clear();
        foreach (var elem in property.EnumerateArray())
        {
            if (ModKey.TryFromNameAndExtension(elem.GetString(), out var modKey))
            {
                Values.Add(modKey);
            }
            else
            {
                Values.Add(ModKey.Null);
            }
        }
    }

    public override void Persist(JObject obj, Action<string> logger)
    {
        obj[Meta.DiskName] = new JArray(Values
            .Select(x =>
            {
                if (x.IsNull)
                {
                    return string.Empty;
                }
                else
                {
                    return x.ToString();
                }
            }).ToArray());
    }

    public override SettingsNodeVM Duplicate()
    {
        return new EnumerableModKeySettingsVM(DetectedLoadOrder, Meta, _defaultVal);
    }
}