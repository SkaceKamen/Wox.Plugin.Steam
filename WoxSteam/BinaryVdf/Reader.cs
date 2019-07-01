using System;
using System.Collections.Generic;
using System.IO;

namespace WoxSteam.BinaryVdf
{
    /// <summary>
    /// Reader for binary vdf files.
    /// </summary>
    public class Reader
    {
        private readonly BinaryReader _reader;

        public readonly Dictionary<uint, BinaryVdfItem> Items = new Dictionary<uint, BinaryVdfItem>();

        public Reader(string path)
        {
            _reader = new BinaryReader(File.OpenRead(path), System.Text.Encoding.UTF8);

            // Read some header fields
            _reader.ReadByte();
            if (_reader.ReadByte() != 0x44 ||
                _reader.ReadByte() != 0x56)
            {
                throw new Exception("Invalid vdf format");
            }

            // Skip more header fields
            _reader.ReadBytes(5);

            while (true)
            {
                var id = _reader.ReadUInt32();
                if (id == 0) break;

                // Skip unused fields
                _reader.ReadBytes(44);

                // Load details
                Items[id] = ReadEntries();
            }
        }

        private BinaryVdfItem ReadEntries()
        {
            var result = new BinaryVdfItem();

            while (true)
            {
                var type = _reader.ReadByte();
                if (type == 0x08) break;

                var key = ReadString();

                switch (type)
                {
                    case 0x00:
                        result[key] = ReadEntries();
                        break;
                    case 0x01:
                        result[key] = new BinaryVdfItem(ReadString());
                        break;
                    case 0x02:
                        result[key] = new BinaryVdfItem(_reader.ReadUInt32().ToString());
                        break;
                    default:
                        throw new Exception("Uknown entry type " + type + ".");
                }
            }

            return result;
        }

        private string ReadString()
        {
            var result = new List<byte>();

            byte b;
            while ((b = _reader.ReadByte()) != 0x00)
            {
                result.Add(b);
            }

            return System.Text.Encoding.UTF8.GetString(result.ToArray());
        }

        public static Dictionary<uint, BinaryVdfItem> Read(string path)
        {
            return new Reader(path).Items;
        }
    }
}