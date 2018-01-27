using System.Collections.Generic;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;

namespace TicTacTubeCore.Google.GoogleAPI
{
	/// <summary>
	/// A factory that is capable of generating an Api for Google services.
	/// </summary>
	/// <typeparam name="T">The type the factory should produce.</typeparam>
	public abstract class ApiFactory<T>
	{
		protected readonly string ApplicationName;
		protected readonly string PathToSecret;

		protected ApiFactory(string applicationName, string pathToSecret = null)
		{
			ApplicationName = applicationName;
			PathToSecret = pathToSecret;
		}

		public virtual T Create()
		{
			return Create(PathToSecret);
		}

		public virtual T Create(string pathToSecret)
		{
			using (var stream = new FileStream(pathToSecret, FileMode.Open, FileAccess.Read))
			{
				return Auth(stream);
			}
		}

		protected virtual UserCredential CreateCredentials(Stream secretStream, IEnumerable<string> scopes)
		{
			return GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(secretStream).Secrets,
				scopes,
				"user",
				CancellationToken.None,
				new FileDataStore(ApplicationName)
			).Result;
		}

		protected abstract T Auth(Stream secretStream);
	}
}