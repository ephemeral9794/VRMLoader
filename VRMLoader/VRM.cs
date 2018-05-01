using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using static System.Math;
using Newtonsoft.Json.Linq;
using VRMLoader.Utility;
using static VRMLoader.GLenum;


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

	class VRMAsset {
		public string copyright;
		public string generator;
		public string version;
		public string minVersion;

		public VRMAsset() {
			copyright = "";
			generator = "";
			version = "";
			minVersion = "";
		}

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
		//public int mode;
		public GLenum mode;
		public Dictionary<string, int> attributes;
		public Dictionary<string, int>[] targets;

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("    indices  : {0}", indices).AppendLine();
			builder.AppendFormat("    material : {0}", material).AppendLine();
			builder.AppendFormat("    mode     : {0}", Enum.GetName(mode.GetType(), mode));
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
	class VRMNode {
		public string name;
		public int camera;
		public int mesh;
		public int skin;
		public int[] children;
		public double[] matrix;		// 16
		public double[] rotation;	// 4
		public double[] scale;		// 3
		public double[] translation;	// 3
		public double[] weights;

		public VRMNode() {
			name = "";
			camera = -1;
			mesh = -1;
			skin = -1;
			matrix = new [] {
				1.0, 0.0, 0.0, 0.0,
				0.0, 1.0, 0.0, 0.0,
				0.0, 0.0, 1.0, 0.0,
				0.0, 0.0, 0.0, 1.0,
			};
			rotation = new [] { 0.0, 0.0, 0.0, 1.0 };
			scale = new [] { 1.0, 1.0, 1.0 };
			translation = new [] { 0.0, 0.0, 0.0 };
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("  name             : {0}", name).AppendLine();
			builder.AppendFormat("  camera           : {0}", camera).AppendLine();
			builder.AppendFormat("  mesh             : {0}", mesh).AppendLine();
			builder.AppendFormat("  skin             : {0}", skin);
			for (int i = 0; i < children.Length; i++) {
				builder.AppendLine();
				builder.AppendFormat("  children #{0:000}    : {1}", i, children[i]);
			}
			for (int i = 0; i < matrix.Length; i++) {
				builder.AppendLine();
				builder.AppendFormat("  matrix #{0:000}      : {1:0.######}", i, matrix[i]);
			}
			for (int i = 0; i < rotation.Length; i++) {
				builder.AppendLine();
				builder.AppendFormat("  rotation #{0:000}    : {1:0.######}", i, rotation[i]);
			}
			for (int i = 0; i < scale.Length; i++) {
				builder.AppendLine();
				builder.AppendFormat("  scale #{0:000}       : {1:0.######}", i, scale[i]);
			}
			for (int i = 0; i < translation.Length; i++) {
				builder.AppendLine();
				builder.AppendFormat("  translation #{0:000} : {1:0.######}", i, translation[i]);
			}
			return builder.ToString();
		}
	}
	struct VRMBuffer {
		public string name;
		public uint byteLength;
		public byte[] data;

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("  name        : {0}", name).AppendLine();
			builder.AppendFormat("  byteLength  : {0}", byteLength).AppendLine();
			builder.AppendFormat("  data.Length : {0}", data.Length);
			return builder.ToString();
		}
	}
	struct VRMAccessor {
		public enum Type {
			Scalar,
			Vec2,
			Vec3,
			Vec4,
			Mat2,
			Mat3,
			Mat4
		}
		public string name;
		public int bufferView;
		public uint byteOffset;
		public GLenum componentType;
		public bool normalized;
		public uint count;
		public Type type;

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("  name          : {0}", name).AppendLine();
			builder.AppendFormat("  bufferView    : {0}", bufferView).AppendLine();
			builder.AppendFormat("  byteOffset    : {0}", byteOffset).AppendLine();
			builder.AppendFormat("  componentType : {0}", Enum.GetName(componentType.GetType(), componentType)).AppendLine();
			builder.AppendFormat("  normalized    : {0}", normalized).AppendLine();
			builder.AppendFormat("  count         : {0}", count).AppendLine();
			builder.AppendFormat("  type          : {0}", Enum.GetName(type.GetType(), type));
			return builder.ToString();
		}
	}
	struct VRMBufferView {
		public enum TargetType {
			None = 0,
			ArrayBuffer = 34962,
			ElementArrayBuffer = 34963,
		}
		public string name;
		public uint buffer; // Index to the buffer
		public uint byteOffset;
		public uint byteLength;
		public uint byteStride; // The stride, in bytes
		public TargetType target;

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("  name       : {0}", name).AppendLine();
			builder.AppendFormat("  buffer     : {0}", buffer).AppendLine();
			builder.AppendFormat("  byteOffset : {0}", byteOffset).AppendLine();
			builder.AppendFormat("  byteLength : {0}", byteLength).AppendLine();
			builder.AppendFormat("  byteStride : {0}", byteStride).AppendLine();
			builder.AppendFormat("  target     : {0}", Enum.GetName(target.GetType(), target));
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
					vrm.asset = new VRMAsset();
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
								int mode = (int)primitive["mode"];
								vrm.meshes[i].primitives[j].mode = (GLenum)mode;
							} else {
								vrm.meshes[i].primitives[j].mode = GL_TRIANGLES;
							}

							// attributes
							if (primitive.ContainsKey("attributes")) {
								JObject attributes = (JObject)primitive["attributes"];
								vrm.meshes[i].primitives[j].attributes = new Dictionary<string, int>();
								foreach (var at in attributes) {
									if (at.Value.Type == JTokenType.Integer) {
										vrm.meshes[i].primitives[j].attributes.Add(at.Key, (int)at.Value);
									}
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
							} else {
								vrm.meshes[i].primitives[j].targets = (Dictionary<string, int>[])Enumerable.Empty<Dictionary<string, int>>();
							}
						}
					}
				} else {
					vrm.meshes = (VRMMesh[])Enumerable.Empty<VRMMesh>();
				}

				// nodes
				if (root.ContainsKey("nodes")) {
					JArray nodes = (JArray)root["nodes"];
					vrm.nodes = new VRMNode[nodes.Count];

					for (int i = 0; i < nodes.Count; i++) {
						JObject node = (JObject)nodes[i];
						vrm.nodes[i] = new VRMNode();

						// name
						if (node.ContainsKey("name")) {
							vrm.nodes[i].name = (string)node["name"];
						} else {
							vrm.nodes[i].name = "";
						}

						// camera
						if (node.ContainsKey("camera")) {
							vrm.nodes[i].camera = (int)node["camera"];
						} else {
							vrm.nodes[i].camera = -1;
						}

						// children
						if (node.ContainsKey("children")) {
							JArray children = (JArray)node["children"];
							vrm.nodes[i].children = new int[children.Count];
							for (int j = 0; j < children.Count; j++) {
								vrm.nodes[i].children[j] = (int)children[j];
							}
						} else {
							vrm.nodes[i].children = (int[])Enumerable.Empty<int>();
						}

						// skin
						if (node.ContainsKey("skin")) {
							vrm.nodes[i].skin = (int)node["skin"];
						} else {
							vrm.nodes[i].skin = -1;
						}

						// mesh
						if (node.ContainsKey("mesh")) {
							vrm.nodes[i].mesh = (int)node["mesh"];
						} else {
							vrm.nodes[i].mesh = -1;
						}

						// matrix
						if (node.ContainsKey("matrix")) {
							JArray matrix = (JArray)node["matrix"];
							for (int j = 0; j < matrix.Count; j++) {
								vrm.nodes[i].matrix[j] = (double)matrix[j];
							}
						} else {
							// translation
							if (node.ContainsKey("translation")) {
								JArray translation = (JArray)node["translation"];
								for (int j = 0; j < translation.Count; j++) {
									vrm.nodes[i].translation[j] = (double)translation[j];
								}
							}
							// rotation
							if (node.ContainsKey("rotation")) {
								JArray rotation = (JArray)node["rotation"];
								for (int j = 0; j < rotation.Count; j++) {
									vrm.nodes[i].rotation[j] = (double)rotation[j];
								}
							}
							// scale
							if (node.ContainsKey("scale")) {
								JArray scale = (JArray)node["scale"];
								for (int j = 0; j < scale.Count; j++) {
									vrm.nodes[i].scale[j] = (double)scale[j];
								}
							}
						}

						// weights
						if (node.ContainsKey("weights")) {
							JArray weights = (JArray)node["weights"];
							vrm.nodes[i].weights = new double[weights.Count];
							for (int j = 0; j < weights.Count; j++) {
								vrm.nodes[i].weights[j] = (double)weights[j];
							}
						} else {
							vrm.nodes[i].weights = (double[])Enumerable.Empty<double>();
						}
					}
				} else {
					vrm.nodes = (VRMNode[])Enumerable.Empty<VRMNode>();
				}

				// buffers
				if (root.ContainsKey("buffers")) {
					JArray buffers = (JArray)root["buffers"];
					vrm.buffers = new VRMBuffer[buffers.Count];
					for (int i = 0; i < buffers.Count; i++) {
						JObject buffer = (JObject)buffers[i];
						vrm.buffers[i] = new VRMBuffer();
						Chunk bin = Chunk.Load(stream);
						/*if (bin.type != 0x204E4942) {	// 'BIN '
							throw new InvalidDataException("This BIN chunk is broken.");
						}*/

						// name
						if (buffer.ContainsKey("name")) {
							vrm.buffers[i].name = (string)buffer["name"];
						} else {
							vrm.buffers[i].name = "";
						}
						
						// byteLength
						if (buffer.ContainsKey("byteLength")) {
							vrm.buffers[i].byteLength = (uint)buffer["byteLength"];
						} else {
							vrm.buffers[i].byteLength = 0;
						}

						vrm.buffers[i].data = new byte[bin.length];
						Array.Copy(bin.data, vrm.buffers[i].data, bin.length);
					}
				} else {
					vrm.buffers = (VRMBuffer[])Enumerable.Empty<VRMBuffer>();
				}

				// accessors
				if (root.ContainsKey("accessors")) {
					JArray accessors = (JArray)root["accessors"];
					vrm.accessors = new VRMAccessor[accessors.Count];
					for (int i = 0; i < accessors.Count; i++) {
						JObject accessor = (JObject)accessors[i];
						vrm.accessors[i] = new VRMAccessor();
						
						// name
						if (accessor.ContainsKey("name")) {
							vrm.accessors[i].name = (string)accessor["name"];
						} else {
							vrm.accessors[i].name = "";
						}
						
						// bufferView
						if (accessor.ContainsKey("bufferView")) {
							vrm.accessors[i].bufferView = (int)accessor["bufferView"];
						} else {
							vrm.accessors[i].bufferView = -1;
						}

						// byteOffset
						if (accessor.ContainsKey("byteOffset")) {
							vrm.accessors[i].byteOffset = (uint)accessor["byteOffset"];
						} else {
							vrm.accessors[i].byteOffset = 0;
						}

						// componentType
						if (accessor.ContainsKey("componentType")) {
							int componentType = (int)accessor["byteOffset"];
							vrm.accessors[i].componentType = (GLenum)componentType;
						} else {
							vrm.accessors[i].componentType = GL_FLOAT;
						}

						// normalized
						if (accessor.ContainsKey("normalized")) {
							vrm.accessors[i].normalized = (bool)accessor["normalized"];
						} else {
							vrm.accessors[i].normalized = false;
						}

						// count
						if (accessor.ContainsKey("count")) {
							vrm.accessors[i].count = (uint)accessor["count"];
						} else {
							vrm.accessors[i].count = 0;
						}

						// type
						if (accessor.ContainsKey("type")) {
							string type = (string)accessor["type"];
							if (type == "SCALAR") {
								vrm.accessors[i].type = VRMAccessor.Type.Scalar;
							} else if (type == "VEC2") {
								vrm.accessors[i].type = VRMAccessor.Type.Vec2;
							} else if (type == "VEC3") {
								vrm.accessors[i].type = VRMAccessor.Type.Vec3;
							} else if (type == "VEC4") {
								vrm.accessors[i].type = VRMAccessor.Type.Vec4;
							} else if (type == "MAT2") {
								vrm.accessors[i].type = VRMAccessor.Type.Mat2;
							} else if (type == "MAT3") {
								vrm.accessors[i].type = VRMAccessor.Type.Mat3;
							} else if (type == "MAT4") {
								vrm.accessors[i].type = VRMAccessor.Type.Mat4;
							} else {
								throw new FormatException("accessors[i][type] is not a valid type");
							}
						} else {
							throw new FormatException("accessors[i][type] is not a valid type");
						}
					}
				} else {
					vrm.accessors = (VRMAccessor[])Enumerable.Empty<VRMAccessor>();
				}

				if (root.ContainsKey("bufferViews")) {
					JArray bufferViews = (JArray)root["bufferViews"];
					vrm.bufferViews = new VRMBufferView[bufferViews.Count];
					for (int i = 0; i < bufferViews.Count; i++) {
						JObject bufferView = (JObject)bufferViews[i];
						vrm.bufferViews[i] = new VRMBufferView();
						
						// name
						if (bufferView.ContainsKey("name")) {
							vrm.bufferViews[i].name = (string)bufferView["name"];
						} else {
							vrm.bufferViews[i].name = "";
						}
						
						// buffer
						if (bufferView.ContainsKey("buffer")) {
							vrm.bufferViews[i].buffer = (uint)bufferView["buffer"];
						} else {
							vrm.bufferViews[i].buffer = 0;
						}

						// byteOffset
						if (bufferView.ContainsKey("byteOffset")) {
							vrm.bufferViews[i].byteOffset = (uint)bufferView["byteOffset"];
						} else {
							vrm.bufferViews[i].byteOffset = 0;
						}

						// byteLength
						if (bufferView.ContainsKey("byteLength")) {
							vrm.bufferViews[i].byteLength = (uint)bufferView["byteLength"];
						} else {
							vrm.bufferViews[i].byteLength = 0;
						}

						// byteStride
						if (bufferView.ContainsKey("byteStride")) {
							vrm.bufferViews[i].byteStride = (uint)bufferView["byteStride"];
						} else {
							vrm.bufferViews[i].byteStride = 0;
						}

						// target
						if (bufferView.ContainsKey("target")) {
							int target = (int)bufferView["target"];
							vrm.bufferViews[i].target = (VRMBufferView.TargetType)target;
						} else {
							vrm.bufferViews[i].target = VRMBufferView.TargetType.None;
						}
					}
				} else {
					vrm.bufferViews = (VRMBufferView[])Enumerable.Empty<VRMBufferView>();
				}
			} else {
				throw new InvalidDataException("This JSON data does not contain a value.");
			}

			return vrm;
		}

		Header header;
		Chunk jsonChunk;

		VRMAsset asset;
		VRMScene[] scenes;
		VRMMesh[] meshes;
		VRMNode[] nodes;
		VRMBuffer[] buffers;
		VRMAccessor[] accessors;
		VRMBufferView[] bufferViews;

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
			DebugLog.WriteLine();
			i = 0;
			foreach(var node in nodes) {
				DebugLog.WriteLine(string.Format("Node #{0:000}", i));
				DebugLog.WriteLine(node.ToString());
				i++;
			}
			DebugLog.WriteLine();
			i = 0;
			foreach(var buffer in buffers) {
				DebugLog.WriteLine(string.Format("Buffer #{0:000}", i));
				DebugLog.WriteLine(buffer.ToString());
				i++;
			}
			DebugLog.WriteLine();
			i = 0;
			foreach(var accessor in accessors) {
				DebugLog.WriteLine(string.Format("Accessor #{0:000}", i));
				DebugLog.WriteLine(accessor.ToString());
				i++;
			}
			DebugLog.WriteLine();
			i = 0;
			foreach(var bufferView in bufferViews) {
				DebugLog.WriteLine(string.Format("BufferView #{0:000}", i));
				DebugLog.WriteLine(bufferView.ToString());
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
			i = 0;
			foreach(var mesh in meshes) {
				Console.WriteLine(string.Format("Mesh #{0:000}", i));
				Console.WriteLine(mesh.ToString());
				i++;
			}
			Console.WriteLine();
		}
		#endif
	}
}
