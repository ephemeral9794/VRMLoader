using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using VRMLoader.Utility;

namespace VRMLoader
{
	public class Loader
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
		
		public static string SerializeModelInfo(ModelInfo info) {
			var serializer = new JsonSerializer() { 
				NullValueHandling = NullValueHandling.Ignore,
				Formatting = Formatting.Indented
			};
			var builder = new StringBuilder();
			serializer.Serialize(new JsonTextWriter(new StringWriter(builder)), info);
			return builder.ToString();
		}
#if DEBUG
		public static void PrintModelInfo(ModelInfo info) {
			DebugLog.Start();
			DebugLog.Visible = true;

			// asset
			DebugLog.WriteLine("== Asset ==");
			DebugLog.WriteLine(string.Format("copyright  : {0}", info.Asset.Copyright));
			DebugLog.WriteLine(string.Format("generator  : {0}", info.Asset.Generator));
			DebugLog.WriteLine(string.Format("version    : {0}", info.Asset.Version));
			DebugLog.WriteLine(string.Format("minVersion : {0}", info.Asset.MinVersion));
			DebugLog.WriteLine();

			// buffers
			int i = 0;
			foreach (var buffer in info.Buffers) {
				DebugLog.WriteLine(string.Format("== Buffer #{0:000} ==", i));
				DebugLog.WriteLine(string.Format("byteLength : {0}", buffer.ByteLength));
				i++;
			}
			DebugLog.WriteLine();

			// bufferViews
			i = 0;
			foreach (var view in info.BufferViews) {
				DebugLog.WriteLine(string.Format("== BufferView #{0:000} ==", i));
				DebugLog.WriteLine(string.Format("buffer     : {0}", view.Buffer));
				DebugLog.WriteLine(string.Format("byteOffset : {0}", view.ByteOffset));
				DebugLog.WriteLine(string.Format("byteLength : {0}", view.ByteLength));
				DebugLog.WriteLine(string.Format("target     : {0}", view.Target == 0 ? "other" : Enum.GetName(((GLenum)view.Target).GetType(), (GLenum)view.Target)));
				DebugLog.WriteLine(string.Format("byteStride : {0}", view.ByteStride));
				i++;
			}
			DebugLog.WriteLine();

			// accessors
			i = 0;
			foreach (var accessor in info.Accessors) {
				DebugLog.WriteLine(string.Format("== Accessor #{0:000} ==", i));
				DebugLog.WriteLine(string.Format("bufferView    : {0}", accessor.BufferView));
				DebugLog.WriteLine(string.Format("byteOffset    : {0}", accessor.ByteOffset));
				DebugLog.WriteLine(string.Format("type          : {0}", accessor.Type));
				DebugLog.WriteLine(string.Format("componentType : {0}", Enum.GetName(((GLenum)accessor.ComponentType).GetType(), (GLenum)accessor.ComponentType)));
				DebugLog.WriteLine(string.Format("count         : {0}", accessor.Count));
				i++;
			}
			DebugLog.WriteLine();

			// images
			i = 0;
			foreach (var image in info.Images) {
				DebugLog.WriteLine(string.Format("== Image #{0:000} ==", i));
				DebugLog.WriteLine(string.Format("name       : {0}", image.Extra.Name));
				DebugLog.WriteLine(string.Format("bufferView : {0}", image.BufferView));
				DebugLog.WriteLine(string.Format("mimeType   : {0}", image.MimeType));
				i++;
			}
			DebugLog.WriteLine();

			// samplers
			i = 0;
			foreach (var sampler in info.Samplers) {
				DebugLog.WriteLine(string.Format("== Sampler #{0:000} ==", i));
				DebugLog.WriteLine(string.Format("magFilter : {0}", sampler.MagFilter));
				DebugLog.WriteLine(string.Format("minFilter : {0}", sampler.MinFilter));
				DebugLog.WriteLine(string.Format("wrapS     : {0}", sampler.WrapS));
				DebugLog.WriteLine(string.Format("wrapT     : {0}", sampler.WrapT));
				i++;
			}
			DebugLog.WriteLine();

			// textures
			i = 0;
			foreach (var texture in info.Textures) {
				DebugLog.WriteLine(string.Format("== Texture #{0:000} ==", i));
				DebugLog.WriteLine(string.Format("sampler : {0}", texture.Sampler));
				DebugLog.WriteLine(string.Format("source  : {0}", texture.Source));
				i++;
			}
			DebugLog.WriteLine();

			// materials
			i = 0;
			foreach (var material in info.Materials) {
				DebugLog.WriteLine(string.Format("== Material #{0:000} ==", i));
				DebugLog.WriteLine(string.Format("name            : {0}", material.Name));
				DebugLog.WriteLine(string.Format("index           : {0}", material.PbrMetallicRoughness.BaseColorTexture.Index));
				DebugLog.WriteLine(string.Format("texCoord        : {0}", material.PbrMetallicRoughness.BaseColorTexture.TexCoord));
				DebugLog.WriteLine(string.Format("metallicFactor  : {0}", material.PbrMetallicRoughness.MetallicFactor));
				DebugLog.WriteLine(string.Format("roughnessFactor : {0}", material.PbrMetallicRoughness.RoughnessFactor));
				i++;
			}
			DebugLog.WriteLine();
			
			// meshes
			i = 0;
			foreach (var mesh in info.Meshes) {
				DebugLog.WriteLine(string.Format("== Mesh #{0:000} ==", i));
				DebugLog.WriteLine(string.Format("name : {0}", mesh.Name));
				int j = 0;
				foreach(var primitive in mesh.Primitives) {
					DebugLog.WriteLine(string.Format("-- Primitive #{0:000} --", j));
					DebugLog.WriteLine(string.Format("  mode     : {0}", Enum.GetName(((GLenum)primitive.Mode).GetType(), (GLenum)primitive.Mode)));
					DebugLog.WriteLine(string.Format("  indices  : {0}", primitive.Indices));
					DebugLog.WriteLine(string.Format("  -- attributes --"));
					foreach(var attribute in primitive.Attributes) {
						DebugLog.WriteLine(string.Format("    {0:-10} : {1}", attribute.Key, attribute.Value));
					}
					DebugLog.WriteLine(string.Format("  material : {0}", primitive.Material));
					int k = 0;
					foreach(var target in primitive.Targets) {
						DebugLog.WriteLine(string.Format("  -- targets #{0:000} --", k));
						DebugLog.WriteLine(string.Format("    name       : {0}", target.Extra.Name));
						DebugLog.WriteLine(string.Format("    POSITION   : {0}", target.POSITION));
						DebugLog.WriteLine(string.Format("    NORMAL     : {0}", target.NORMAL));
						DebugLog.WriteLine(string.Format("    TANGENT    : {0}", target.TANGENT));
						DebugLog.WriteLine(string.Format("    TEXCOORD_0 : {0}", target.TEXCOORD_0));
						DebugLog.WriteLine(string.Format("    JOINT_0    : {0}", target.JOINTS_0));
						DebugLog.WriteLine(string.Format("    WEIGHTS_0  : {0}", target.WEIGHTS_0));
						k++;
					}
					j++;
				}
				i++;
			}
			DebugLog.WriteLine();

			// skins
			i = 0;
			foreach (var skin in info.Skins) {
				DebugLog.WriteLine(string.Format("== Skin #{0:000} ==", i));
				DebugLog.WriteLine(string.Format("inverseBindMatrices : {0}", skin.InverseBindMatrices));
				if (skin.Joints != null) {
					int j = 0;
					foreach(var joint in skin.Joints) {
						DebugLog.WriteLine(string.Format("joint#{0:000} : {1}", j, joint));
						j++;
					}
				}
				i++;
			}
			DebugLog.WriteLine();

			// nodes
			i = 0;
			foreach (var node in info.Nodes) {
				DebugLog.WriteLine(string.Format("== Node #{0:000} ==", i));
				DebugLog.WriteLine(string.Format("name         : {0}", node.Name));
				DebugLog.WriteLine(string.Format("translation  : [{0},{1},{2}]", node.Translation[0], node.Translation[1], node.Translation[2]));
				DebugLog.WriteLine(string.Format("rotation     : [{0},{1},{2},{3}]", node.Rotation[0], node.Rotation[1], node.Rotation[2], node.Rotation[3]));
				DebugLog.WriteLine(string.Format("scale        : [{0},{1},{2}]", node.Scale[0], node.Scale[1], node.Scale[2]));
				DebugLog.WriteLine(string.Format("mesh         : {0}", node.Mesh));
				DebugLog.WriteLine(string.Format("skin         : {0}", node.Skin));
				if (node.Children != null) {
					int j = 0;
					foreach(var child in node.Children) {
						DebugLog.WriteLine(string.Format("children#{0:000} : {1}", j, child));
						j++;
					}
				}
				i++;
			}
			DebugLog.WriteLine();

			// scenes
			i = 0;
			foreach (var scene in info.Scenes) {
				DebugLog.WriteLine(string.Format("== Scene #{0:000} ==", i));
				foreach (var node in scene.Nodes) {
					DebugLog.WriteLine(string.Format("node : {0}", node));
				}
				i++;
			}
			DebugLog.WriteLine();

			DebugLog.End();
		}
#else
		public static void PrintModelInfo(ModelInfo info) {
			// asset
			Console.WriteLine("== Asset ==");
			Console.WriteLine(string.Format("copyright  : {0}", info.Asset.Copyright));
			Console.WriteLine(string.Format("generator  : {0}", info.Asset.Generator));
			Console.WriteLine(string.Format("version    : {0}", info.Asset.Version));
			Console.WriteLine(string.Format("minVersion : {0}", info.Asset.MinVersion));
			Console.WriteLine();

			// buffers
			int i = 0;
			foreach (var buffer in info.Buffers) {
				Console.WriteLine(string.Format("== Buffer #{0:000} ==", i));
				Console.WriteLine(string.Format("byteLength : {0}", buffer.ByteLength));
				i++;
			}
			Console.WriteLine();

			// bufferViews
			i = 0;
			foreach (var view in info.BufferViews) {
				Console.WriteLine(string.Format("== BufferView #{0:000} ==", i));
				Console.WriteLine(string.Format("buffer     : {0}", view.Buffer));
				Console.WriteLine(string.Format("byteOffset : {0}", view.ByteOffset));
				Console.WriteLine(string.Format("byteLength : {0}", view.ByteLength));
				Console.WriteLine(string.Format("target     : {0}", view.Target == 0 ? "other" : Enum.GetName(((GLenum)view.Target).GetType(), (GLenum)view.Target)));
				Console.WriteLine(string.Format("byteStride : {0}", view.ByteStride));
				i++;
			}
			Console.WriteLine();

			// accessors
			i = 0;
			foreach (var accessor in info.Accessors) {
				Console.WriteLine(string.Format("== Accessor #{0:000} ==", i));
				Console.WriteLine(string.Format("bufferView    : {0}", accessor.BufferView));
				Console.WriteLine(string.Format("byteOffset    : {0}", accessor.ByteOffset));
				Console.WriteLine(string.Format("type          : {0}", accessor.Type));
				Console.WriteLine(string.Format("componentType : {0}", Enum.GetName(((GLenum)accessor.ComponentType).GetType(), (GLenum)accessor.ComponentType)));
				Console.WriteLine(string.Format("count         : {0}", accessor.Count));
				i++;
			}
			Console.WriteLine();

			// images
			i = 0;
			foreach (var image in info.Images) {
				Console.WriteLine(string.Format("== Image #{0:000} ==", i));
				Console.WriteLine(string.Format("name       : {0}", image.Extra.Name));
				Console.WriteLine(string.Format("bufferView : {0}", image.BufferView));
				Console.WriteLine(string.Format("mimeType   : {0}", image.MimeType));
				i++;
			}
			Console.WriteLine();

			// samplers
			i = 0;
			foreach (var sampler in info.Samplers) {
				Console.WriteLine(string.Format("== Sampler #{0:000} ==", i));
				Console.WriteLine(string.Format("magFilter : {0}", sampler.MagFilter));
				Console.WriteLine(string.Format("minFilter : {0}", sampler.MinFilter));
				Console.WriteLine(string.Format("wrapS     : {0}", sampler.WrapS));
				Console.WriteLine(string.Format("wrapT     : {0}", sampler.WrapT));
				i++;
			}
			Console.WriteLine();

			// textures
			i = 0;
			foreach (var texture in info.Textures) {
				Console.WriteLine(string.Format("== Texture #{0:000} ==", i));
				Console.WriteLine(string.Format("sampler : {0}", texture.Sampler));
				Console.WriteLine(string.Format("source  : {0}", texture.Source));
				i++;
			}
			Console.WriteLine();

			// materials
			i = 0;
			foreach (var material in info.Materials) {
				Console.WriteLine(string.Format("== Material #{0:000} ==", i));
				Console.WriteLine(string.Format("name            : {0}", material.Name));
				Console.WriteLine(string.Format("index           : {0}", material.PbrMetallicRoughness.BaseColorTexture.Index));
				Console.WriteLine(string.Format("texCoord        : {0}", material.PbrMetallicRoughness.BaseColorTexture.TexCoord));
				Console.WriteLine(string.Format("metallicFactor  : {0}", material.PbrMetallicRoughness.MetallicFactor));
				Console.WriteLine(string.Format("roughnessFactor : {0}", material.PbrMetallicRoughness.RoughnessFactor));
				i++;
			}
			Console.WriteLine();
			
			// meshes
			i = 0;
			foreach (var mesh in info.Meshes) {
				Console.WriteLine(string.Format("== Mesh #{0:000} ==", i));
				Console.WriteLine(string.Format("name : {0}", mesh.Name));
				int j = 0;
				foreach(var primitive in mesh.Primitives) {
					Console.WriteLine(string.Format("-- Primitive #{0:000} --", j));
					Console.WriteLine(string.Format("  mode     : {0}", Enum.GetName(((GLenum)primitive.Mode).GetType(), (GLenum)primitive.Mode)));
					Console.WriteLine(string.Format("  indices  : {0}", primitive.Indices));
					Console.WriteLine(string.Format("  -- attributes --"));
					foreach(var attribute in primitive.Attributes) {
						Console.WriteLine(string.Format("    {0:-10} : {1}", attribute.Key, attribute.Value));
					}
					Console.WriteLine(string.Format("  material : {0}", primitive.Material));
					int k = 0;
					foreach(var target in primitive.Targets) {
						Console.WriteLine(string.Format("  -- targets #{0:000} --", k));
						Console.WriteLine(string.Format("    name       : {0}", target.Extra.Name));
						Console.WriteLine(string.Format("    POSITION   : {0}", target.POSITION));
						Console.WriteLine(string.Format("    NORMAL     : {0}", target.NORMAL));
						Console.WriteLine(string.Format("    TANGENT    : {0}", target.TANGENT));
						Console.WriteLine(string.Format("    TEXCOORD_0 : {0}", target.TEXCOORD_0));
						Console.WriteLine(string.Format("    JOINT_0    : {0}", target.JOINTS_0));
						Console.WriteLine(string.Format("    WEIGHTS_0  : {0}", target.WEIGHTS_0));
						k++;
					}
					j++;
				}
				i++;
			}
			Console.WriteLine();

			// skins
			i = 0;
			foreach (var skin in info.Skins) {
				Console.WriteLine(string.Format("== Skin #{0:000} ==", i));
				Console.WriteLine(string.Format("inverseBindMatrices : {0}", skin.InverseBindMatrices));
				if (skin.Joints != null) {
					int j = 0;
					foreach(var joint in skin.Joints) {
						Console.WriteLine(string.Format("joint#{0:000} : {1}", j, joint));
						j++;
					}
				}
				i++;
			}
			Console.WriteLine();

			// nodes
			i = 0;
			foreach (var node in info.Nodes) {
				Console.WriteLine(string.Format("== Node #{0:000} ==", i));
				Console.WriteLine(string.Format("name         : {0}", node.Name));
				Console.WriteLine(string.Format("translation  : [{0},{1},{2}]", node.Translation[0], node.Translation[1], node.Translation[2]));
				Console.WriteLine(string.Format("rotation     : [{0},{1},{2},{3}]", node.Rotation[0], node.Rotation[1], node.Rotation[2], node.Rotation[3]));
				Console.WriteLine(string.Format("scale        : [{0},{1},{2}]", node.Scale[0], node.Scale[1], node.Scale[2]));
				Console.WriteLine(string.Format("mesh         : {0}", node.Mesh));
				Console.WriteLine(string.Format("skin         : {0}", node.Skin));
				if (node.Children != null) {
					int j = 0;
					foreach(var child in node.Children) {
						Console.WriteLine(string.Format("children#{0:000} : {1}", j, child));
						j++;
					}
				}
				i++;
			}
			Console.WriteLine();

			// scenes
			i = 0;
			foreach (var scene in info.Scenes) {
				Console.WriteLine(string.Format("== Scene #{0:000} ==", i));
				foreach (var node in scene.Nodes) {
					Console.WriteLine(string.Format("node : {0}", node));
				}
				i++;
			}
			Console.WriteLine();
		}
#endif
	}
}
