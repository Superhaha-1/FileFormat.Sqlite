using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileFormat.Sqlite
{
    internal static class FileFormatHelper
    {
        public static void VerifyName(this string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name为空");
        }

        public static void VerifyFullName(this string fullName)
        {
            if (fullName.Contains(' '))
                throw new Exception("Name中有空字符");
            fullName.VerifyName();
        }

        public static void VerifyData(this byte[] data)
        {
            if (data == null)
                throw new Exception("数据不能为空");
        }

    }
}
