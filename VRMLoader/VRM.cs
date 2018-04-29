using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections;
using Newtonsoft.Json.Linq;
using VRMLoader.Utility;
using static VRMLoader.GLenum;
using System.Collections.Generic;

namespace VRMLoader
{
	public enum GLenum {
		GL_BYTE = 0x1400,
		GL_UNSIGNED_BYTE = 0x1401,
		GL_SHORT = 0x1402,
		GL_UNSIGNED_SHORT = 0x1403,
		GL_INT = 0x1404,
		GL_UNSIGNED_INT = 0x1405,
		GL_FLOAT = 0x1406,
		GL_2_BYTES = 0x1407,
		GL_3_BYTES = 0x1408,
		GL_4_BYTES = 0x1409,
		GL_DOUBLE = 0x140A,

		/* TextureMagFilter */
		GL_NEAREST = 0x2600,
		GL_LINEAR = 0x2601,

		/* TextureMinFilter */
		GL_NEAREST_MIPMAP_NEAREST = 0x2700,
		GL_LINEAR_MIPMAP_NEAREST = 0x2701,
		GL_NEAREST_MIPMAP_LINEAR = 0x2702,
		GL_LINEAR_MIPMAP_LINEAR = 0x2703,

		/* TextureWrapMode */
		GL_CLAMP = 0x2900,
		GL_REPEAT = 0x2901,

