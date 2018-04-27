using System;
using System.IO;
using System.Linq;
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
	struct VRMScene {
		public string name;
		public int[] nodes;

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("  name      : {0}", name);
			int i = 0;
			foreach (int n in nodes) {
				builder.AppendLine();
				builder.AppendFormat("  node #{0:000} : {1}", i, n);
				i++;
			}
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
					} else {
						vrm.asset.version = "";
					}
					// copyright
					if (asset.ContainsKey("copyright")) {
						vrm.asset.copyright = asset.Value<String>("copyright");
					} else {
						vrm.asset.copyright = "";
					}
					// generator
					if (asset.ContainsKey("generator")) {
						vrm.asset.generator = asset.Value<String>("generator");
					} else {
						vrm.asset.generator = "";
					}
					// minVersion
					if (asset.ContainsKey("minVersion")) {
						vrm.asset.minVersion = asset.Value<String>("minVersion");
					} else {
						vrm.asset.minVersion = "";
					}
				}

				// scenes
				if (root.ContainsKey("scenes")) {
					JArray scenes = root.Value<JArray>("scenes");
					vrm.scenes = new VRMScene[scenes.Count];
					for(int i = 0; i < scenes.Count; i++) {
						JObject scene = scenes.Value<JObject>(i);
						vrm.scenes[i] = new VRMScene();
						if (scene.ContainsKey("name")) {
							vrm.scenes[i].name = scene.Value<string>("name");
						} else {
							vrm.scenes[i].name = "";
						}
						if (scene.ContainsKey("nodes")) {
							JArray nodes = scene.Value<JArray>("nodes");
							vrm.scenes[i].nodes = new int[nodes.Count];
							for (int j = 0; j < nodes.Count; j++) {
								vrm.scenes[i].nodes[j] = nodes.Value<int>(j);
							}
						} else {
							vrm.scenes[i].nodes = (int[])Enumerable.Empty<int>();
						}
					}
				} else {
					vrm.scenes = (VRMScene[])Enumerable.Empty<VRMScene>();
				}
			} else {
				throw new InvalidDataException("This JSON file does not contain a value.");
			}

			return vrm;
		}

		Header header;
		Chunk jsonChunk;

		VRMAsset asset;
		VRMScene[] scenes;

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
			DebugLog.WriteLine();
			DebugLog.WriteLine("*** VRM Scene ***");
			int i = 0;
			foreach(var scene in scenes) {
				DebugLog.WriteLine(string.Format("Scene #{0:000}", i));
				DebugLog.WriteLine(scene.ToString());
				i++;
			}
			DebugLog.WriteLine();

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
			Console.WriteLine();
			Console.WriteLine("*** VRM Scene ***");
			int i = 0;
			foreach(var scene in scenes) {
				Console.WriteLine(string.Format("Scene #{0:000}", i));
				Console.WriteLine(scene.ToString());
				i++;
			}
			Console.WriteLine();
		}
		#endif
	}
}
