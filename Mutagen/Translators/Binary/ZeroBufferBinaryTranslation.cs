using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Binary
{
    public class ZeroBufferBinaryTranslation
    {
        public readonly static ZeroBufferBinaryTranslation Instance = new ZeroBufferBinaryTranslation();
        public const byte Zero = 0;
        
        public void Write(MutagenWriter writer, int length)
        {
            for (int i = 0; i < length; i++)
            {
                writer.Write(Zero);
            }
        }
    }
}
