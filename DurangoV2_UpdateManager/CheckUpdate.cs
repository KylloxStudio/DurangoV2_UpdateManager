using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using HtmlAgilityPack;

namespace CheckUpdate
{
	class Program
	{
		public static bool FoundUpdate;
		public static double CurrentVersion;
		public static double LastVersion;

		static void Main(string[] args)
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			HtmlWeb web = new HtmlWeb();
			HtmlDocument document = web.Load("https://github.com/KylloxStudio/Durango_V2/tags");
			HtmlNode h4Node = document.DocumentNode.SelectSingleNode("//h4[@class='flex-auto min-width-0 pr-2 pb-1 commit-title']");
			HtmlNode aNode = h4Node.SelectSingleNode("a");

			string[] version = aNode.InnerText.Split(new char[]
			{
				'v'
			});
			LastVersion = double.Parse(version[1]);

			string path = Directory.GetCurrentDirectory() + "\\DurangoV2_Data\\version.txt";
			Console.WriteLine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

			if (new FileInfo(path).Exists)
			{
				string txt = File.ReadAllText(path);
				CurrentVersion = double.Parse(txt);
			}
			else
			{
				Console.WriteLine("Error: Cannot Found 'version.txt' File.");
				Console.ReadKey();
				return;
			}

			if (CurrentVersion < LastVersion)
				FoundUpdate = true;

			Console.WriteLine("Found Update: " + FoundUpdate.ToString());
			Console.WriteLine("Current Version: " + string.Format("{0:0.0}", CurrentVersion));
			Console.WriteLine("Last Version: " + string.Format("{0:0.0}", LastVersion));

			if (FoundUpdate)
				DownloadUpdate.Program.Update();
		}
	}
}