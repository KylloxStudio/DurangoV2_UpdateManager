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
		public static double LatestVersion;
		public static bool Error;

		static void Main(string[] args)
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			HtmlWeb web = new HtmlWeb();
			HtmlDocument document = web.Load("https://github.com/KylloxStudio/Durango_V2/tags");
			HtmlNode h2Node = document.DocumentNode.SelectSingleNode("//h2[@class='f4 d-inline']");
			if (h2Node != null)
			{
				HtmlNode aNode = h2Node.SelectSingleNode("a");
				if (aNode != null)
				{
					string[] version = aNode.InnerText.Split(new char[]
					{
						'v'
					});
					LatestVersion = double.Parse(version[1]);

					string path = Directory.GetParent(Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)).FullName).FullName + "\\version.txt";

					if (new FileInfo(path).Exists)
					{
						string txt = File.ReadAllText(path);
						if (double.TryParse(txt, out double ver))
							CurrentVersion = ver;
					}
					else
					{
						Error = true;
						Console.WriteLine("Error: Cannot Found 'version.txt' File.");
						foreach (Process process in Process.GetProcesses())
						{
							if (process.ProcessName.StartsWith("DurangoV2_UpdateManager"))
							{
								process.Kill();
							}
						}
					}
				}
			}

			if (!Error)
			{
				if (CurrentVersion < LatestVersion)
					FoundUpdate = true;

				Console.WriteLine("Found Update: " + FoundUpdate.ToString());
				Console.WriteLine("Current Version: " + string.Format("{0:0.0}", CurrentVersion));
				Console.WriteLine("Latest Version: " + string.Format("{0:0.0}", LatestVersion));
				Console.WriteLine();
			}

			if (Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) == Directory.GetCurrentDirectory())
			{
				if (FoundUpdate)
					DownloadUpdate.Program.Update();
			}
			else
            {
				string path2 = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\FoundUpdate.txt";
				if (new FileInfo(path2).Exists)
				{
					string txt = File.ReadAllText(path2);
					if (FoundUpdate && txt.IndexOf("True") != -1)
						DownloadUpdate.Program.Update();
				}
			}
		}
	}
}