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
			string pastFileName = string.Empty;
			string fileName = string.Empty;
			bool foundUpdate = false;
			bool isDownloading = false;
			int fileLength = 0;

			WebClient webClient = new WebClient();
			webClient.DownloadProgressChanged += (s, e) =>
			{
				if (isDownloading)
				{
					if (pastFileName != fileName)
					{
						pastFileName = fileName;
						Console.Write("\"{0}\": Downloading... ", fileName);
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
						Thread.Sleep(fileLength / 1000000);
					}
				}
				Console.WriteLine("Completed.");
				Thread.Sleep(50);
				isDownloading = false;
			};

			Console.Title = "DownloadUpdate_DurangoV2 - Updating...";
			SetCloseButtonEnabled(consoleWindowHandle, false);

			string address = "http://durangomanager.kyllox.studio/client/isUpdateEtc.txt";
			string txt = new StreamReader(webClient.OpenRead(address)).ReadToEnd();
			bool isUpdateEtc = bool.Parse(txt);
			int downloadCount = 3;
			string path = Directory.GetParent(Directory.GetParent(Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName).FullName).FullName;
			if (isUpdateEtc)
				downloadCount = 6;

			for (int i = 0; i < downloadCount; i++)
			{
				while (isDownloading)
				{
				}
				string name = string.Empty;
				if (i == 0)
				{
					downloadPath = "https://github.com/KylloxStudio/Durango_V2/blob/main/DurangoV2_Data/Managed/Assembly-CSharp.dll?raw=true";
					localPath = path + "\\Managed\\Assembly-CSharp.dll";
					name = "Assembly-CSharp.dll";
				}
				else if (i == 1)
				{
					downloadPath = "https://github.com/KylloxStudio/Durango_V2/blob/main/DurangoV2_Data/resources.assets?raw=true";
					localPath = path + "\\resources.assets";
					name = "resources.assets";
				}
				else if (i == 2)
				{
					downloadPath = "https://github.com/KylloxStudio/Durango_V2/blob/main/DurangoV2_Data/version.txt?raw=true";
					localPath = path + "\\version.txt";
					name = "version.txt";
				}

				if (downloadCount > 3)
				{
					if (i == 3)
					{
						downloadPath = "https://github.com/KylloxStudio/Durango_V2/blob/main/DurangoV2_Data/StreamingAssets/Movie/PC/title.mp4?raw=true";
						localPath = path + "\\StreamingAssets\\Movie\\PC\\title.mp4";
						name = "title.mp4";
					}
					else if (i == 4)
					{
						downloadPath = "https://github.com/KylloxStudio/Durango_V2/blob/main/DurangoV2_Data/StreamingAssets/Movie/PC/warping.mp4?raw=true";
						localPath = path + "\\StreamingAssets\\Movie\\PC\\warping.mp4";
						name = "warping.mp4";
					}
					else if (i == 5)
					{
						downloadPath = "https://github.com/KylloxStudio/Durango_V2/blob/main/DurangoV2_Data/StreamingAssets/Audio/GeneratedSoundBanks/Windows/title.bnk?raw=true";
						localPath = path + "\\StreamingAssets\\Audio\\GeneratedSoundBanks\\Windows\\title.bnk";
						name = "title.bnk";
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
						fileName = name;
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
			SetCloseButtonEnabled(consoleWindowHandle, true);

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
						Process.Start("https://minhaskamal.github.io/DownGit/#/home?url=https://github.com/KylloxStudio/Durango_V2/tree/main/DurangoV2_Data/Programs/UpdateManager");
						isOpenedUrl = true;
						break;
					}
					else if (input == "N" || input == "n")
					{
						break;
					}
				}
			}
			else
            {
				Console.WriteLine("Press any key to restart the game.");
				Console.ReadKey();
			}

			string path2 = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\FoundUpdate.txt";
			File.Delete(path2);
			foreach (Process process in Process.GetProcesses())
			{
				if (process.ProcessName.IndexOf("UpdateManager") == -1 && process.ProcessName.IndexOf("DurangoV2") != -1)
				{
					process.Kill();
					break;
				}
				else
                {
					Console.WriteLine("Cannot Found exe file.");
					break;
                }
			}

			if (!isOpenedUrl)
			{
				foreach (Process process in Process.GetProcesses())
				{
					if (process.ProcessName.IndexOf("UpdateManager") == -1 && process.ProcessName.IndexOf("DurangoV2") != -1)
					{
						process.Start();
						break;
					}
					else
					{
						Console.WriteLine("Cannot Found exe file.");
						break;
					}
				}
			}
		}

		private static void SetCloseButtonEnabled(IntPtr windowHandle, bool enabled)
		{
			IntPtr systemMenuHandle = GetSystemMenu(windowHandle, false);
			EnableMenuItem(systemMenuHandle, SC_CLOSE, MF_ENABLED | (enabled ? MF_ENABLED : MF_GRAYED));
		}
	}
}
