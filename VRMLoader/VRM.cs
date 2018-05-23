using System;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
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

		/* BufferTarget */
		GL_ARRAY_BUFFER = 0x8892,
		GL_ELEMENT_ARRAY_BUFFER = 0x8893,

		None = -1,
	};

	public struct VRMAsset {
		private string copyright;
		private string generator;
		private string version;
		private string minVersion;

		// property
		public string Copyright {
			get { return copyright; }
			set { copyright = value; }
		}
		public string Generator {
			get { return generator; }
			set { generator = value; }
		}
		public string Version {
			get { return version; }
			set { version = value; }
		}
		public string MinVersion {
			get { return minVersion; }
			set { minVersion = value; }
		}

		public VRMAsset(JObject obj) {
			// version
			if (obj.ContainsKey("version")) {
				version = (string)obj["version"];
			} else {
				version = "";
			}
			// copyright
			if (obj.ContainsKey("copyright")) {
				copyright = (string)obj["copyright"];
			} else {
				copyright = "";
			}
			// generator
			if (obj.ContainsKey("generator")) {
				generator = (string)obj["generator"];
			} else {
				generator = "";
			}
			// minVersion
			if (obj.ContainsKey("minVersion")) {
				minVersion = (string)obj["minVersion"];
			} else {
				minVersion = "";
			}
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
	public struct VRMBuffer {
		public uint byteLength;
		public byte[] data;
		
		public uint ByteLength {
			get { return byteLength; }
			set { byteLength = value; }
		}
		public byte[] Data {
			get { return data; }
			set { data = value; }
		}

		public VRMBuffer(JObject obj) {
			// byteLength
			if (obj.ContainsKey("byteLength")) {
				byteLength = (uint)obj["byteLength"];
			} else {
				byteLength = 0;
			}
			data = new byte[0];
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("  byteLength  : {0}", byteLength).AppendLine();
			builder.AppendFormat("  data.Length : {0}", data.Length);
			return builder.ToString();
		}
	}
	public struct VRMBufferView {
		private uint buffer; // Index to the buffer
		private uint byteOffset;
		private uint byteLength;
		private uint byteStride; // The stride, in bytes
		private GLenum target;
		
		public uint Buffer {
			get { return buffer; }
			set { buffer = value; }
		}
		public uint ByteOffset {
			get { return byteOffset; }
			set { byteOffset = value; }
		}
		public uint ByteLength {
			get { return byteLength; }
			set { byteLength = value; }
		}
		public uint ByteStride {
			get { return byteStride; }
			set { byteStride = value; }
		}
		public GLenum Target {
			get { return target; }
			set { target = value; }
		}

		public VRMBufferView(JObject obj) {
			// buffer
			if (obj.ContainsKey("buffer")) {
				buffer = (uint)obj["buffer"];
			} else {
				buffer = 0;
			}
			// byteOffset
			if (obj.ContainsKey("byteOffset")) {
				byteOffset = (uint)obj["byteOffset"];
			} else {
				byteOffset = 0;
			}
			// byteLength
			if (obj.ContainsKey("byteLength")) {
				byteLength = (uint)obj["byteLength"];
			} else {
				byteLength = 0;
			}
			// byteStride
			if (obj.ContainsKey("byteStride")) {
				byteStride = (uint)obj["byteStride"];
			} else {
				byteStride = 0;
			}
			// target
			if (obj.ContainsKey("target")) {
				int t = (int)obj["target"];
				target = (GLenum)t;
			} else {
				target = None;
			}
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("  buffer     : {0}", buffer).AppendLine();
			builder.AppendFormat("  byteOffset : {0}", byteOffset).AppendLine();
			builder.AppendFormat("  byteLength : {0}", byteLength).AppendLine();
			builder.AppendFormat("  byteStride : {0}", byteStride).AppendLine();
			builder.AppendFormat("  target     : {0}", Enum.GetName(target.GetType(), target));
			return builder.ToString();
		}
	}
	public struct VRMAccessor {
		private int bufferView;
		private uint byteOffset;
		private GLenum componentType;
		private uint count;
		private string type;

		public int BufferView {
			get { return bufferView; }
			set { bufferView = value; }
		}
		public uint ByteOffset {
			get { return byteOffset; }
			set { byteOffset = value; }
		}
		public GLenum ComponentType {
			get { return componentType; }
			set { componentType = value; }
		}
		public uint Count {
			get { return count; }
			set { count = value; }
		}
		public string Type {
			get { return type; }
			set { type = value; }
		}

		public VRMAccessor(JObject obj) {
			// bufferView
			if (obj.ContainsKey("bufferView")) {
				bufferView = (int)obj["bufferView"];
			} else {
				bufferView = -1;
			}
			// byteOffset
			if (obj.ContainsKey("byteOffset")) {
				byteOffset = (uint)obj["byteOffset"];
			} else {
				byteOffset = 0;
			}
			// componentType
			if (obj.ContainsKey("componentType")) {
				int c = (int)obj["byteOffset"];
				componentType = (GLenum)c;
			} else {
				componentType = None;
			}
			// count
			if (obj.ContainsKey("count")) {
				count = (uint)obj["count"];
			} else {
				count = 0;
			}
			// type
			if (obj.ContainsKey("type")) {
				type = (string)obj["type"];				
			} else {
				type = "";
			}
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("  bufferView    : {0}", bufferView).AppendLine();
			builder.AppendFormat("  byteOffset    : {0}", byteOffset).AppendLine();
			builder.AppendFormat("  componentType : {0}", Enum.GetName(componentType.GetType(), componentType)).AppendLine();
			builder.AppendFormat("  count         : {0}", count).AppendLine();
			builder.AppendFormat("  type          : {0}", type);
			return builder.ToString();
		}
	}
	public struct VRMScene {
		private string name;
		private int[] nodes;

		public string Name {
			get { return name; }
			set { name = value; }
		}
		public int[] Nodes {
			get { return nodes; }
			set { nodes = value; }
		}

		public VRMScene(JObject obj) {
			// name
			if (obj.ContainsKey("name")) {
				name = (string)obj["name"];
			} else {
				name = "";
			}
			// nodes
			if (obj.ContainsKey("nodes")) {
				JArray node = (JArray)obj["nodes"];
				nodes = new int[node.Count];
				for (int j = 0; j < node.Count; j++) {
					nodes[j] = (int)node[j];
				}
			} else {
				nodes = (int[])Enumerable.Empty<int>();
			}
		}

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
	public struct VRMNode {
		private string name;
		private int camera;
		private int mesh;
		private int skin;
		private List<int> children;
		private double[] matrix;		// 16
		private double[] rotation;	// 4
		private double[] scale;		// 3
		private double[] translation;	// 3
		private List<double> weights;

		public string Name {
			get { return name; }
			set { name = value; }
		}
		public int Camera {
			get { return camera; }
			set { camera = value; }
		}
		public int Mesh {
			get { return mesh; }
			set { mesh = value; }
		}
		public int Skin {
			get { return skin; }
			set { skin = value; }
		}
		public List<int> Children {
			get { return children; }
			set { children = value; }
		}
		public double[] Matrix {
			get { return matrix; }
			set { matrix = value; }
		}
		public double[] Translation {
			get { return translation; }
			set { translation = value; }
		}
		public double[] Rotation {
			get { return rotation; }
			set { rotation = value; }
		}
		public double[] Scale {
			get { return scale; }
			set { scale = value; }
		}
		public List<double> Weights {
			get { return weights; }
			set { weights = value; }
		}

		public VRMNode(JObject obj) {
			// name
			if (obj.ContainsKey("name")) {
				name = (string)obj["name"];
			} else {
				name = "";
			}
			// camera
			if (obj.ContainsKey("camera")) {
				camera = (int)obj["camera"];
			} else {
				camera = -1;
			}
			// children
			children = new List<int>();
			if (obj.ContainsKey("children")) {
				JArray child = (JArray)obj["children"];
				foreach (var c in child) {
					children.Add((int)c);
				}
			}
			// skin
			if (obj.ContainsKey("skin")) {
				skin = (int)obj["skin"];
			} else {
				skin = -1;
			}
			// mesh
			if (obj.ContainsKey("mesh")) {
				mesh = (int)obj["mesh"];
			} else {
				mesh = -1;
			}
			// matrix
			if (obj.ContainsKey("matrix")) {
				JArray mat = (JArray)obj["matrix"];
				matrix = new double[mat.Count];
				for (int j = 0; j < mat.Count; j++) {
					matrix[j] = (double)mat[j];
				}
				translation = new double[] { 0.0, 0.0, 0.0 };
				rotation = new double[] { 0.0, 0.0, 0.0, 1.0 };
				scale = new double[] { 1.0, 1.0, 1.0 };
			} else {
				matrix = new double[] {
					1.0, 0.0, 0.0, 0.0,
					0.0, 1.0, 0.0, 0.0,
					0.0, 0.0, 1.0, 0.0,
					0.0, 0.0, 0.0, 1.0
				};
				// translation
				if (obj.ContainsKey("translation")) {
					JArray trans = (JArray)obj["translation"];
					translation = new double[trans.Count];
					for (int j = 0; j < trans.Count; j++) {
						translation[j] = (double)trans[j];
					}
				} else {
					translation = new double[] { 0.0, 0.0, 0.0 };
				}
				// rotation
				if (obj.ContainsKey("rotation")) {
					JArray rotate = (JArray)obj["rotation"];
					rotation = new double[rotate.Count];
					for (int j = 0; j < rotate.Count; j++) {
						rotation[j] = (double)rotate[j];
					}
				} else {
					rotation = new double[] { 0.0, 0.0, 0.0, 1.0 };
				}
				// scale
				if (obj.ContainsKey("scale")) {
					JArray sc = (JArray)obj["scale"];
					scale = new double[sc.Count];
					for (int j = 0; j < sc.Count; j++) {
						scale[j] = (double)sc[j];
					}
				} else {
					scale = new double[] { 1.0, 1.0, 1.0 };
				}
			}
			// weights
			weights = new List<double>();
			if (obj.ContainsKey("weights")) {
				JArray weight = (JArray)obj["weights"];
				foreach (var w in weight) {
					weights.Add((double)w);
				}
			}
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("  name             : {0}", name).AppendLine();
			builder.AppendFormat("  camera           : {0}", camera).AppendLine();
			builder.AppendFormat("  mesh             : {0}", mesh).AppendLine();
			builder.AppendFormat("  skin             : {0}", skin);
			for (int i = 0; i < children.Count; i++) {
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
	public struct VRMPrimitive {
		public int indices;
		public int material;
		//public int mode;
		public GLenum mode;
		public Dictionary<string, int> attributes;
		public List<Dictionary<string, int>> targets;

		public VRMPrimitive(JObject obj) {
			// indices
			if (obj.ContainsKey("indices")) {
				indices = (int)obj["indices"];
			} else {
				indices = -1;
			}
			// material
			if (obj.ContainsKey("material")) {
				material = (int)obj["material"];
			} else {
				material = -1;
			}
			// mode
			if (obj.ContainsKey("mode")) {
				int m = (int)obj["mode"];
				mode = (GLenum)m;
			} else {
				mode = GL_TRIANGLES;
			}
			// attributes
			attributes = new Dictionary<string, int>();
			if (obj.ContainsKey("attributes")) {
				JObject attribute = (JObject)obj["attributes"];
				foreach (var at in attribute) {
					if (at.Value.Type == JTokenType.Integer) {
						attributes.Add(at.Key, (int)at.Value);
					}
				}
			}
			// targets
			targets = new List<Dictionary<string, int>>();
			if (obj.ContainsKey("targets")) {
				JArray tg = (JArray)obj["targets"];
				foreach (JObject t in tg) {
					Dictionary<string, int> target = new Dictionary<string, int>();
					foreach(var at in t) {
						if (at.Value.Type == JTokenType.Integer) {
							target.Add(at.Key, (int)(at.Value));
						}
					}
					targets.Add(target);
				}
			}
		}

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
			for (int i = 0; i < targets.Count; i++) {
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
	public struct VRMMesh {
		public string name;
		public List<double> weights;
		public List<VRMPrimitive> primitives;

		public VRMMesh(JObject obj) {
			// name
			if (obj.ContainsKey("name")) {
				name = (string)obj["name"];
			} else {
				name = "";
			}
			// weights
			weights = new List<double>();
			if (obj.ContainsKey("weights")) {
				JArray weight = (JArray)obj["weights"];
				for (int j = 0; j < weight.Count; j++) {
					weights.Add((double)weight[j]);
				}
			}
			// primitives
			JArray primitive = (JArray)obj["primitives"];
			primitives = new List<VRMPrimitive>();
			foreach (JObject p in primitive) {
				primitives.Add(new VRMPrimitive(p));
			}
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("  name      : {0}", name);
			for (int i = 0; i < weights.Count; i++) {
				builder.AppendLine();
				builder.AppendFormat("  weight #{0:000} : {1}", i, weights[i]);
			}
			for (int i = 0; i < primitives.Count; i++) {
				builder.AppendLine();
				builder.AppendFormat("  primitive #{0:000}", i).AppendLine();
				builder.AppendFormat("{0}", primitives[i].ToString());
			}
			return builder.ToString();
		}
	}

	public class VRM
	{
		VRMAsset asset;
		List<VRMBuffer> buffers;
		List<VRMBufferView> bufferViews;
		List<VRMAccessor> accessors;
		List<VRMScene> scenes;
		List<VRMNode> nodes;
		List<VRMMesh> meshes;

		public VRMAsset Asset {
			get { return asset; }
			set { asset = value; }
		}
		public List<VRMBuffer> Buffers {
			get { return buffers; }
			set { buffers = value; }
		}
		public List<VRMBufferView> BufferViews {
			get { return bufferViews; }
			set { bufferViews = value; }
		}
		public List<VRMAccessor> Accessors {
			get { return accessors; }
			set { accessors = value; }
		}
		public List<VRMScene> Scenes {
			get { return scenes; }
			set { scenes = value; }
		}
		public List<VRMNode> Nodes {
			get { return nodes; }
			set { nodes = value; }
		}
		public List<VRMMesh> Meshes {
			get { return meshes; }
			set { meshes = value; }
		}

		public VRM() {
			scenes = new List<VRMScene>();
			buffers = new List<VRMBuffer>();
			bufferViews = new List<VRMBufferView>();
			accessors = new List<VRMAccessor>();
			scenes = new List<VRMScene>();
			nodes = new List<VRMNode>();
			meshes = new List<VRMMesh>();
		}
	}
}
