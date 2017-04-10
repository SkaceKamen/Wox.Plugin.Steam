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
		private readonly BinaryReader reader;

		public Dictionary<uint, BinaryVdfItem> Items = new Dictionary<uint, BinaryVdfItem>();

		public Reader(string path)
		{
			reader = new BinaryReader(File.OpenRead(path));

			// Read some header fields
			reader.ReadByte();
			if (reader.ReadByte() != 0x44 ||
			    reader.ReadByte() != 0x56)
			{
				throw new Exception("Invalid vdf format");
			}

			// Skip more header fields
			reader.ReadBytes(5);

			while (true)
			{
				var id = reader.ReadUInt32();
				if (id == 0) break;

				// Skip unused fields
				reader.ReadBytes(44);

				// Load details
				Items[id] = ReadEntries();
			}
		}

		private BinaryVdfItem ReadEntries()
		{
			var result = new BinaryVdfItem();

			while (true)
			{
				var type = reader.ReadByte();
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
						result[key] = new BinaryVdfItem(reader.ReadUInt32().ToString());
						break;
					default:
						throw new Exception("Uknown entry type " + type + ".");
				}
			}

			return result;
		}

		private string ReadString()
		{
			var result = "";

			// Read chars until null
			while (reader.PeekChar() != 0x00)
			{
				result += reader.ReadChar();
			}

			// Skip null
			reader.ReadByte();

			return result;
		}


	}

	public class BinaryVdfItem
	{
		public Dictionary<string, BinaryVdfItem> Items = new Dictionary<string, BinaryVdfItem>();
		public string Value;

		public BinaryVdfItem()
		{
			
		}

		public BinaryVdfItem(string value)
		{
			Value = value;
		}

		public BinaryVdfItem this[string index]
		{
			get { return Items[index]; }
			set { Items[index] = value; }
		}

		public string GetString(string index)
		{
			return this[index].Value;
		}
	}

}
