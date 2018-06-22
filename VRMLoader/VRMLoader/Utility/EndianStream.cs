using System;
using System.IO;

namespace VRMLoader.Utility
{
	public enum Endian {
		BIG_ENDIAN,
		LITTLE_ENDIAN,
	};
	public class EndianStream : BinaryReader
	{
		public EndianStream(Stream stream, Endian type) : base(stream) { endian = type; }
		public EndianStream(Stream stream) : this(stream, Endian.LITTLE_ENDIAN) {}
		public EndianStream(byte[] buffer, Endian type) : this(new MemoryStream(buffer), type) {}
		public EndianStream(byte[] buffer) : this(new MemoryStream(buffer)) {}
		~EndianStream() { Dispose(); }

		private Endian endian;
		public Endian Order { 
			get { return endian; } 
			set { endian = value; } 
		}
		public long Size {
			get { return base.BaseStream.Length; }
		}
		public long Position {
			get { return base.BaseStream.Position; }
			set { base.BaseStream.Position = value; }
		}
		private byte[] a16 = new byte[2];
        private byte[] a32 = new byte[4];
        private byte[] a64 = new byte[8];

		public new void Dispose()  {
			base.Dispose();
		}

		public long Reset() {
			return base.BaseStream.Seek(0, SeekOrigin.Begin);
		}

		public override byte[] ReadBytes(int count) {
			return base.ReadBytes(count);
		}

		public override bool ReadBoolean()
		{
			return base.ReadBoolean();
		}

		public override byte ReadByte()
		{
			try {
				return base.ReadByte();
			} catch {
				return 0;
			}
		}

		public override sbyte ReadSByte()
		{
			try {
				return base.ReadSByte();
			} catch {
				return 0;
			}
		}

		public override char ReadChar()
        {
            return base.ReadChar();
        }
 
        public override Int16 ReadInt16()
        {
            if (endian == Endian.BIG_ENDIAN)
            {
                a16 = base.ReadBytes(2);
                Array.Reverse(a16);
                return BitConverter.ToInt16(a16, 0);
            }
            else return base.ReadInt16();
        }
 
        public override int ReadInt32()
        {
            if (endian == Endian.BIG_ENDIAN)
            {
                a32 = base.ReadBytes(4);
                Array.Reverse(a32);
                return BitConverter.ToInt32(a32, 0);
            }
            else return base.ReadInt32();
        }
 
        public override Int64 ReadInt64()
        {
            if (endian == Endian.BIG_ENDIAN)
            {
                a64 = base.ReadBytes(8);
                Array.Reverse(a64);
                return BitConverter.ToInt64(a64, 0);
            }
            else return base.ReadInt64();
        }
 
        public override UInt16 ReadUInt16()
        {
            if (endian == Endian.BIG_ENDIAN)
            {
                a16 = base.ReadBytes(2);
                Array.Reverse(a16);
                return BitConverter.ToUInt16(a16, 0);
            }
            else return base.ReadUInt16();
        }
 
        public override UInt32 ReadUInt32()
        {
            if (endian == Endian.BIG_ENDIAN)
            {
                a32 = base.ReadBytes(4);
                Array.Reverse(a32);
                return BitConverter.ToUInt32(a32, 0);
            }
            else return base.ReadUInt32();
        }

        public override UInt64 ReadUInt64()
        {
            if (endian == Endian.BIG_ENDIAN)
            {
                a64 = base.ReadBytes(8);
                Array.Reverse(a64);
                return BitConverter.ToUInt64(a64, 0);
            }
            else return base.ReadUInt64();
        }
 
        public override Single ReadSingle()
        {
            if (endian == Endian.BIG_ENDIAN)
            {
                a32 = base.ReadBytes(4);
                Array.Reverse(a32);
                return BitConverter.ToSingle(a32, 0);
            }
            else return base.ReadSingle();
        }
 
        public override Double ReadDouble()
        {
            if (endian == Endian.BIG_ENDIAN)
            {
                a64 = base.ReadBytes(8);
                Array.Reverse(a64);
                return BitConverter.ToUInt64(a64, 0);
            }
            else return base.ReadDouble();
        }

		public void ReadSkip(long count) {
			Position += count;
		}

		public string ReadStringToNull()
        {
            string result = "";
            char c;
            for (int i = 0; i < base.BaseStream.Length; i++)
            {
                if ((c = (char)base.ReadByte()) == 0)
                {
                    break;
                }
                result += c.ToString();
            }
            return result;
        }
	}
}
