using Loqui.Translators;
using Noggog;
using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Binary
{
    public class BinaryTranslator : Translator<IBinaryTranslation<object, object>>
    {
        public readonly static BinaryTranslator Instance = new BinaryTranslator();

        public BinaryTranslator() 
            : base(
                  nullTranslator: null,
                  genericCaster: typeof(BinaryTranslationCaster<,>),
                  loquiTranslation: typeof(LoquiBinaryTranslation<,>), 
                  enumTranslation: null)
        {
        }
    }

    public class BinaryTranslator<T, M>
    {
        private static NotifyingItem<GetResponse<IBinaryTranslation<T, M>>> _translator = new NotifyingItem<GetResponse<IBinaryTranslation<T, M>>>();
        public static INotifyingItemGetter<GetResponse<IBinaryTranslation<T, M>>> Translator => _translator;

        static BinaryTranslator()
        {
            var transl = BinaryTranslator.Instance.GetTranslator(typeof(T));
            transl.Subscribe(
                _translator,
                (change) =>
                {
                    if (change.New.Failed)
                    {
                        _translator.Item = change.New.BubbleFailure<IBinaryTranslation<T, M>>();
                        return;
                    }
                    var caster = change.New.Value as BinaryTranslationCaster<T, M>;
                    _translator.Item = GetResponse<IBinaryTranslation<T, M>>.Succeed(caster.Source);
                });
        }
    }
}
