using FileFormat.Sqlite.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FileFormat.Sqlite
{
    public sealed class FileConnection
    {
        public FileConnection(string filePath)
        {
            FilePath = filePath;
            if (!File.Exists(filePath))
            {
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FileFormat.Sqlite.template.db"))
                {
                    using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
                    {
                        stream.CopyTo(fileStream);
                    }
                }
            }
        }

        private string FilePath { get; }

        private char Separator => '.';

        private string RootName => string.Empty;

        public async Task SaveDataAsync(string fullName, byte[] value)
        {
            fullName.VerifyFullName();
            value.VerifyData();
            using (var context = new FileFormatContext(FilePath))
            {
                string[] names = fullName.Split(Separator);
                var node = await context.Nodes.FindAsync(1);
                for (int i = 0; i < names.Length - 1; i++)
                {
                    var name = names[i];
                    name.VerifyName();
                    var newNode = await context.Entry(node).Collection(b => b.ChildrenNodes).Query().FirstOrDefaultAsync(n => n.Name == name);
                    if (newNode == null)
                    {
                        newNode = new Node(name, node.Key);
                        await context.Nodes.AddAsync(newNode);
                    }
                    node = newNode;
                }
                var dataName = names[names.Length - 1];
                dataName.VerifyName();
                var data = await context.Entry(node).Collection(b => b.ChildrenDatas).Query().FirstOrDefaultAsync(d => d.Name == dataName);
                if (data == null)
                    await context.Datas.AddAsync(new Data(dataName, value, node.Key));
                else
                    data.Value = value;
                await context.SaveChangesAsync();
            }
        }

        public async Task<byte[]> ReadDataAsync(string fullName)
        {
            fullName.VerifyFullName();
            using (var context = new FileFormatContext(FilePath))
            {
                string[] names = fullName.Split(Separator);
                var node = await context.FindNodeAsync(names.Take(names.Length - 1));
                string dataName = names[names.Length - 1];
                var data = await context.GetDataAsync(node, dataName);
                if (data == null)
                    throw new Exception($"该节点不存在为{dataName}的数据");
                return data.Value;
            }
        }

        public async Task DeleteNode(string fullName)
        {

        }

        //public string[] GetChildrenKeys(string key)
        //{
        //    using (var context = new FileFormatContext(FilePath))
        //    {
        //        return context.Datas.Select(d => d.Key).Where(k => k.StartsWith(key)).ToArray();
        //    }
        //}
    }
}
