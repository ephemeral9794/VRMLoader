using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using VRMLoader.Utility;

namespace VRMLoader
{
	class Loader
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
			Header header = Header.Load(stream);
			if (!header.Check()) {	// 'glTF'
				throw new FormatException("This file is not The VRM(glTF) file.");
			}
			Chunk jsonChunk = Chunk.Load(stream);
			if (jsonChunk.type != 0x4E4F534A) {	// 'JSON'
				throw new InvalidDataException("The JSON chunk must follow the header");
			}
			
			// Parse JSON
			JObject root = JObject.Parse(Encoding.UTF8.GetString(jsonChunk.data));
			if (root.HasValues) {
				// asset
				if (root.ContainsKey("asset")) {
					vrm.Asset = new VRMAsset((JObject)root["asset"]);
				}
				
				// buffers
				if (root.ContainsKey("buffers")) {
					JArray buffers = (JArray)root["buffers"];
					foreach (JObject b in buffers) {
						VRMBuffer buffer = new VRMBuffer(b);
						Chunk bin = Chunk.Load(stream);
						buffer.Data = new byte[bin.length];
						Array.Copy(bin.data, buffer.data, bin.length);
						vrm.Buffers.Add(buffer);
					}
				} else {
					throw new VRMException("The 'buffers' node is a required element.");
				}

				if (root.ContainsKey("bufferViews")) {
					JArray bufferViews = (JArray)root["bufferViews"];
					foreach (JObject v in bufferViews) {
						vrm.BufferViews.Add(new VRMBufferView(v));
					}
				} else {
					throw new VRMException("The 'bufferViews' node is a required element.");
				}

				// accessors
				if (root.ContainsKey("accessors")) {
					JArray accessors = (JArray)root["accessors"];
					foreach(JObject a in accessors) {
						vrm.Accessors.Add(new VRMAccessor(a));
					}
				} else {
					throw new VRMException("The 'accessors' node is a required element.");
				}

				// scenes
				if (root.ContainsKey("scenes")) {
					JArray scenes = (JArray)root["scenes"];
					for(int i = 0; i < scenes.Count; i++) {
						vrm.Scenes.Add(new VRMScene((JObject)scenes[i]));
					}
				} else {
					throw new VRMException("The 'scenes' node is a required element.");
				}

				// nodes
				if (root.ContainsKey("nodes")) {
					JArray nodes = (JArray)root["nodes"];
					foreach (JObject n in nodes) {
						vrm.Nodes.Add(new VRMNode(n));
					}
				} else {
					throw new VRMException("The 'nodes' node is a required element.");
				}

				// meshes
				if (root.ContainsKey("meshes")) {
					JArray meshes = (JArray)root["meshes"];
					foreach (JObject m in meshes) {
						vrm.Meshes.Add(new VRMMesh(m));
					}
				}
			} else {
				throw new InvalidDataException("This JSON data does not contain a value.");
			}

			return vrm;
		}

		#if DEBUG
		public static void Print(VRM vrm) {
			DebugLog.Start();
			DebugLog.Visible = true;
			
			DebugLog.WriteLine("*** VRM Asset ***");
			DebugLog.WriteLine(vrm.Asset.ToString());
			DebugLog.WriteLine();
			int i = 0;
			foreach(var buffer in vrm.Buffers) {
				DebugLog.WriteLine(string.Format("Buffer #{0:000}", i));
				DebugLog.WriteLine(buffer.ToString());
				i++;
			}
			DebugLog.WriteLine();
			i = 0;
			foreach(var bufferView in vrm.BufferViews) {
				DebugLog.WriteLine(string.Format("BufferView #{0:000}", i));
				DebugLog.WriteLine(bufferView.ToString());
				i++;
			}
			DebugLog.WriteLine();
			i = 0;
			foreach(var accessor in vrm.Accessors) {
				DebugLog.WriteLine(string.Format("Accessor #{0:000}", i));
				DebugLog.WriteLine(accessor.ToString());
				i++;
			}
			DebugLog.WriteLine();
			i = 0;
			foreach(var scene in vrm.Scenes) {
				DebugLog.WriteLine(string.Format("Scene #{0:000}", i));
				DebugLog.WriteLine(scene.ToString());
				i++;
			}
			DebugLog.WriteLine();
			i = 0;
			foreach(var node in vrm.Nodes) {
				DebugLog.WriteLine(string.Format("Node #{0:000}", i));
				DebugLog.WriteLine(node.ToString());
				i++;
			}
			DebugLog.WriteLine();
			i = 0;
			foreach(var mesh in vrm.Meshes) {
				DebugLog.WriteLine(string.Format("Mesh #{0:000}", i));
				DebugLog.WriteLine(mesh.ToString());
				i++;
			}
			DebugLog.WriteLine();

			DebugLog.End();
		}
		#else 
		public void Print(VRM vrm) {
			Console.WriteLine("*** VRM Asset ***");
			Console.WriteLine(vrm.Asset.ToString());
			Console.WriteLine();
			int i = 0;
			foreach(var buffer in vrm.Buffers) {
				Console.WriteLine(string.Format("Buffer #{0:000}", i));
				Console.WriteLine(buffer.ToString());
				i++;
			}
			Console.WriteLine();
			i = 0;
			foreach(var bufferView in vrm.BufferViews) {
				Console.WriteLine(string.Format("BufferView #{0:000}", i));
				Console.WriteLine(bufferView.ToString());
				i++;
			}
			Console.WriteLine();
			i = 0;
			foreach(var accessor in vrm.Accessors) {
				Console.WriteLine(string.Format("Accessor #{0:000}", i));
				Console.WriteLine(accessor.ToString());
				i++;
			}
			Console.WriteLine();
			i = 0;
			foreach(var scene in vrm.Scenes) {
				Console.WriteLine(string.Format("Scene #{0:000}", i));
				Console.WriteLine(scene.ToString());
				i++;
			}
			Console.WriteLine();
			i = 0;
			foreach(var node in vrm.Nodes) {
				Console.WriteLine(string.Format("Node #{0:000}", i));
				Console.WriteLine(node.ToString());
				i++;
			}
			Console.WriteLine();
			i = 0;
			foreach(var mesh in vrm.Meshes) {
				Console.WriteLine(string.Format("Mesh #{0:000}", i));
				Console.WriteLine(mesh.ToString());
				i++;
			}
			Console.WriteLine();
		}
		#endif
	}
}
