using System.Collections.Generic;
using System.Linq;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace TicTacTubeCore.Google.Utils.Extensions.YouTube
{
	public static class PlaylistItemsResourceExtensions
	{
		public static IList<PlaylistItem> ListAll(this PlaylistItemsResource resource, string playlistId, string part)
		{
			var request = resource.List(part);
			request.PlaylistId = playlistId;
			var playlistItems = new List<PlaylistItem>();

			PlaylistItemListResponse response;
			do
			{
				response = request.Execute();

				playlistItems.AddRange(response.Items);

				request.PageToken = response.NextPageToken;

			} while (response.NextPageToken != null);

			// distinct by id
			return playlistItems.GroupBy(i => i.Id).Select(i => i.First()).ToList();
		}
	}
}