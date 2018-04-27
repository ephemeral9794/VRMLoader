using System;
using System.Collections.Generic;
using System.Text;

namespace VRMLoader.Utility
{
	public static class Util
	{
		public static T[] FillArray<T>(T[] array, T n) {
			for (int i = 0; i < array.Length; i++) {
				array[i] = n;
			}
			return array;
		}
		public static string[] ParseString(byte[] data) {
			return ParseString(new EndianStream(data));
		}
		public static string[] ParseString(EndianStream stream) {
			List<string> list = new List<string>();
			try {
				while(true) {
					list.Add(stream.ReadStringToNull());
				}
			} catch (System.IO.EndOfStreamException) { }
			return list.ToArray();
		}
		public static uint ReverseUInt(uint n) {
			byte[] bytes = BitConverter.GetBytes(n);
            Array.Reverse(bytes);
			return BitConverter.ToUInt32(bytes, 0);
		}
		public static string ReverseUIntToString(uint n) {
			return UIntToString(ReverseUInt(n));
		}
		public static string UIntToString(uint n) {
			return UIntToString(n, Endian.LITTLE_ENDIAN);
		}
		public static string UIntToString(uint n, Endian type) {
			if (type == Endian.LITTLE_ENDIAN) {
				byte[] bytes = BitConverter.GetBytes(n);
                Array.Reverse(bytes);
                return Encoding.GetEncoding("ASCII").GetString(bytes);
			} else {
				return Encoding.GetEncoding("ASCII").GetString(BitConverter.GetBytes(n));
			}
		}
		public static uint ByteArrayToUInt(byte[] buf, Endian type) {
			if (type == Endian.BIG_ENDIAN) {
                byte[] a32 = new byte[4];
				Array.Copy(buf, a32, 4);
                Array.Reverse(a32);
                return BitConverter.ToUInt32(a32, 0);
            }
            else return BitConverter.ToUInt32(buf, 0);
		}
		public static uint ByteArrayToUInt(byte[] buf, uint index, Endian type) {
			byte[] a32 = new byte[4];
			Array.Copy(buf, index, a32, 0, 4);
			return ByteArrayToUInt(a32, type);
		}
	}
}
