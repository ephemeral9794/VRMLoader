using System;
using System.IO;
using System.Text;
using VRMLoader.Utility;

namespace VRMLoader
{
	struct VRMAsset {
        public string copyright;
        public string generator;
        public string version;
        public string minVersion;
	}

	public class VRM
	{
		struct Header {
			public uint magic;
			public uint version;
			public uint length;

			public static Header Load(EndianStream stream) {
				Header header;
				header.magic = stream.ReadUInt32();
				header.version = stream.ReadUInt32();
				header.length = stream.ReadUInt32();
				return header;
			}
			public bool Check() {
				return (magic == 0x46546C67);
			}
			public override string ToString() {
				return string.Format("[magic:{0} version:{1} length:{2}]", Util.ReverseUIntToString(magic), version, length);
			}
		}
		struct Chunk {
			public uint length;
			public uint type;
			public byte[] data;

			public static Chunk Load(EndianStream stream) {
				Chunk chunk;
				chunk.length = stream.ReadUInt32();
				chunk.type = stream.ReadUInt32();
				chunk.data = stream.ReadBytes((int)chunk.length);
				return chunk;
			}
			public override string ToString() {
				return string.Format("[length:{0}, type:{1}]", length, Util.ReverseUIntToString(type));
			}
		}

		public static VRM Load(Stream s) {
			VRM asset = new VRM();
			EndianStream stream = new EndianStream(s, Endian.LITTLE_ENDIAN);
			asset.header = Header.Load(stream);
			if (!asset.header.Check()) {	// 'glTF'
				throw new FormatException("This file is not The VRM(glTF) file.");
			}
			asset.jsonChunk = Chunk.Load(stream);
			if (asset.jsonChunk.type != 0x4E4F534A) {	// 'JSON'
				throw new InvalidDataException("The JSON chunk must follow the header");
			}

			return asset;
		}

		Header header;
		Chunk jsonChunk;

		#if DEBUG
		public void Print() {
			DebugLog.Start();
			DebugLog.Visible = true;

			DebugLog.WriteLine("-- VRM Header --");
			DebugLog.WriteLine(header.ToString());
			DebugLog.WriteLine("-- VRM JSON Chunk --");
			DebugLog.WriteLine(jsonChunk.ToString());

			DebugLog.End();
		}
		#else 
		public void Print() {
			Console.WriteLine("-- VRM Header --");
			Console.WriteLine(header.ToString());
			Console.WriteLine("-- VRM JSON Chunk --");
			Console.WriteLine(jsonChunk.ToString());
		}
		#endif
	}
}
