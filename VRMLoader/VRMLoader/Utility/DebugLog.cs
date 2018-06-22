using System;
using System.IO;

namespace VRMLoader.Utility
{
	class DebugLog
	{
		private static StreamWriter writer;
		public static StreamWriter Out { get { return writer; } }
		private static bool visible;
		public static bool Visible { get { return visible; } set { visible = value; } }
		static DebugLog() {
			visible = false;
		}
		public static void Start() {
			try {
				writer = new StreamWriter(File.Open(".\\debug.log", FileMode.Create));
			} catch (Exception e) {
				throw e;
			}
		}
		public static void WriteLine() {
			try {
				writer.WriteLine();
				if (visible) {
					Console.WriteLine();
				}
			} catch (Exception e) {
				End();
				throw e;
			}
		}
		public static void WriteLine(string s) {
			try {
				writer.WriteLine(s);
				if (visible) {
					Console.WriteLine(s);
				}
			} catch (Exception e) {
				End();
				throw e;
			}
		}
		public static void Write(string s) {
			try {
				writer.Write(s);
				if (visible) {
					Console.Write(s);
				}
			} catch (Exception e) {
				End();
				throw e;
			}
		}
		public static void End() {
			writer.Flush();
			writer.Close();
			writer.Dispose();
			writer = null;
		}
	}
}
