using System.ComponentModel.DataAnnotations;

namespace FileFormat.Sqlite.Models
{
    internal class Data
    {
        [Key]
        public int Key { get; set; }

        public string Name { get; set; }

        public virtual byte[] Value { get; set; }

        public int NodeKey { get; set; }

        private Data()
        {

        }

        public Data(string name, byte[] value, int nodeKey)
        {
            Name = name;
            Value = value;
            NodeKey = nodeKey;
        }
    }
}
