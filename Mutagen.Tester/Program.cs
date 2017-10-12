using Mutagen.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var tests = new Passthrough_Tests();
            tests.OblivionESM();
        }
    }
}
