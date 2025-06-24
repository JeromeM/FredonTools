using System.IO;

namespace SasFredonWPF.Helpers
{
    public static class FileHelper
    {
        public static string[] GetFiles(string path, string pattern = "*")
        {
            return Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly);
        }
    }
}
