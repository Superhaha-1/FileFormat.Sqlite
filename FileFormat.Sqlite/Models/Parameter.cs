using System.ComponentModel.DataAnnotations;

namespace FileFormat.Sqlite.Models
{
    internal sealed class Parameter
    {
        [Key]
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
