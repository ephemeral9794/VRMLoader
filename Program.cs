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
				Model model = Loader.Load(stream);
				//Loader.Print(vrm);
				string json = Loader.SerializeModelInfo(model.Info);
				using (var output = File.CreateText("model.json")) {
					output.WriteLine(json);
					output.Flush();
				}
			}
		} catch (Exception e) {
			Console.Error.WriteLine(e.Message);
			Console.Error.WriteLine(e.StackTrace);
		}
	}
}
