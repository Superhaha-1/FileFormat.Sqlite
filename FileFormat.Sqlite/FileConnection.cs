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

        public async Task<NodeConnection> ConnectRootNodeAsync()
        {
            var context = new FileFormatContext(FilePath);
            return new NodeConnection(context, await context.GetRootNodeAsync());
        }

        public async Task<NodeConnection> ConnectNodeAsync(string fullName)
        {
            fullName.VerifyFullName();
            var context = new FileFormatContext(FilePath);
            string[] names = fullName.Split(Separator);
            try
            {
                var node = await context.FindNodeAsync(names);
                return new NodeConnection(context, node);
            }
            catch
            {
                context.Dispose();
                throw;
            }
        }

        public async Task CreateNodeAsync(string fullName)
        {
            fullName.VerifyFullName();
            using (var context = new FileFormatContext(FilePath))
            {
                string[] names = fullName.Split(Separator);
                Node node = await context.GetRootNodeAsync();
                foreach(var name in names)
                {
                    var node_new = await context.GetNodeAsync(node, name);
                    if(node_new == null)
                    {
                        node_new = new Node(name, node.Key);
                        await context.Nodes.AddAsync(node_new);
                        await context.SaveChangesAsync();
                    }
                    node = node_new;
                }
            }
        }

        public async Task SaveDataAsync(string fullName, byte[] value)
        {
            fullName.VerifyFullName();
            value.VerifyData();
            using (var context = new FileFormatContext(FilePath))
            {
                string[] names = fullName.Split(Separator);
                var node = await context.FindNodeAsync(names.Take(names.Length - 1));
                string dataName = names[names.Length - 1];
                var data = await context.GetDataAsync(node, dataName);
                if (data == null)
                    await context.Datas.AddAsync(new Data(dataName, value, node.Key));
                else
                    data.Value = value;
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 读取数据(不存在该节点会报错)
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public async Task<byte[]> ReadDataAsync(string fullName)
        {
            fullName.VerifyFullName();
            string[] names = fullName.Split(Separator);
            using (var connection = await ConnectNodeAsync(names[0]))
            {
                foreach (var name in names.Skip(1).Take(names.Length - 2))
                    await connection.MoveDownToAsync(name);
                return await connection.ReadDataAsync(names[names.Length - 1]);
            }
            //using (var context = new FileFormatContext(FilePath))
            //{
            //    string[] names = fullName.Split(Separator);
            //    var node = await context.FindNodeAsync(names.Take(names.Length - 1));
            //    string dataName = names[names.Length - 1];
            //    var data = await context.GetDataAsync(node, dataName);
            //    if (data == null)
            //        throw new Exception($"该节点不存在为{dataName}的数据");
            //    return data.Value;
            //}
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
