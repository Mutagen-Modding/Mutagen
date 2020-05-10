using Loqui;
using Mutagen.Bethesda.Internals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using static Loqui.EqualsMaskHelper;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface for an object exposing readonly Gendered Items
    /// </summary>
    public interface IGenderedItemGetter<out T> : IEnumerable<T>, IPrintable
    {
        /// <summary>
        /// Male item
        /// </summary>
        T Male { get; }
        
        /// <summary>
        /// Female item
        /// </summary>
        T Female { get; }
    }

    /// <summary>
    /// An object exposing data in a gendered format
    /// </summary>
    public class GenderedItem<T> : IGenderedItemGetter<T>
    {
        /// <summary>
        /// Male item
        /// </summary>
        public T Male { get; set; }
        
        /// <summary>
        /// Female item
        /// </summary>
        public T Female { get; set; }

        /// <summary>
        /// Constructor that takes a male and female item
        /// </summary>
        public GenderedItem(T male, T female)
        {
            this.Male = male;
            this.Female = female;
        }

        /// <summary>
        /// Enumerates first the male item, then the female item
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            yield return Male;
            yield return Female;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Prints the male and female items to the stream
        /// </summary>
        /// <param name="fg">Stream to print into</param>
        /// <param name="name">Optional name to include</param>
        public void ToString(FileGeneration fg, string? name)
        {
            GenderedItem.ToString(this, fg, name);
        }
    }

    namespace Internals
    {
        public static class GenderedItem
        {
            public static void ToString<TItem>(IGenderedItemGetter<TItem> item, FileGeneration fg, string? name)
            {
                fg.AppendLine($"{name} =>");
                fg.AppendLine("[");
                using (new DepthWrapper(fg))
                {
                    var male = item.Male;
                    if (male is IPrintable mp)
                    {
                        mp.ToString(fg, "Male");
                    }
                    else if (male != null)
                    {
                        fg.AppendLine($"Male => {male}");
                    }
                    var female = item.Female;
                    if (female is IPrintable fp)
                    {
                        fp.ToString(fg, "Female");
                    }
                    else if (female != null)
                    {
                        fg.AppendLine($"Female => {female}");
                    }
                }
                fg.AppendLine("]");
            }

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
                    var maleEqual = maleMask.All((b) => b);
                    var femaleEqual = femaleMask.All((b) => b);
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

            public static MaskItem<bool, GenderedItem<MaskItem<bool, TMask?>?>?>? EqualityMaskHelper<TItem, TMask>(
                IGenderedItemGetter<TItem>? lhs,
                IGenderedItemGetter<TItem>? rhs,
                Func<TItem, TItem, Include, MaskItem<bool, TMask?>?> maskGetter,
                EqualsMaskHelper.Include include)
                where TMask : class, IMask<bool>
            {
                if (lhs == null || rhs == null)
                {
                    if (lhs == null && rhs == null)
                    {
                        return include == Include.All ? new MaskItem<bool, GenderedItem<MaskItem<bool, TMask?>?>?>(true, default) : null;
                    }
                    return new MaskItem<bool, GenderedItem<MaskItem<bool, TMask?>?>?>(false, default);
                }
                else
                {
                    var maleMask = maskGetter(lhs.Male, rhs.Male, include);
                    var femaleMask = maskGetter(lhs.Female, rhs.Female, include);
                    var maleEqual = maleMask?.Overall ?? true;
                    var femaleEqual = femaleMask?.Overall ?? true;
                    var overall = maleEqual && femaleEqual;
                    if (!overall || include == Include.All)
                    {
                        return new MaskItem<bool, GenderedItem<MaskItem<bool, TMask?>?>?>(
                            overall,
                            new GenderedItem<MaskItem<bool, TMask?>?>(
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

            public static bool AllMask<TItem, TMask>(MaskItem<TItem, GenderedItem<TMask?>?>? item, Func<TItem, bool> eval)
                where TMask : class, IMask<TItem>
            {
                if (item == null) return true;
                if (!eval(item.Overall)) return false;
                if (item.Specific == null) return true;
                if (item.Specific.Male != null && !item.Specific.Male.All(eval)) return false;
                if (item.Specific.Female != null && !item.Specific.Female.All(eval)) return false;
                return true;
            }

            public static bool AllMask<TItem, TMask>(MaskItem<TItem, GenderedItem<MaskItem<TItem, TMask?>?>?>? item, Func<TItem, bool> eval)
                where TMask : class, IMask<TItem>
            {
                if (item == null) return true;
                if (!eval(item.Overall)) return false;
                if (item.Specific == null) return true;
                if (item.Specific.Male != null)
                {
                    if (!eval(item.Specific.Male.Overall)) return false;
                    if (item.Specific.Male.Specific != null && !item.Specific.Male.Specific.Any(eval)) return false;
                }
                if (item.Specific.Female != null)
                {
                    if (!eval(item.Specific.Female.Overall)) return false;
                    if (item.Specific.Female.Specific != null && !item.Specific.Female.Specific.Any(eval)) return false;
                }
                return true;
            }

            public static bool All<TItem>(MaskItem<TItem, GenderedItem<TItem>?>? item, Func<TItem, bool> eval)
            {
                if (item == null) return true;
                if (!eval(item.Overall)) return false;
                if (item.Specific == null) return true;
                if (!eval(item.Specific.Male)) return false;
                if (!eval(item.Specific.Female)) return false;
                return true;
            }

            public static bool AnyMask<TItem, TMask>(MaskItem<TItem, GenderedItem<TMask?>?>? item, Func<TItem, bool> eval)
                where TMask : class, IMask<TItem>
            {
                if (item == null) return false;
                if (eval(item.Overall)) return true;
                if (item.Specific == null) return false;
                if (item.Specific.Male != null && item.Specific.Male.Any(eval)) return true;
                if (item.Specific.Female != null && item.Specific.Female.Any(eval)) return true;
                return false;
            }

            public static bool AnyMask<TItem, TMask>(MaskItem<TItem, GenderedItem<MaskItem<TItem, TMask?>?>?>? item, Func<TItem, bool> eval)
                where TMask : class, IMask<TItem>
            {
                if (item == null) return false;
                if (eval(item.Overall)) return true;
                if (item.Specific == null) return false;
                if (item.Specific.Male != null)
                {
                    if (eval(item.Specific.Male.Overall)) return true;
                    if (item.Specific.Male.Specific != null && item.Specific.Male.Specific.Any(eval)) return true;
                }
                if (item.Specific.Female != null)
                {
                    if (eval(item.Specific.Female.Overall)) return true;
                    if (item.Specific.Female.Specific != null && item.Specific.Female.Specific.Any(eval)) return true;
                }
                return false;
            }

            public static bool Any<TItem>(MaskItem<TItem, GenderedItem<TItem>?>? item, Func<TItem, bool> eval)
            {
                if (item == null) return false;
                if (eval(item.Overall)) return true;
                if (item.Specific == null) return true;
                if (eval(item.Specific.Male)) return true;
                if (eval(item.Specific.Female)) return true;
                return false;
            }

            public static MaskItem<TRet, GenderedItem<TRet>?>? TranslateHelper<TItem, TRet>(MaskItem<TItem, GenderedItem<TItem>?>? mask, Func<TItem, TRet> eval)
            {
                if (mask == null)
                {
                    return null;
                }
                else
                {
                    return new MaskItem<TRet, GenderedItem<TRet>?>(
                        eval(mask.Overall),
                        mask.Specific == null ? null : new GenderedItem<TRet>(eval(mask.Specific.Male), eval(mask.Specific.Female)));
                }
            }

            public static MaskItem<TRet, GenderedItem<TRetMask?>?>? TranslateHelper<TItem, TMask, TRet, TRetMask>(
                MaskItem<TItem, GenderedItem<TMask?>?>? mask,
                Func<TItem, TRet> eval,
                Func<TMask?, Func<TItem, TRet>, TRetMask?> maskConv)
                where TMask : class, IMask<TItem>
                where TRetMask : class, IMask<TRet>
            {
                if (mask == null)
                {
                    return null;
                }
                else
                {
                    return new MaskItem<TRet, GenderedItem<TRetMask?>?>(
                        eval(mask.Overall),
                        mask.Specific == null ? null : new GenderedItem<TRetMask?>(maskConv(mask.Specific.Male, eval), maskConv(mask.Specific.Female, eval)));
                }
            }

            public static MaskItem<bool, GenderedItem<TMask?>?>? HasBeenSetHelper<TItem, TMask>(IGenderedItemGetter<TItem>? item, Func<TItem, TMask?> getter)
                where TMask : class, IMask<bool>
            {
                if (item == null)
                {
                    return null;
                }
                else
                {
                    return new MaskItem<bool, GenderedItem<TMask?>?>(
                        true,
                        new GenderedItem<TMask?>(
                            getter(item.Male),
                            getter(item.Female)));
                }
            }

            public static MaskItem<bool, GenderedItem<MaskItem<bool, TMask?>?>?>? HasBeenSetMaskHelper<TItem, TMask>(
                IGenderedItemGetter<TItem?>? item,
                Func<TItem, TMask?> getter)
                where TItem : class
                where TMask : class, IMask<bool>
            {
                if (item == null)
                {
                    return null;
                }
                else
                {
                    var maleMask = item.Male == null ? null : getter(item.Male);
                    var femaleMask = item.Female == null ? null : getter(item.Female);
                    var maleHas = maleMask?.Any(i => i) ?? false;
                    var femaleHas = femaleMask?.Any(i => i) ?? false;
                    return new MaskItem<bool, GenderedItem<MaskItem<bool, TMask?>?>?>(
                        true,
                        new GenderedItem<MaskItem<bool, TMask?>?>(
                            new MaskItem<bool, TMask?>(maleHas, maleMask),
                            new MaskItem<bool, TMask?>(femaleHas, femaleMask)));
                }
            }

            public static MaskItem<TRet, GenderedItem<MaskItem<TRet, TRetMask?>?>?>? TranslateHelper<TItem, TMask, TRet, TRetMask>(
                MaskItem<TItem, GenderedItem<MaskItem<TItem, TMask?>?>?>? mask,
                Func<TItem, TRet> eval,
                Func<TMask?, Func<TItem, TRet>, TRetMask?> maskConv)
                where TMask : class, IMask<TItem>
                where TRetMask : class, IMask<TRet>
            {
                if (mask == null)
                {
                    return null;
                }
                else
                {
                    return new MaskItem<TRet, GenderedItem<MaskItem<TRet, TRetMask?>?>?>(
                        eval(mask.Overall),
                        mask.Specific == null ? null : new GenderedItem<MaskItem<TRet, TRetMask?>?>(
                            mask.Specific.Male == null ? null : new MaskItem<TRet, TRetMask?>(eval(mask.Specific.Male.Overall), maskConv(mask.Specific.Male.Specific, eval)),
                            mask.Specific.Female == null ? null : new MaskItem<TRet, TRetMask?>(eval(mask.Specific.Female.Overall), maskConv(mask.Specific.Female.Specific, eval))));
                }
            }
        }
    }
}
