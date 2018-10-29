using System;
using System.IO;
using System.Linq;

namespace FileFormat.Sqlite
{
    public static class FileFormatHelper
    {
        private static char Separator => '.';

        internal static string[] GetChidrenNames(this string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                throw new Exception("FullName为空");
            var names = fullName.Split(Separator);
            return names;
        }

        internal static void VerifyName(this string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name为空");
            if (name.Contains(' '))
                throw new Exception("Name中有空字符");
        }

        internal static void VerifyData(this byte[] data)
        {
            if (data == null)
                throw new Exception("数据不能为空");
        }

        private static char[] InvalidPathChars { get; } = Path.GetInvalidPathChars().Intersect(new char[] { Separator }).ToArray();

        public static (bool isValid, string describe) IsValidName(this string name)
        {
            foreach(var c in InvalidPathChars)
            {
                if (name.Contains(c))
                    return (false, $"名称中不能包含{c}");
            }
            return (true, null);
        }
    }
}
