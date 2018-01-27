using System;
using Google.Apis.YouTube.v3;
using TagLib.Id3v2;

namespace TicTacTubeCore.Google.GoogleAPI
{
	public interface IApiHandler : IDisposable
	{
		YouTubeService YouTubeService { get; }
		void SetYoutubeService(ApiFactory<YouTubeService> factory);
	}
}