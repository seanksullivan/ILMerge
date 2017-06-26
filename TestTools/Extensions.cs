using ILMerging;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using TestTools.Helpers;

namespace TestTools
{
    [ExcludeFromCodeCoverage]
    internal static class Extensions
    {
        public static void SetUpInputAssemblyForTest(this ILMerge ilMerge, Assembly inputAssembly)
        {
            ilMerge.SetSearchDirectories(ShadowCopyUtils.GetTransitiveClosureDirectories(inputAssembly).ToArray());
            ilMerge.SetInputAssemblies(new[] { inputAssembly.Location });
        }

        public static void MergeToTempFileForTest(this ILMerge ilMerge, string outputExtension)
        {
            using (var outputFile = TempFile.WithExtension(outputExtension))
            {
                ilMerge.OutputFile = outputFile;
                ilMerge.Merge();
            }
        }
    }
}
