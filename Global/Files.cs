using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Global
{
    public static class Files
    {
        public static List<FileInfo> GetFiles(string path)
        {
            return new DirectoryInfo(path).GetFiles().ToList();
        }
    }
}