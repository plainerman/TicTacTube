using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using TicTacTubeCore.Google.GoogleAPI;
using TicTacTubeCore.Google.Utils.Extensions.YouTube;
using TicTacTubeCore.Schedulers;
using TicTacTubeCore.Sources.Files;
using TicTacTubeCore.Sources.Files.External;

namespace TicTacTubeCore.Google.Schedulers.YouTube.Playlists
{
	public class PlaylistChangedScheduler : PollingBaseScheduler
	{
		public IApiHandler ApiHandler { get; }
		public string LocalPath { get; }
		public string PlaylistId { get; }

		protected List<string> ProcessedItems { get; set; }

		public PlaylistChangedScheduler(IApiHandler apiHandler, ThreadPriority priority, string localPath, string playlistId, int delayInMillis = 15000) : base(priority, delayInMillis)
		{
			ApiHandler = apiHandler;
			LocalPath = localPath;
			PlaylistId = playlistId;

			ProcessedItems = new List<string>();
		}

		protected virtual IFileSource CreateFileSource(string path)
		{
			return new FileSource(new UrlSource(path), LocalPath);
		}

		protected override bool PollSource(out IFileSource[] sources)
		{
			var result = ApiHandler.YouTubeService.PlaylistItems.ListAll(PlaylistId, "id");
			for (int i = 0; i < result.Count; i++)
			{
				if (ProcessedItems.Contains(result[i].Id))
				{
					result.RemoveAt(i--);
				}
			}

			sources = null;
			if (result.Count > 0)
			{
				ProcessedItems.AddRange(result.Select(r => r.Id));
				throw new NotImplementedException();
				//sources = result.Select(r => new )
			}

			return result.Count > 0;
		}
	}
}