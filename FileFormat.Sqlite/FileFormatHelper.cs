using System;
using System.IO;
using System.Linq;

namespace FileFormat.Sqlite
{
    public static class FileFormatHelper
    {
        private static char Separator => '.';

        private static char Space => ' ';

        internal static string[] GetChidrenNames(this string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                throw new Exception("FullName为空");
            var names = fullName.Split(Separator);
            return names;
        }

        internal static void ValidateName(this string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name为空");
            if (name.Contains(Space))
                throw new Exception("Name中有空字符");
        }

        internal static void ValidateData(this byte[] data)
        {
            if (data == null)
                throw new Exception("数据不能为空");
        }

        public static char[] InvalidPathChars { get; } = Path.GetInvalidPathChars().Union(new char[] { Separator, Space }).ToArray();
    }
}
