using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.Tests.TestsHelpers
{
    public static class TestFileHelper
    {
        private static readonly string Root = Path.Combine(Path.GetTempPath(), "ReadingListTests");

        public static string CreateTempCsv(string name, string content)
        {
            Directory.CreateDirectory(Root);
            var path = Path.Combine(Root, $"{Guid.NewGuid()}_{name}.csv");
            File.WriteAllText(path, content);
            return path;
        }

        public static void Cleanup()
        {
            if (Directory.Exists(Root))
                Directory.Delete(Root, recursive: true);
        }
    }
}
