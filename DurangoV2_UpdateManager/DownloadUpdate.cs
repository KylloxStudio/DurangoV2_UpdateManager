using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

namespace DownloadUpdate
{
	class Program
	{
		static string DirectoryPath = Directory.GetParent(Directory.GetParent(Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName).FullName).FullName;

		public static void Update()
		{
			Console.Title = "DurangoV2_UpdateManager - Updating...";
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			string downloadPath = string.Empty;
			string localPath = string.Empty;
			string downloadedFileName = string.Empty;
			string downloadingFileName = string.Empty;
			bool isDownloading = false;
			int fileLength = 0;

			WebClient webClient = new WebClient();
			webClient.DownloadProgressChanged += (s, e) =>
			{
				if (isDownloading)
				{
					if (downloadedFileName != downloadingFileName)
					{
						downloadedFileName = downloadingFileName;
						Console.Write("\"{0}\": Downloading... ", downloadingFileName);
					}
				}
			};
			webClient.DownloadFileCompleted += (s, e) =>
			{
				fileLength = (int)new FileInfo(localPath).Length;
				using (var progress = new ProgressBar())
				{
					for (int i = 0; i <= 100; i++)
					{
						progress.Report((double)i / 100);
						Thread.Sleep(fileLength / 8263019);
					}
				}
				Console.WriteLine("Completed.");
				isDownloading = false;
			};

			string[] downloadPaths = new StreamReader(webClient.OpenRead("https://durango-database.vercel.app/update/UpdateFilePaths.txt")).ReadToEnd().Split('\n');
			string[] localPaths = new StreamReader(webClient.OpenRead("https://durango-database.vercel.app/update/UpdateFileLocalPaths.txt")).ReadToEnd().Split('\n');
			for (int i = 0; i < downloadPaths.Length; i++)
			{
				try
				{
					while (isDownloading)
					{
						Thread.Sleep(100);
					}
					downloadPath = downloadPaths[i];
					localPath = DirectoryPath + localPaths[i];
					HttpWebRequest request = (HttpWebRequest)WebRequest.Create(downloadPath);
					HttpWebResponse response = (HttpWebResponse)request.GetResponse();
					if (response.StatusCode == HttpStatusCode.OK)
					{
						while (webClient.IsBusy)
                        {
							Thread.Sleep(100);
						}
						webClient.DownloadFileAsync(new Uri(downloadPath), localPath);
						string[] split = localPaths[i].Split('\\');
						downloadingFileName = split[split.Length - 1];
						isDownloading = true;
					}
					response.Close();
				}
				catch (WebException)
				{
					break;
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}

			while (isDownloading)
			{
				Thread.Sleep(100);
			}

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("All Files Update Completed.");
			Console.WriteLine("Press any key to restart the game.");

			Console.ReadKey();
			try
			{
				foreach (Process process in Process.GetProcesses())
				{
					if (process.ProcessName.IndexOf("UpdateManager") == -1 && process.ProcessName.IndexOf("Durango") != -1)
					{
						process.Kill();
						Process.Start(process.ProcessName);
						break;
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
	}
}
