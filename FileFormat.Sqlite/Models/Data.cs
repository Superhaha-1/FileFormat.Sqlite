using System.ComponentModel.DataAnnotations;

namespace FileFormat.Sqlite.Models
{
    internal sealed class Data
    {
        [Key]
        public string Key { get; set; }

        public byte[] Value { get; set; }

        public Data()
        {

        }

        public Data(string key, byte[] value)
        {
            Key = key;
            Value = value;
        }
    }
}
