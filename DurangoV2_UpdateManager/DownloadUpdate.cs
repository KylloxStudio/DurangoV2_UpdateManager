using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using HtmlAgilityPack;

namespace DownloadUpdate
{
	class Program
	{
		[DllImport("kernel32.dll")]
		private static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll")]
		private static extern IntPtr GetSystemMenu(IntPtr windowHandle, bool revert);

		[DllImport("user32.dll")]
		private static extern bool EnableMenuItem(IntPtr menuHandle, uint menuItemID, uint enabled);

		private const uint SC_CLOSE = 0xf060;
		private const uint MF_ENABLED = 0x00000000;
		private const uint MF_GRAYED = 0x00000001;

		public static void Update()
		{
			Console.Title = "DownloadUpdate_DurangoV2";
			IntPtr consoleWindowHandle = GetConsoleWindow();

			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			string downloadPath = string.Empty;
			string localPath = string.Empty;
			string fileName = string.Empty;
			bool foundUpdate = false;
			bool isDownloading = false;
			int fileLength = 0;

			WebClient webClient = new WebClient();
			webClient.DownloadProgressChanged += (s, e) =>
			{
				if (isDownloading)
				{
					Console.Write("\"{0}\": Downloading... ", fileName);
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
						Thread.Sleep(fileLength / 1000000);
					}
				}
				Console.WriteLine("Completed.");
				Thread.Sleep(500);
				isDownloading = false;
			};

			Console.Title = "DownloadUpdate_DurangoV2 - Updating...";
			SetCloseButtonEnabled(consoleWindowHandle, false);

			string address = "http://durangomanager.000webhostapp.com/client/isUpdateEtc.txt";
			string txt = new StreamReader(webClient.OpenRead(address)).ReadToEnd();
			bool isUpdateEtc = bool.Parse(txt);
			int downloadCount = 3;
			if (isUpdateEtc)
				downloadCount = 6;

			for (int i = 0; i < downloadCount; i++)
			{
				if (i == 0)
				{
					downloadPath = "https://github.com/KylloxStudio/Durango_V2/blob/main/DurangoV2_Data/Managed/Assembly-CSharp.dll?raw=true";
					localPath = Directory.GetCurrentDirectory() + "\\DurangoV2_Data\\Managed\\Assembly-CSharp.dll";
					fileName = "Assembly-CSharp.dll";
				}
				else if (i == 1)
				{
					downloadPath = "https://github.com/KylloxStudio/Durango_V2/blob/main/DurangoV2_Data/resources.assets?raw=true";
					localPath = Directory.GetCurrentDirectory() + "\\DurangoV2_Data\\resources.assets";
					fileName = "resources.assets";
				}
				else if (i == 2)
				{
					downloadPath = "https://github.com/KylloxStudio/Durango_V2/blob/main/DurangoV2_Data/version.txt?raw=true";
					localPath = Directory.GetCurrentDirectory() + "\\DurangoV2_Data\\version.txt";
					fileName = "version.txt";
				}

				if (downloadCount > 3)
				{
					if (i == 3)
					{
						downloadPath = "https://github.com/KylloxStudio/Durango_V2/blob/main/DurangoV2_Data/StreamingAssets/Movie/PC/title.mp4?raw=true";
						localPath = Directory.GetCurrentDirectory() + "\\DurangoV2_Data\\StreamingAssets\\Movie\\PC\\title.mp4";
						fileName = "title.mp4";
					}
					else if (i == 4)
					{
						downloadPath = "https://github.com/KylloxStudio/Durango_V2/blob/main/DurangoV2_Data/StreamingAssets/Movie/PC/warping.mp4?raw=true";
						localPath = Directory.GetCurrentDirectory() + "\\DurangoV2_Data\\StreamingAssets\\Movie\\PC\\warping.mp4";
						fileName = "warping.mp4";
					}
					else if (i == 5)
					{
						downloadPath = "https://github.com/KylloxStudio/Durango_V2/blob/main/DurangoV2_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/title.bnk?raw=true";
						localPath = Directory.GetCurrentDirectory() + "\\DurangoV2_Data\\StreamingAssets\\Audio\\GeneratedSoundBanks\\Windows\\title.bnk";
						fileName = "title.bnk";
					}
				}

				try
				{
					HttpWebRequest request = (HttpWebRequest)WebRequest.Create(downloadPath);
					HttpWebResponse response = (HttpWebResponse)request.GetResponse();
					if (response.StatusCode == HttpStatusCode.OK)
					{
						while (webClient.IsBusy)
						{
						}
						while (isDownloading)
						{
						}
						webClient.DownloadFileAsync(new Uri(downloadPath), localPath);
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

			Console.Title = "DownloadUpdate_DurangoV2";

			while (webClient.IsBusy)
			{
			}
			while (isDownloading)
			{
			}
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("All Files Update Completed.");

			bool isOpenedUrl = false;
			if (foundUpdate)
			{
				Console.WriteLine();
				Console.WriteLine("자동 업데이트 다운로더의 업데이트가 발견되었습니다. 다운로드 페이지로 이동하시겠습니까? (Y / N)\n(다운로드 후 게임 폴더 내의 [Durango_V2_Data//Programs] 폴더에 압축을 풀어주세요.");
				Console.WriteLine("An update of the automatic update downloader has been found. Do you want to go to the download page? (Y / N)\n(After manual download, and unzip into the [DurangoV2_Data//Programs] folder.)");
				while (true)
				{
					string input = Console.ReadLine();
					if (input == "Y" || input == "y")
					{
						Process.Start("https://minhaskamal.github.io/DownGit/#/home?url=https://github.com/KylloxStudio/Durango_V2/tree/main/DurangoV2_Data/DownloadUpdate");
						isOpenedUrl = true;
						break;
					}
					else if (input == "N" || input == "n")
					{
						break;
					}
				}
			}

			foreach (Process process in Process.GetProcesses())
			{
				if (process.ProcessName.StartsWith("DurangoV2"))
				{
					process.Kill();
					break;
				}
			}
			if (!isOpenedUrl)
				Process.Start(Directory.GetCurrentDirectory() + "\\DurangoV2.exe");
		}

		private static void SetCloseButtonEnabled(IntPtr windowHandle, bool enabled)
		{
			IntPtr systemMenuHandle = GetSystemMenu(windowHandle, false);
			EnableMenuItem(systemMenuHandle, SC_CLOSE, MF_ENABLED | (enabled ? MF_ENABLED : MF_GRAYED));
		}
	}
}
