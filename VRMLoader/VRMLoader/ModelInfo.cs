using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace VRMLoader
{
	[DataContract]
	public class ModelInfo
	{
		[DataMember(Name = "extensions")]
		public Extensions Extensions { get; set; }

		[DataMember(Name = "asset")]
		public Asset Asset { get; set; }

		[DataMember(Name = "buffers")]
		public Buffer[] Buffers { get; set; }

		[DataMember(Name = "bufferViews")]
		public BufferView[] BufferViews { get; set; }

		[DataMember(Name = "accessors")]
		public Accessor[] Accessors { get; set; }

		[DataMember(Name = "images")]
		public Image[] Images { get; set; }

		[DataMember(Name = "samplers")]
		public Sampler[] Samplers { get; set; }

		[DataMember(Name = "textures")]
		public Texture[] Textures { get; set; }

		[DataMember(Name = "materials")]
		public Material[] Materials { get; set; }

		[DataMember(Name = "meshes")]
		public Mesh[] Meshes { get; set; }

		[DataMember(Name = "skins")]
		public Skin[] Skins { get; set; }

		[DataMember(Name = "nodes")]
		public Node[] Nodes { get; set; }

		[DataMember(Name = "scenes")]
		public Scene[] Scenes { get; set; }
	}
	
	[DataContract]
	public class VRMVector3 
	{
		[DataMember(Name = "x")]
		public float X { get; set; }

		[DataMember(Name = "y")]
		public float Y { get; set; }

		[DataMember(Name = "z")]
		public float Z { get; set; }
	}

	[DataContract]
	public class Extensions
	{
		[DataMember(Name = "VRM")]
		public VRM VRM { get; set; }
	}

	[DataContract]
	public class VRM
	{
		[DataMember(Name = "version")]
		public string Version { get; set; }
		
		[DataMember(Name = "meta")]
		public Meta Meta { get; set; }
		
		[DataMember(Name = "humanoid")]
		public Humanoid Humanoid { get; set; }
		
		[DataMember(Name = "firstPerson")]
		public FirstPerson FirstPerson { get; set; }
		
		[DataMember(Name = "blendShapeMaster")]
		public BlendShapeMaster BlendShapeMaster { get; set; }
		
		[DataMember(Name = "secondaryAnimation")]
		public SecondaryAnimation SecondaryAnimation { get; set; }
		
		[DataMember(Name = "materialProperties")]
		public MaterialProperty[] MaterialProperties { get; set; }
	}

	[DataContract]
	public class Meta
	{
		[DataMember(Name = "version")]
		public string Version { get; set; }
		
		[DataMember(Name = "author")]
		public string Author { get; set; }
		
		[DataMember(Name = "contactInformation")]
		public string ContactInformation { get; set; }
		
		[DataMember(Name = "reference")]
		public string Reference { get; set; }
		
		[DataMember(Name = "title")]
		public string Title { get; set; }
		
		[DataMember(Name = "texture")]
		public int Texture { get; set; }
		
		[DataMember(Name = "allowedUserName")]
		public string AllowedUserName { get; set; }
		
		[DataMember(Name = "violentUssageName")]
		public string ViolentUssageName { get; set; }
		
		[DataMember(Name = "sexualUssageName")]
		public string SexualUssageName { get; set; }
		
		[DataMember(Name = "commercialUssageName")]
		public string CommercialUssageName { get; set; }
		
		[DataMember(Name = "otherPermissionUrl")]
		public string OtherPermissionUrl { get; set; }
		
		[DataMember(Name = "licenseName")]
		public string LicenseName { get; set; }
		
		[DataMember(Name = "otherLicenseUrl")]
		public string OtherLicenseUrl { get; set; }
	}

	[DataContract]
	public class Humanoid
	{
		[DataMember(Name = "humanBones")]
		public HumanBone[] HumanBones { get; set; }

		[DataMember(Name = "armStretch")]
		public float ArmStretch { get; set; }

		[DataMember(Name = "legStretch")]
		public float LegStretch { get; set; }

		[DataMember(Name = "upperArmTwist")]
		public float UpperArmTwist { get; set; }

		[DataMember(Name = "lowerArmTwist")]
		public float LowerArmTwist { get; set; }

		[DataMember(Name = "upperLegTwist")]
		public float UpperLegTwist { get; set; }

		[DataMember(Name = "lowerLegTwist")]
		public float LowerLegTwist { get; set; }

		[DataMember(Name = "feetSpacing")]
		public int FeetSpacing { get; set; }

		[DataMember(Name = "hasTranslationDoF")]
		public bool HasTranslationDoF { get; set; }
	}

	[DataContract]
	public class HumanBone
	{
		[DataMember(Name = "bone")]
		public string Bone { get; set; }

		[DataMember(Name = "node")]
		public int Node { get; set; }
				
		[DataMember(Name = "useDefaultValues")]
		public bool UseDefaultValues { get; set; }
	}

	[DataContract]
	public class FirstPerson
	{
		[DataMember(Name = "firstPersonBone")]
		public int FirstPersonBone { get; set; }

		[DataMember(Name = "firstPersonBoneOffset")]
		public VRMVector3 FirstPersonBoneOffset { get; set; }

		[DataMember(Name = "meshAnnotations")]
		public MeshAnnotation[] MeshAnnotations { get; set; }

		[DataMember(Name = "lookAtTypeName")]
		public string LookAtTypeName { get; set; }

		[DataMember(Name = "lookAtHorizontalInner")]
		public LookAt LookAtHorizontalInner { get; set; }

		[DataMember(Name = "lookAtHorizontalOuter")]
		public LookAt LookAtHorizontalOuter { get; set; }

		[DataMember(Name = "lookAtVerticalDown")]
		public LookAt LookAtVerticalDown { get; set; }

		[DataMember(Name = "lookAtVerticalUp")]
		public LookAt LookAtVerticalUp { get; set; }
	}

	[DataContract]
	public class LookAt 
	{
		[DataMember(Name = "curve")]
		public int[] Curve { get; set; }

		[DataMember(Name = "xRange")]
		public int XRange { get; set; }

		[DataMember(Name = "yRange")]
		public int YRange { get; set; }
	}

	[DataContract]
	public class MeshAnnotation
	{
		[DataMember(Name = "mesh")]
		public int Mesh { get; set; }

		[DataMember(Name = "firstPersonFlag")]
		public string FirstPersonFlag { get; set; }
	}

	[DataContract]
	public class BlendShapeMaster
	{
		[DataMember(Name = "blendShapeGroups")]
		public BlendShapeGroup[] BlendShapeGroups { get; set; }
	}

	[DataContract]
	public class BlendShapeGroup
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }
		
		[DataMember(Name = "presetName")]
		public string PresetName { get; set; }
		
		[DataMember(Name = "binds")]
		public Bind[] Binds { get; set; }
		
		[DataMember(Name = "materialValues")]
		public object[] MaterialValues { get; set; }
	}

	[DataContract]
	public class Bind
	{
		[DataMember(Name = "mesh")]
		public int Mesh { get; set; }
		
		[DataMember(Name = "index")]
		public int Index { get; set; }
		
		[DataMember(Name = "weight")]
		public float Weight { get; set; }
	}

	[DataContract]
	public class SecondaryAnimation
	{
		[DataMember(Name = "boneGroups")]
		public BoneGroup[] BoneGroups { get; set; }
		
		[DataMember(Name = "colliderGroups")]
		public ColliderGroup[] ColliderGroups { get; set; }
	}

	[DataContract]
	public class BoneGroup
	{
		
		[DataMember(Name = "comment")]
		public string Comment { get; set; }
		
		[DataMember(Name = "stiffiness")]
		public float Stiffiness { get; set; }
		
		[DataMember(Name = "gravityPower")]
		public int GravityPower { get; set; }
		
		[DataMember(Name = "gravityDir")]
		public VRMVector3 GravityDir { get; set; }
		
		[DataMember(Name = "dragForce")]
		public float DragForce { get; set; }
		
		[DataMember(Name = "center")]
		public int Center { get; set; }
		
		[DataMember(Name = "hitRadius")]
		public float HitRadius { get; set; }
		
		[DataMember(Name = "bones")]
		public int[] Bones { get; set; }
		
		[DataMember(Name = "colliderGroups")]
		public int[] ColliderGroups { get; set; }
	}

	[DataContract]
	public class ColliderGroup
	{
		[DataMember(Name = "node")]
		public int Node { get; set; }

		[DataMember(Name = "colliders")]
		public Collider[] Colliders { get; set; }
	}

	[DataContract]
	public class Collider
	{
		[DataMember(Name = "offset")]
		public VRMVector3 Offset { get; set; }

		[DataMember(Name = "radius")]
		public float Radius { get; set; }
	}

	[DataContract]
	public class MaterialProperty
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "renderQueue")]
		public int RenderQueue { get; set; }

		[DataMember(Name = "shader")]
		public string Shader { get; set; }

		[DataMember(Name = "floatProperties")]
		public FloatProperties FloatProperties { get; set; }

		[DataMember(Name = "vectorProperties")]
		public VectorProperties VectorProperties { get; set; }

		[DataMember(Name = "textureProperties")]
		public TextureProperties TextureProperties { get; set; }

		[DataMember(Name = "keywordMap")]
		public KeywordMap KeywordMap { get; set; }

		[DataMember(Name = "tagMap")]
		public TagMap TagMap { get; set; }
	}

	[DataContract]
	public class FloatProperties
	{
	}

	[DataContract]
	public class VectorProperties
	{
	}

	[DataContract]
	public class TextureProperties
	{
		[DataMember(Name = "_MainTex")]
		public int _MainTex { get; set; }
	}

	[DataContract]
	public class KeywordMap
	{
		[DataMember(Name = "_ALPHAPREMULTIPLY_ON")]
		public bool _ALPHAPREMULTIPLY_ON { get; set; }
	}

	[DataContract]
	public class TagMap
	{
		[DataMember(Name = "RenderType")]
		public string RenderType { get; set; }
	}

	[DataContract]
	public class Asset
	{
		[DataMember(Name = "copyright")]
		public string Copyright { get; set; }

		[DataMember(Name = "generator")]
		public string Generator { get; set; }

		[DataMember(Name = "version")]
		public string Version { get; set; }

		[DataMember(Name = "minVersion")]
		public string MinVersion  { get; set; }
	}

	[DataContract]
	public class Buffer
	{
		[DataMember(Name = "byteLength")]
		public int ByteLength { get; set; }
	}

	[DataContract]
	public class BufferView
	{
		[DataMember(Name = "buffer")]
		public int Buffer { get; set; }
		
		[DataMember(Name = "byteOffset")]
		public int ByteOffset { get; set; }
		
		[DataMember(Name = "byteLength")]
		public int ByteLength { get; set; }
		
		[DataMember(Name = "target")]
		public int Target { get; set; }

		[DataMember(Name = "byteStride")]
		public int ByteStride { get; set; }
	}

	[DataContract]
	public class Accessor
	{

		[DataMember(Name = "bufferView")]
		public int BufferView { get; set; }

		[DataMember(Name = "byteOffset")]
		public int ByteOffset { get; set; }

		[DataMember(Name = "type")]
		public string Type { get; set; }

		[DataMember(Name = "componentType")]
		public int ComponentType { get; set; }

		[DataMember(Name = "count")]
		public int Count { get; set; }

		[DataMember(Name = "max")]
		public float[] Max { get; set; }

		[DataMember(Name = "min")]
		public float[] Min { get; set; }
	}

	[DataContract]
	public class Image
	{
		[DataMember(Name = "extra")]
		public Extra Extra { get; set; }

		[DataMember(Name = "bufferView")]
		public int BufferView { get; set; }

		[DataMember(Name = "mimeType")]
		public string MimeType { get; set; }
	}

	[DataContract]
	public class Extra
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }
	}

	[DataContract]
	public class Sampler
	{
		[DataMember(Name = "magFilter")]
		public int MagFilter { get; set; }
		
		[DataMember(Name = "minFilter")]
		public int MinFilter { get; set; }
		
		[DataMember(Name = "wrapS")]
		public int WrapS { get; set; }
		
		[DataMember(Name = "wrapT")]
		public int WrapT { get; set; }
	}

	[DataContract]
	public class Texture
	{
		[DataMember(Name = "sampler")]
		public int Sampler { get; set; }
		
		[DataMember(Name = "source")]
		public int Source { get; set; }
	}

	[DataContract]
	public class Material
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }
		
		[DataMember(Name = "pbrMetallicRoughness")]
		public PbrMetallicRoughness PbrMetallicRoughness { get; set; }
	}

	[DataContract]
	public class PbrMetallicRoughness
	{
		[DataMember(Name = "baseColorTexture")]
		public BaseColorTexture BaseColorTexture { get; set; }
		
		[DataMember(Name = "metallicFactor")]
		public int MetallicFactor { get; set; }
		
		[DataMember(Name = "roughnessFactor")]
		public int RoughnessFactor { get; set; }
	}

	[DataContract]
	public class BaseColorTexture
	{
		[DataMember(Name = "index")]
		public int Index { get; set; }
		
		[DataMember(Name = "texCoord")]
		public int TexCoord { get; set; }
	}

	[DataContract]
	public class Mesh
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }
		
		[DataMember(Name = "primitives")]
		public Primitive[] Primitives { get; set; }
	}

	[DataContract]
	public class Primitive
	{
		[DataMember(Name = "mode")]
		public int Mode { get; set; }
		
		[DataMember(Name = "indices")]
		public int Indices { get; set; }
		
		[DataMember(Name = "attributes")]
		//public Attributes Attributes { get; set; }
		public Dictionary<string, int> Attributes { get; set; }
		
		[DataMember(Name = "material")]
		public int Material { get; set; }
		
		[DataMember(Name = "targets")]
		public Target[] Targets { get; set; }
	}

	/*[DataContract]
	public class Attributes
	{
		[DataMember(Name = "POSITION")]
		public int POSITION { get; set; }
		
		[DataMember(Name = "NORMAL")]
		public int NORMAL { get; set; }
		
		[DataMember(Name = "TANGENT")]
		public int TANGENT { get; set; }
		
		[DataMember(Name = "TEXCOORD_0")]
		public int TEXCOORD_0 { get; set; }
		
		[DataMember(Name = "JOINTS_0")]
		public int JOINTS_0 { get; set; }
		
		[DataMember(Name = "WEIGHTS_0")]
		public int WEIGHTS_0 { get; set; }
	}*/

	[DataContract]
	public class Target
	{
		[DataMember(Name = "extra")]
		public Extra Extra { get; set; }
		
		[DataMember(Name = "POSITION")]
		public int POSITION { get; set; }
		
		[DataMember(Name = "NORMAL")]
		public int NORMAL { get; set; }
		
		[DataMember(Name = "TANGENT")]
		public int TANGENT { get; set; }
		
		[DataMember(Name = "TEXCOORD_0")]
		public int TEXCOORD_0 { get; set; }
		
		[DataMember(Name = "JOINTS_0")]
		public int JOINTS_0 { get; set; }
		
		[DataMember(Name = "WEIGHTS_0")]
		public int WEIGHTS_0 { get; set; }
	}

	[DataContract]
	public class Skin
	{
		[DataMember(Name = "inverseBindMatrices")]
		public int InverseBindMatrices { get; set; }
		
		[DataMember(Name = "joints")]
		public int[] Joints { get; set; }
	}

	[DataContract]
	public class Node
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }
		
		[DataMember(Name = "translation")]
		public float[] Translation { get; set; }
		
		[DataMember(Name = "rotation")]
		public float[] Rotation { get; set; }
		
		[DataMember(Name = "scale")]
		public float[] Scale { get; set; }
		
		[DataMember(Name = "mesh")]
		public int Mesh { get; set; }
		
		[DataMember(Name = "skin")]
		public int Skin { get; set; }
		
		[DataMember(Name = "extra")]
		public NodeExtra Extra { get; set; }
		
		[DataMember(Name = "children")]
		public int[] Children { get; set; }
	}

	[DataContract]
	public class NodeExtra
	{
		[DataMember(Name = "skinRootBone")]
		public int SkinRootBone { get; set; }
	}

	[DataContract]
	public class Scene
	{
		[DataMember(Name = "nodes")]
		public int[] Nodes { get; set; }
	}

}
