using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using VRMLoader.Utility;

namespace VRMLoader
{
	struct VRMAsset {
        public string copyright;
        public string generator;
        public string version;
        public string minVersion;

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("  copyright  : {0}", copyright).AppendLine();
			builder.AppendFormat("  generator  : {0}", generator).AppendLine();
			builder.AppendFormat("  version    : {0}", version).AppendLine();
			builder.AppendFormat("  minVersion : {0}", minVersion);
			return builder.ToString();
		}
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
				return string.Format("[magic:{0} version:{1} length:{2}]", Extention.ReverseUIntToString(magic), version, length);
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
				return string.Format("[length:{0}, type:{1}]", length, Extention.ReverseUIntToString(type));
			}
		}

		public static VRM Load(Stream s) {
			VRM vrm = new VRM();
			EndianStream stream = new EndianStream(s, Endian.LITTLE_ENDIAN);
			vrm.header = Header.Load(stream);
			if (!vrm.header.Check()) {	// 'glTF'
				throw new FormatException("This file is not The VRM(glTF) file.");
			}
			vrm.jsonChunk = Chunk.Load(stream);
			if (vrm.jsonChunk.type != 0x4E4F534A) {	// 'JSON'
				throw new InvalidDataException("The JSON chunk must follow the header");
			}
			
			// Parse JSON
			JObject root = JObject.Parse(Encoding.UTF8.GetString(vrm.jsonChunk.data));
			if (root.HasValues) {
				// asset
				if (root.ContainsKey("asset")) {
					JObject asset = (JObject)root["asset"];
					// version
					if (asset.ContainsKey("version")) {
						vrm.asset.version = asset.Value<String>("version");
					}
					// copyright
					if (asset.ContainsKey("copyright")) {
						vrm.asset.copyright = asset.Value<String>("copyright");
					}
					// generator
					if (asset.ContainsKey("generator")) {
						vrm.asset.generator = asset.Value<String>("generator");
					}
					// minVersion
					if (asset.ContainsKey("minVersion")) {
						vrm.asset.minVersion = asset.Value<String>("minVersion");
					}
				}
			} else {
				throw new InvalidDataException("This JSON file does not contain a value.");
			}

			return vrm;
		}

		Header header;
		Chunk jsonChunk;

		VRMAsset asset;

		#if DEBUG
		public void Print() {
			DebugLog.Start();
			DebugLog.Visible = true;

			DebugLog.WriteLine("-- VRM Header --");
			DebugLog.WriteLine(header.ToString());
			DebugLog.WriteLine("-- VRM JSON Chunk --");
			DebugLog.WriteLine(jsonChunk.ToString());
			DebugLog.WriteLine();
			DebugLog.WriteLine("*** VRM Asset ***");
			DebugLog.WriteLine(asset.ToString());

			DebugLog.End();
		}
		#else 
		public void Print() {
			Console.WriteLine("-- VRM Header --");
			Console.WriteLine(header.ToString());
			Console.WriteLine("-- VRM JSON Chunk --");
			Console.WriteLine(jsonChunk.ToString());
			Console.WriteLine();
			Console.WriteLine("** Asset **");
			Console.WriteLine(asset.ToString());
		}
		#endif
	}
}
