using System;
using log4net;
using TicTacTubeCore.Sources.Files;
using TicTacTubeCore.Sources.Files.External;
using SystemPath = System.IO.Path;

namespace TicTacTubeTest.Sources.Files
{
	public class TempFileSource : BaseFileSource
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(TempFileSource));

		public TempFileSource() : base(SystemPath.GetTempFileName())
		{
		}

		public TempFileSource(IExternalFileSource externalSource) : base(externalSource, SystemPath.GetTempPath())
		{
		}

		~TempFileSource()
		{
			try
			{
				FileInfo.Delete();
			}
			catch (Exception)
			{
				Log.WarnFormat("Could not delete temporary file source {0} in directory {1}. " +
					"Maybe clean it yourself? Tests work nonetheless.",
					FileInfo?.FullName, FileInfo?.Directory);
			}
		}
	}
}