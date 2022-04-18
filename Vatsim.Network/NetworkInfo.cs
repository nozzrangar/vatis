using System.Net;

namespace Vatsim.Network
{
    public class NetworkInfo
	{
		public List<NetworkServerInfo> Servers { get; private set; }
		public VoiceSystem VoiceSystem { get; private set; }

		public static NetworkInfo Fetch(string statusUrl)
		{
			string[] statusFileLines = GetStatusFile(statusUrl);

			List<string> serverListUrls = GetServerListUrls(statusFileLines);
			if (serverListUrls.Count == 0)
			{
				throw new ApplicationException("No server list URLs found.");
			}

			Random rand = new Random();
			int index = rand.Next(0, serverListUrls.Count);
			string serverListUrl = serverListUrls[index];

			NetworkInfo info = new NetworkInfo
			{
				Servers = DownloadAndParseServerList(serverListUrl),
				VoiceSystem = GetVoiceSystem(statusFileLines)
			};

			return info;
		}

		public static List<NetworkServerInfo> GetServerList(string statusUrl)
		{
			return Fetch(statusUrl).Servers;
		}

		public static VoiceSystem GetVoiceSystem(string statusUrl)
		{
			return GetVoiceSystem(GetStatusFile(statusUrl));
		}

		private static string[] GetStatusFile(string statusUrl)
		{
			using (WebClient webClient = new WebClient())
			{
				string statusFile = webClient.DownloadString(statusUrl);
				return statusFile.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			}
		}

		private static List<string> GetServerListUrls(string[] statusFileLines)
		{
			List<string> serverListURLs = new List<string>();
			foreach (string line in statusFileLines)
			{
				if (line.StartsWith("url1=") && (line.Length > 5))
				{
					serverListURLs.Add(line.Substring(5));
				}
			}
			return serverListURLs;
		}

		private static List<NetworkServerInfo> DownloadAndParseServerList(string serverListURL)
		{
			using (WebClient webClient = new WebClient())
			{
				string serverFile = webClient.DownloadString(serverListURL);
				string[] lines = serverFile.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
				bool inServers = false;
				List<NetworkServerInfo> servers = new List<NetworkServerInfo>();
				foreach (string line in lines)
				{
					if (line.Trim().StartsWith(";"))
					{
						continue;
					}
					if (line.StartsWith("!SERVERS:"))
					{
						inServers = true;
						continue;
					}
					if (inServers)
					{
						if (line.StartsWith("!"))
						{
							break;
						}
						string[] fields = line.Split(new char[] { ':' }, StringSplitOptions.None);
						servers.Add(new NetworkServerInfo()
						{
							Name = fields[0].Trim(),
							Address = fields[1].Trim(),
							Location = fields[2].Trim(),
							Description = fields[3].Trim()
						});
					}
				}
				return servers;
			}
		}

		private static VoiceSystem GetVoiceSystem(string[] statusFileLines)
		{
			foreach (string line in statusFileLines)
			{
				if (line.StartsWith("voice0=") && (line.Length > 7))
				{
					return line.Substring(7).ToUpper() == "VVL" ? VoiceSystem.Vvl : VoiceSystem.Afv;
				}
			}

			return VoiceSystem.Afv;
		}
	}
}