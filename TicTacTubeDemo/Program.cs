namespace TicTacTubeDemo
{
	public class Program
	{
		private static void Main(string[] args)
		{
			//var scheduler = new EventFiringScheduler();
			//var pipelineBuilder = new DataPipelineBuilder();

			//var source = new FileSource(externalSource: new UrlSource("http://example.org/file"), lazy: false);

			//pipelineBuilder
			//	.Append(new SourceConverter(Type.Mp3))
			//	.Append(new MetaDataFiller())
			//	.Append(new MusicDetector())
			//	.Append(new MusicRenamer())
			//	.Append(new ConditionalProcessor(file => file.Size > 10000),
			//		new SourceMover(destination: "/home/plainer/podcasts/"),
			//		new SourceMover(destination: "/home/plainer/music/"))
			//	.Split(new SourceDuplicator("(2)"), new MultiProcessor(new SymlinkCreator()..., ... /*nested pipeline */), new SourceMover("/home/src"));


			//scheduler.Add(pipelineBuilder);
			//scheduler.Fire();
		}
	}
}
