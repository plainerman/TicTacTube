using System.IO;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;

namespace TicTacTubeCore.Google.GoogleAPI.YouTube
{
	public class YouTubeApiFactory : ApiFactory<YouTubeService>
	{
		public YouTubeApiFactory(string applicationName) : base(applicationName) { }

		protected override YouTubeService Auth(Stream secretStream)
		{
			var creds = CreateCredentials(secretStream, new[] { YouTubeService.Scope.YoutubeReadonly });

			return new YouTubeService(new BaseClientService.Initializer
			{
				HttpClientInitializer = creds,
				ApplicationName = ApplicationName
			});
		}
	}
}