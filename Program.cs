using System;
using System.IO;
using VRMLoader;

class Program
{
	static void Main(string[] args)
	{
		try {
			string file = "C:\\Users\\Administrator\\Desktop\\Alicia\\VRM\\AliciaSolid.vrm";
			using (Stream stream = File.Open(file, FileMode.Open)) {
				VRM vrm = VRM.Load(stream);
				if (vrm != null) {
					vrm.Print();
				}
			}
		} catch (Exception e) {
			Console.Error.WriteLine(e.Message);
			Console.Error.WriteLine(e.StackTrace);
		}
	}
}
