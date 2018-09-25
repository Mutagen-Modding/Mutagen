using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests
{
    public static class FormIDLinkTesterHelper
    {
        public static bool Active = false;
        public static List<ILink> CreatedLinks = new List<ILink>();

        public static void Add(ILink link)
        {
            if (!Active) return;
            lock (CreatedLinks)
            {
                CreatedLinks.Add(link);
            }
        }
    }
}
