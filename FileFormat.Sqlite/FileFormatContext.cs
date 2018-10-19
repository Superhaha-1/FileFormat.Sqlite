using FileFormat.Sqlite.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileFormat.Sqlite
{
    internal class FileFormatContext : DbContext
    {
        public DbSet<Parameter> Parameters { get; set; }

        public DbSet<Node> Nodes { get; set; }

        public DbSet<Data> Datas { get; set; }

        public FileFormatContext()
        {
            
        }

        public FileFormatContext(string path)
        {
            if (!File.Exists(path))
                throw new Exception("文件不存在");
            Path = path;
        }

        private string Path { get; } = "template.db";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={Path};Cache=shared");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Node>()
                .HasMany(n => n.ChildrenNodes)
                .WithOne()
                .IsRequired(false)
                .HasForeignKey(n=>n.NodeKey)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Node>()
                .HasMany(n => n.ChildrenDatas)
                .WithOne()
                .IsRequired(true)
                .HasForeignKey(d=>d.NodeKey)
                .OnDelete(DeleteBehavior.Cascade);
        }

        /// <summary>
        /// 获取根节点
        /// </summary>
        /// <returns></returns>
        public async Task<Node> GetRootNodeAsync()
        {
            return await Nodes.FindAsync(1);
        }

        public async Task<Node> FindNodeAsync(IEnumerable<string> nodeNames)
        {
            var node = await GetRootNodeAsync();
            foreach (var name in nodeNames)
            {
                name.VerifyName();
                node = await GetNodeAsync(node, name)
                    ?? throw new Exception($"没有为{name}的节点");
            }
            return node;
        }

        public async Task<Node> GetNodeAsync(Node sourceNode, string nodeName)
        {
            nodeName.VerifyName();
            if (sourceNode == null)
                throw new Exception("源节点为空");
            return await Entry(sourceNode)
                .Collection(n => n.ChildrenNodes)
                .Query().FirstOrDefaultAsync(n => n.Name == nodeName);
        }

        public async Task<Data> GetDataAsync(Node node, string name)
        {
            name.VerifyName();
            return await Entry(node)
                .Collection(n => n.ChildrenDatas)
                .Query()
                .FirstOrDefaultAsync(d => d.Name == name);
        }
    }
}
