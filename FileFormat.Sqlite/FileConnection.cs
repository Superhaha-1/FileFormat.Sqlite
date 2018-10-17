using FileFormat.Sqlite.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileFormat.Sqlite
{
    public sealed class FileConnection
    {
        public FileConnection(string path)
        {
            Path = path;
            if (!File.Exists(path))
            {
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FileFormat.Sqlite.template.db"))
                {
                    using (var fileStream = new FileStream(path, FileMode.CreateNew))
                    {
                        stream.CopyTo(fileStream);
                    }
                }
            }
        }

        private string Path { get; }

        public async Task SaveData(string key, byte[] value)
        {
            if (value == null)
                throw new Exception("数据不能为空");
            using (var context = new FileFormatContext(Path))
            {
                var data = await context.Datas.FindAsync(key);
                if (data == null)
                    await context.Datas.AddAsync(new Data(key, value));
                else
                    data.Value = value;
                await context.SaveChangesAsync();
            }
        }

        public async Task<byte[]> ReadData(string key)
        {
            using (var context = new FileFormatContext(Path))
            {
                var data = await context.Datas.FindAsync(key);
                if (data == null)
                    throw new Exception("不存在该文件");
                return data.Value;
            }
        }
    }
}