		/* BeginMode */
		GL_POINTS = 0x0000,
		GL_LINES = 0x0001,
		GL_LINE_LOOP = 0x0002,
		GL_LINE_STRIP = 0x0003,
		GL_TRIANGLES = 0x0004,
		GL_TRIANGLE_STRIP = 0x0005,
		GL_TRIANGLE_FAN = 0x0006,
		GL_QUADS = 0x0007,
		GL_QUAD_STRIP = 0x0008,
		GL_POLYGON = 0x0009,
	};

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
	struct VRMPrimitive {
		public int indices;
		public int material;
		public int mode;
		public Dictionary<string, int> attributes;
		public Dictionary<string, int>[] targets;

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("    indices  : {0}", indices).AppendLine();
			builder.AppendFormat("    material : {0}", material).AppendLine();
			builder.AppendFormat("    mode     : {0}", mode);
			foreach (var at in attributes) {
				builder.AppendLine();
				builder.AppendFormat("    attributes {0} : {1}", at.Key, at.Value);
			}
			for (int i = 0; i < targets.Length; i++) {
				builder.AppendLine();
				builder.AppendFormat("    targets #{0:000}", i);
				foreach (var at in targets[i]) {
					builder.AppendLine();
					builder.AppendFormat("      {0} : {1}", at.Key, at.Value);
				}
			}
			return builder.ToString();
		}
	}
	struct VRMMesh {
		public string name;
		public float[] weights;
		public VRMPrimitive[] primitives;

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("  name      : {0}", name);
			for (int i = 0; i < weights.Length; i++) {
				builder.AppendLine();
				builder.AppendFormat("  weight #{0:000} : {1}", i, weights[i]);
			}
			for (int i = 0; i < primitives.Length; i++) {
				builder.AppendLine();
				builder.AppendFormat("  primitive #{0:000}", i).AppendLine();
				builder.AppendFormat("{0}", primitives[i].ToString());
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
						vrm.asset.version = (string)asset["version"];
					} else {
						vrm.asset.version = "";
					}
					// copyright
					if (asset.ContainsKey("copyright")) {
						vrm.asset.copyright = (string)asset["copyright"];
					} else {
						vrm.asset.copyright = "";
					}
					// generator
					if (asset.ContainsKey("generator")) {
						vrm.asset.generator = (string)asset["generator"];
					} else {
						vrm.asset.generator = "";
					}
					// minVersion
					if (asset.ContainsKey("minVersion")) {
						vrm.asset.minVersion = (string)asset["minVersion"];
					} else {
						vrm.asset.minVersion = "";
					}
				}

				// scenes
				if (root.ContainsKey("scenes")) {
					JArray scenes = (JArray)root["scenes"];
					vrm.scenes = new VRMScene[scenes.Count];

					for(int i = 0; i < scenes.Count; i++) {
						JObject scene = (JObject)scenes[i];
						vrm.scenes[i] = new VRMScene();

						// name
						if (scene.ContainsKey("name")) {
							vrm.scenes[i].name = (string)scene["name"];
						} else {
							vrm.scenes[i].name = "";
						}

						// nodes
						if (scene.ContainsKey("nodes")) {
							JArray nodes = (JArray)scene["nodes"];
							vrm.scenes[i].nodes = new int[nodes.Count];
							for (int j = 0; j < nodes.Count; j++) {
								vrm.scenes[i].nodes[j] = (int)nodes[j];
							}
						} else {
							vrm.scenes[i].nodes = (int[])Enumerable.Empty<int>();
						}
					}
				} else {
					vrm.scenes = (VRMScene[])Enumerable.Empty<VRMScene>();
				}

				// meshes
				if (root.ContainsKey("meshes")) {
					JArray meshes = (JArray)root["meshes"];
					vrm.meshes = new VRMMesh[meshes.Count];

					for (int i = 0; i < meshes.Count; i++) {
						JObject mesh = (JObject)meshes[i];
						vrm.meshes[i] = new VRMMesh();

						// name
						if (mesh.ContainsKey("name")) {
							vrm.meshes[i].name = (string)mesh["name"];
						} else {
							vrm.meshes[i].name = "";
						}

						// weights
						if (mesh.ContainsKey("weights")) {
							JArray weights = (JArray)mesh["weights"];
							vrm.meshes[i].weights = new float[weights.Count];
							for (int j = 0; j < weights.Count; j++) {
								vrm.meshes[i].weights[j] = (float)weights[j];
							}
						} else {
							vrm.meshes[i].weights = (float[])Enumerable.Empty<float>();
						}

						// primitives
						JArray primitives = (JArray)mesh["primitives"];
						vrm.meshes[i].primitives = new VRMPrimitive[primitives.Count];
						for (int j = 0; j < primitives.Count; j++) {
							JObject primitive = (JObject)primitives[j];

							// indices
							if (primitive.ContainsKey("indices")) {
								vrm.meshes[i].primitives[j].indices = (int)primitive["indices"];
							} else {
								vrm.meshes[i].primitives[j].indices = -1;
							}

							// material
							if (primitive.ContainsKey("material")) {
								vrm.meshes[i].primitives[j].material = (int)primitive["material"];
							} else {
								vrm.meshes[i].primitives[j].material = -1;
							}

							// mode
							if (primitive.ContainsKey("mode")) {
								vrm.meshes[i].primitives[j].mode = (int)primitive["mode"];
							} else {
								vrm.meshes[i].primitives[j].mode = -1;
							}

							// attributes
							if (primitive.ContainsKey("attributes")) {
								JObject attributes = (JObject)primitive["attributes"];
								vrm.meshes[i].primitives[j].attributes = new Dictionary<string, int>();
								foreach (var at in attributes) {
									vrm.meshes[i].primitives[j].attributes.Add(at.Key, (int)at.Value);
								}
							} else {
								vrm.meshes[i].primitives[j].attributes = new Dictionary<string, int>();
							}

							// targets
							if (primitive.ContainsKey("targets")) {
								JArray targets = (JArray)primitive["targets"];
								vrm.meshes[i].primitives[j].targets = new Dictionary<string, int>[targets.Count];
								for (int n = 0; n < targets.Count; n++) {
									JObject target = (JObject)targets[n];
									vrm.meshes[i].primitives[j].targets[n] = new Dictionary<string, int>();
									foreach(var t in target) {
										if (t.Value.Type == JTokenType.Integer) {
											vrm.meshes[i].primitives[j].targets[n].Add(t.Key, (int)(t.Value));
										}
									}
								}
							}
						}
					}
				} else {
					vrm.meshes = (VRMMesh[])Enumerable.Empty<VRMMesh>();
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
		VRMMesh[] meshes;

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
			i = 0;
			foreach(var mesh in meshes) {
				DebugLog.WriteLine(string.Format("Mesh #{0:000}", i));
				DebugLog.WriteLine(mesh.ToString());
				i++;
			}

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
