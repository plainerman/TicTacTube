using Google.Apis.YouTube.v3;

namespace TicTacTubeCore.Google.GoogleAPI
{
	public class ApiHandler : IApiHandler
	{
		/// <inheritdoc />
		public YouTubeService YouTubeService { get; protected set; }

		public void SetYoutubeService(ApiFactory<YouTubeService> factory)
		{
			var old = YouTubeService;
			YouTubeService = factory.Create();
			old?.Dispose();
		}

		/// <inheritdoc />
		public void Dispose()
		{
			YouTubeService?.Dispose();
		}
	}
}