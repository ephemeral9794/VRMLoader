using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRMLoader
{
	public class ModelData
	{
		public byte[] Binary { get; set; }

		public ModelData(byte[] bin) {
			Binary = new byte[bin.Length];
			Array.Copy(bin, Binary, bin.Length);
		}

		private static int GetByteLength(GLenum componentType) {
			switch (componentType) {
				case GLenum.GL_BYTE:
				case GLenum.GL_UNSIGNED_BYTE:
					return sizeof(byte);
				case GLenum.GL_SHORT:
				case GLenum.GL_UNSIGNED_SHORT:
					return sizeof(short);
				case GLenum.GL_INT:
				case GLenum.GL_UNSIGNED_INT:
					return sizeof(int);
				case GLenum.GL_FLOAT:
					return sizeof(float);
				case GLenum.GL_2_BYTES:
					return sizeof(byte) * 2;
				case GLenum.GL_3_BYTES:
					return sizeof(byte) * 3;
				case GLenum.GL_4_BYTES:
					return sizeof(byte) * 4;
				case GLenum.GL_DOUBLE:
					return sizeof(double);
				default:
					return 0;
			}
		}
		private static int GetByteStride(Accessor accessor, BufferView view) {
			int len = 0;
			if (view.ByteStride == 0) {
				len = GetByteLength((GLenum)accessor.ComponentType);
				String type = accessor.Type;
				if (type == "SCALAR") {
					len *= 1;
				} else if (type == "VEC2") {
					len *= 2;
				} else if (type == "VEC3") {
					len *= 3;
				} else if (type == "VEC4") {
					len *= 4;
				} else if (type == "MAT2") {
					len *= 4;
				} else if (type == "MAT3") {
					len *= 9;
				} else if (type == "MAT4") {
					len *= 16;
				} else {
					len = 0;
				}
				return len;
			} else {
				len = GetByteLength((GLenum)accessor.ComponentType);
				if ((view.ByteStride % len) != 0) {
					return 0;
				}
				return view.ByteStride;
			}
		}

		/// <summary>
		/// バイナリデータをAccessorをもとに切り出す
		/// </summary>
		/// <param name="accessor">VRM Model Accessor</param>
		/// <param name="view">VRM Model Buffer View</param>
		/// <returns></returns>
		public MemoryStream ParseBinary(Accessor accessor, BufferView view) {
			// 切り出し
			byte[] buffer = new byte[view.ByteLength];
			byte[] temp = new byte[view.ByteLength - accessor.ByteOffset];
			Array.Copy(Binary, view.ByteOffset, buffer, 0, view.ByteLength);
			Array.Copy(buffer, accessor.ByteOffset, temp, 0, temp.Length);

			// Byte Stride 算出
			int len = GetByteStride(accessor, view);
			String type = accessor.Type;
			if (type == "SCALAR") {
				len *= 1;
			} else if (type == "VEC2") {
				len *= 2;
			} else if (type == "VEC3") {
				len *= 3;
			} else if (type == "VEC4") {
				len *= 4;
			} else if (type == "MAT2") {
				len *= 4;
			} else if (type == "MAT3") {
				len *= 9;
			} else if (type == "MAT4") {
				len *= 16;
			} else {
				len = 0;
			}

			// 展開
			using (var stream = new MemoryStream(len * accessor.Count)) {
				for (int j = 0; j < accessor.Count * len; j++) {
					stream.WriteByte(temp[j]);
				}
				stream.Position = 0;
				return new MemoryStream(stream.GetBuffer());
			}
		}
	}
}
