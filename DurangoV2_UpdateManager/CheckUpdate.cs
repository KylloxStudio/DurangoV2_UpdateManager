using System;
using System.IO;
using System.Net;
using System.Diagnostics;

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
			if (args != null && args.Length > 0 && args[0] == "update")
			{
				DownloadUpdate.Program.Update();
				return;
			}

			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			WebClient webClient = new WebClient();
			string address = "https://raw.githubusercontent.com/KylloxStudio/Durango_V2/main/DurangoV2_Data/version.txt";
			LatestVersion = double.Parse(webClient.DownloadString(address));

			string path = Directory.GetParent(Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)).FullName).FullName + "\\version.txt";

			if (new FileInfo(path).Exists)
			{
				string txt = File.ReadAllText(path);
				if (double.TryParse(txt, out double ver))
				{
					CurrentVersion = ver;
				}
			}
			else
			{
				Error = true;
				Console.WriteLine("Error: Cannot Found 'version.txt' File.");
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
		}
	}
}