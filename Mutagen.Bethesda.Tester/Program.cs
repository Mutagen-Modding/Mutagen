using Mutagen.Bethesda.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var tests = new Passthrough_Tests();
            tests.OblivionESM_Binary();
        }
    }
}
