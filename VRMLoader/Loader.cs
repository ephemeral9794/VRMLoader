using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using VRMLoader.Utility;

namespace VRMLoader
{
	class Loader
	{
		#region VRM(glb) Format Struct
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
		#endregion

		public static Model Load(Stream s) {
			EndianStream stream = new EndianStream(s, Endian.LITTLE_ENDIAN);
			Header header = Header.Load(stream);
			if (!header.Check()) {	// 'glTF'
				throw new FormatException("This file is not The VRM(glTF) file.");
			}
			Chunk jsonChunk = Chunk.Load(stream);
			if (jsonChunk.type != 0x4E4F534A) {	// 'JSON'
				throw new InvalidDataException("The JSON chunk must follow the header");
			}
			string json = Encoding.UTF8.GetString(jsonChunk.data);
			Chunk binChunk = Chunk.Load(stream);
			if (binChunk.type != 0x004E4942) {	// 'BIN'
				throw new InvalidDataException("The JSON chunk must follow the header");
			}

			var serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };
			Model model = new Model {
				Info = serializer.Deserialize<ModelInfo>(new JsonTextReader(new StringReader(json))),
				Data = new ModelData(binChunk.data)
			};
			return model;
		}
		
		public static string SerializeModelInfo(ModelInfo model) {
			var serializer = new JsonSerializer() { 
				NullValueHandling = NullValueHandling.Ignore,
				Formatting = Formatting.Indented
			};
			var builder = new StringBuilder();
			serializer.Serialize(new JsonTextWriter(new StringWriter(builder)), model);
			return builder.ToString();
		}
	}
}
