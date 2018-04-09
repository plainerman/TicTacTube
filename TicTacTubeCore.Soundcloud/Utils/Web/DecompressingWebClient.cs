using System;
using System.Net;

namespace TicTacTubeCore.Soundcloud.Utils.Web
{
	/// <summary>
	/// This is a little helper class, that automatically handles compressed webrequests.
	/// </summary>
	public class DecompressingWebClient : WebClient
	{
		/// <inheritdoc />
		protected override WebRequest GetWebRequest(Uri address)
		{
			var request = (HttpWebRequest)base.GetWebRequest(address);
			request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
			return request;
		}
	}
}