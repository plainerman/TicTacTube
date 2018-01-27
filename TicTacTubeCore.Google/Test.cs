using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;


namespace TicTacTubeCore.Google
{
	public class Test
	{
		private static YouTubeService ytService = Auth();

		private static YouTubeService Auth()
		{
			UserCredential creds;
			using (var stream = new FileStream("youtube_client.json.secret", FileMode.Open, FileAccess.Read))
			{
				creds = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets,
					new[] { YouTubeService.Scope.YoutubeReadonly },
					"user",
					CancellationToken.None,
					new FileDataStore("YouTubeAPI")
				).Result;
			}

			return new YouTubeService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = creds,
				ApplicationName = "YouTubeAPI"
			});
		}

		public static string GetVideoTitle(string id)
		{
			var videoRequest = ytService.Videos.List("snippet");
			videoRequest.Id = id;

			var response = videoRequest.Execute();
			return response.Items.Count <= 0 ? null : response.Items[0].Snippet.Title;
		}

		public static string[] GetPlaylistIds(string playlistId)
		{
			var request = ytService.PlaylistItems.List("id");
			request.PlaylistId = playlistId;
			var videos = new List<string>();

			PlaylistItemListResponse response;
			do
			{
				response = request.Execute();

				videos.AddRange(response.Items.Select(i => i.Id));

				request.PageToken = response.NextPageToken;

			} while (response.NextPageToken != null);

			return videos.Distinct().ToArray();
		}

		public static string[] ListPlaylists()
		{
			var request = ytService.Playlists.List("id");
			request.Mine = true;
			var response = request.Execute();
			return response.Items.Select(p => p.Id).ToArray();
		}

		public static void AddVideo(string playlistId, string videoId)
		{

		}
	}
}