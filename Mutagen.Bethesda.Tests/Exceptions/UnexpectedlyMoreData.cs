using Loqui;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Tests
{
    public class UnexpectedlyMoreData : Exception, IPrintable
    {
        public readonly string Path;
        public readonly long Position;

        public UnexpectedlyMoreData(string path, long pos)
        {
            this.Position = pos;
            this.Path = path;
        }

        public override string ToString()
        {
            return $"{Path} had more data past position 0x{Position} than source stream.";
        }

        public void ToString(FileGeneration fg, string name)
        {
            fg.AppendLine(Path);
            using (new DepthWrapper(fg))
            {
                fg.AppendLine($"had more data past position 0x{Position} than source stream.");
            }
        }
    }
}
