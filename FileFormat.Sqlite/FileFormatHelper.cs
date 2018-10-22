using System;
using System.Linq;

namespace FileFormat.Sqlite
{
    internal static class FileFormatHelper
    {
        private static char Separator => '.';

        public static string[] GetChidrenNames(this string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                throw new Exception("FullName为空");
            var names = fullName.Split(Separator);
            return names;
        }

        public static void VerifyName(this string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name为空");
            if (name.Contains(' '))
                throw new Exception("Name中有空字符");
        }

        public static void VerifyData(this byte[] data)
        {
            if (data == null)
                throw new Exception("数据不能为空");
        }
    }
}
