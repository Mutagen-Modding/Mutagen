using Loqui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using static Loqui.EqualsMaskHelper;

namespace Mutagen.Bethesda
{
    public interface IGenderedItemGetter<out T> : IEnumerable<T>, IPrintable
    {
        T Male { get; }
        T Female { get; }
    }

    public class GenderedItem<T> : IGenderedItemGetter<T>
    {
        public T Male { get; set; }
        public T Female { get; set; }

        public GenderedItem(T male, T female)
        {
            this.Male = male;
            this.Female = female;
        }

        public IEnumerator<T> GetEnumerator()
        {
            yield return Male;
            yield return Female;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public void ToString(FileGeneration fg, string? name)
        {
            fg.AppendLine($"{name} =>");
            fg.AppendLine("[");
            using (new DepthWrapper(fg))
            {
                if (this.Male is IPrintable mp)
                {
                    mp.ToString(fg, "Male");
                }
                else if (this.Male != null)
                {
                    fg.AppendLine($"Male => {this.Male}");
                }
                if (this.Female is IPrintable fp)
                {
                    fp.ToString(fg, "Female");
                }
                else if (this.Female != null)
                {
                    fg.AppendLine($"Female => {this.Female}");
                }
            }
            fg.AppendLine("]");
        }
    }

    public static class GenderedItem
    {
        public static GenderedItem<Exception?>? Combine(GenderedItem<Exception?>? lhs, GenderedItem<Exception?>? rhs)
        {
            if (lhs == null) return rhs;
            if (rhs == null) return lhs;
            return new GenderedItem<Exception?>(
                lhs.Male ?? rhs.Male,
                lhs.Female ?? rhs.Female);
        }

        public static MaskItem<bool, GenderedItem<TMask?>?>? EqualityMaskHelper<TItem, TMask>(
            IGenderedItemGetter<TItem>? lhs,
            IGenderedItemGetter<TItem>? rhs,
            Func<TItem, TItem, Include, TMask> maskGetter,
            EqualsMaskHelper.Include include)
            where TMask : class, IMask<bool>
        {
            if (lhs == null || rhs == null)
            {
                if (lhs == null && rhs == null)
                {
                    return include == Include.All ? new MaskItem<bool, GenderedItem<TMask?>?>(true, default) : null;
                }
                return new MaskItem<bool, GenderedItem<TMask?>?>(false, default);
            }
            else
            {
                var maleMask = maskGetter(lhs.Male, rhs.Male, include);
                var femaleMask = maskGetter(lhs.Female, rhs.Female, include);
                var maleEqual = maleMask.AllEqual((b) => b);
                var femaleEqual = femaleMask.AllEqual((b) => b);
                var overall = maleEqual && femaleEqual;
                if (!overall || include == Include.All)
                {
                    return new MaskItem<bool, GenderedItem<TMask?>?>(
                        overall,
                        new GenderedItem<TMask?>(
                            male: !maleEqual || include == Include.All ? maleMask : null,
                            female: !femaleEqual || include == Include.All ? femaleMask : null));
                }
                return null;
            }
        }

        public static MaskItem<bool, GenderedItem<bool>?>? EqualityMaskHelper<TItem>(
            IGenderedItemGetter<TItem>? lhs,
            IGenderedItemGetter<TItem>? rhs,
            Func<TItem, TItem, Include, bool> maskGetter,
            EqualsMaskHelper.Include include)
        {
            if (lhs == null || rhs == null)
            {
                if (lhs == null && rhs == null)
                {
                    return include == Include.All ? new MaskItem<bool, GenderedItem<bool>?>(true, default) : null;
                }
                return new MaskItem<bool, GenderedItem<bool>?>(false, default);
            }
            else
            {
                var maleMask = maskGetter(lhs.Male, rhs.Male, include);
                var femaleMask = maskGetter(lhs.Female, rhs.Female, include);
                bool overall = maleMask == femaleMask;
                if (!overall || include == Include.All)
                {
                    return new MaskItem<bool, GenderedItem<bool>?>(
                        overall,
                        new GenderedItem<bool>(maleMask, femaleMask));
                }
                return null;
            }
        }

        public static bool AllEqualMask<TItem, TMask>(MaskItem<TItem, GenderedItem<TMask?>?>? item, Func<TItem, bool> eval)
            where TMask : class, IMask<TItem>
        {
            if (item == null) return true;
            if (!eval(item.Overall)) return false;
            if (item.Specific == null) return true;
            if (item.Specific.Male != null && !(item.Specific.Male.AllEqual(eval))) return false;
            if (item.Specific.Female != null && !(item.Specific.Female.AllEqual(eval))) return false;
            return true;
        }

        public static bool AllEqual<TItem>(MaskItem<TItem, GenderedItem<TItem>?>? item, Func<TItem, bool> eval)
        {
            if (item == null) return true;
            if (!eval(item.Overall)) return false;
            if (item.Specific == null) return true;
            if (!eval(item.Specific.Male)) return false;
            if (!eval(item.Specific.Female)) return false;
            return true;
        }
    }
}
