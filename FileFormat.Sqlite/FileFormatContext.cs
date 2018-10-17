using FileFormat.Sqlite.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace FileFormat.Sqlite
{
    internal class FileFormatContext : DbContext
    {
        public DbSet<Parameter> Parameters { get; set; }

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
            optionsBuilder.UseSqlite($"Data Source={Path}");
        }
    }
}
