using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;

namespace TestTools.Helpers
{
    [ExcludeFromCodeCoverage]
    [DebuggerDisplay("{ToString(),nq}")]
    public sealed class TempFile : IDisposable
    {
        public TempFile() : this(System.IO.Path.GetTempFileName())
        {
        }

        public TempFile(string path)
        {
            this.path = path;
        }

        public static TempFile WithExtension(string extension)
        {
            return new TempFile(
                System.IO.Path.Combine(
                    System.IO.Path.GetTempPath(),
                    System.IO.Path.ChangeExtension(System.IO.Path.GetRandomFileName(), extension)));
        }

        private string path;
        public string Path => path;

        public static implicit operator string(TempFile tempFile) => tempFile.path;

        public override string ToString() => path;

        public void Dispose()
        {
            var path = Interlocked.Exchange(ref this.path, null);
            if (path != null) File.Delete(path);
        }
    }
}
