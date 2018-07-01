using System;
using System.IO;
using VRMLoader;
using VRMLoader.Utility;

namespace VRMLoaderTest
{
	class Program
	{
		static void Main(string[] args)
		{
			try {
				string file = "..\\..\\sample\\AliciaSolid.vrm";
				using (Stream stream = File.Open(file, FileMode.Open)) {
					Console.WriteLine($"Load Model File ({file}).");
					Model model = Loader.Load(stream);
					//Loader.Print(vrm);
					Loader.PrintModelInfo(model.Info);
					Console.WriteLine($"Output Model Info JSON.");
					string json = Loader.SerializeModelInfo(model.Info);
					using (var output = File.CreateText("model.json")) {
						output.WriteLine(json);
						output.Flush();
					}
					Console.WriteLine($"Output Model Binary Data.");
					using(var output = File.Create("binary.bin")) {
						output.Write(model.Data.Binary, 0, model.Data.Binary.Length);
						output.Flush();
					}
				}
			} catch (Exception e) {
				Console.Error.WriteLine(e.Message);
				Console.Error.WriteLine(e.StackTrace);
			}
		}
	}
}
