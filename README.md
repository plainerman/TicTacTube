# TicTacTube (TTT)
[![Build Status (Master)](https://img.shields.io/travis/plainerman/TicTacTube/master.svg?style=for-the-badge&logo=travis)](https://travis-ci.org/plainerman/TicTacTube/branches#)	[![Nuget (Alpha)](https://img.shields.io/nuget/vpre/TTT.TicTacTube.svg?style=for-the-badge&logo=nuget)](https://www.nuget.org/packages/TTT.TicTacTube)	[![MIT license](https://img.shields.io/github/license/mashape/apistatus.svg?style=for-the-badge)](http://choosealicense.com/licenses/mit)

A versatile pipelining library created with media organization in mind and designed as a service.

Most (service) applications (i.e. daemons) wait in the background for an (external) event causing the program to perform operations on files or data.
The aim of this library is to provide an easy and concise way to describe actions triggered by events. Further, TicTacTube includes predefined schedulers and processors to get you started with while still being easily extensible and usable for almost any program based on the I/O principle.

To prevent your packages from getting bloated, the TicTacTube core package only includes the most basic features; other packages can be included separately (officially supported packages are also in available in this repo).
Some packages are:
* [Telegram Integration](https://www.nuget.org/packages/TTT.TicTacTube.Telegram/) Enabling users to easily create telegram bots.
* [Genius.com Info Fetcher](https://www.nuget.org/packages/TTT.TicTacTube.Genius/) Making it easy to update meta-info for songs by using a Genius API-token.
* [YouTube-DL Integration](https://github.com/plainerman/TicTacTube/tree/master/TicTacTubeCore.YoutubeDL) (working on it...) Make use of a local youtube-dl installation to download content and convert files.
* ... and more!

## Table of Contents
* [Getting Started](#getting-started)
	+ [NuGet](#nuget)
	+ [(Optional) Logging](#optional-logging)
	+ [(Kind of) Hello World](#kind-of-hello-world)
* [Demo: Music Organization](#demo-music-organization)
	+ [The Code](#the-code)
* [Used Libraries](#used-libraries)

## Getting Started

### NuGet

Include the [core NuGet package](https://www.nuget.org/packages/TTT.TicTacTube) and all other packages your heart desires. Links can be found above (or use the NuGet search).

#### (Optional) May the source be with you

If you would like to customize the code (or view it in your IDE) you can always clone the repository.

```Shell
git clone https://github.com/plainerman/TicTacTube
```

To build the package(s) or run the tests locally, use [.NET CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x) (>= 2.2) with the following commands:

```Shell
dotnet build
```

```Shell
dotnet test TicTacTubeTest
```

Genius integration tests will fail unless you provide a system variable called `GENIUS_TOKEN` containing a valid genius token. If you don't need Genius integration, you can ignore those; otherwise request an API-token [here](https://genius.com/api-clients/new).
Some tests require an active internet connection.

### (Optional) Logging
Log4Net is used for logging and must be enabled manually. The easiest way is to add the following lines to your code. An example log4net.config file, can be downloaded [here](https://github.com/plainerman/TicTacTube/blob/master/TicTacTubeDemo/log4net.config) &mdash; just put it inside your bin (or run) folder. 
```C#
var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
```

### (Kind of) Hello World
The following example consists of a manually triggered scheduler, executing a single pipeline on two files (single-threaded). One of those files is an external source, that will be downloaded before operating on it. Once the pipeline executes, the names of the files are printed.
```C#
var scheduler = new EventFiringScheduler(); // a scheduler that can be triggered by calling a function
var pipelineBuilder = new DataPipelineBuilder(); // container for the pipeline

// append a new processor (use a predefined one, or a custom class ...)
pipelineBuilder.Append(new LambdaProcessor(source => {
   Console.WriteLine($"Processing: {source.FileName}, extension: {source.FileExtension}");
   return source; // the source could be changed (e.g. rename a file) or set to null
}));

scheduler.Add(pipelineBuilder); // assign the pipeline
scheduler.Start(); // activate the scheduler

scheduler.Fire(new FileSource("file.tmp")); // manually trigger with a given data source
// download the file and store it inside the folder "download" (automatically created)
scheduler.Fire(new FileSource(
   new UrlSource("https://raw.githubusercontent.com/plainerman/TicTacTube/master/README.md"), "download")
);

scheduler.Stop(); // notify the scheduler to stop (non-blocking)
scheduler.Join(); // ensure that all operations are finished (especially downloading)
```

The expected output (with logging disabled) is:

```
Processing: file, extension: .tmp
Processing: README, extension: .md
```

## Demo: Music Organization

This short program demonstrates what TicTacTube was originally designed for: organizing media.

In this application, the music folder of the system is observed. Whenever a new mp3-file has been added, the following steps will be executed:
1. The name of the file will be parsed and analyzed for the title of the song and contributing artists. Parts with no relevance will be omitted (e.g. "(official video)").
2. With the extracted metadata a search on [Genius](https://genius.com) will be performed, downloading the info of the best match.
3. If the similarity of the parsed filename and the metadata from Genius is high enough, the information will be merged.
4. The new ID3-tag (including the album cover) will be written, the file moved to a new folder, and renamed to a concise name.

Everything described in steps 1&ndash;4 can be seen in this short gif: 
![TicTacTube in action (organizing media)](https://i.imgur.com/1TcVyIt.gif)

----------------------------------------------------------------
### The Code
If you are only interested in how this framework operates, go ahead. Look at the code and ignore the next paragraphs that describe how to set everything up.

If you would like to try the demo code out yourself and don't need Genius integration, you can just delete all parts involving Genius and run the project with the core package added as a NuGet dependency.

To setup Genius integration, you should keep a few things in mind: 
1. This code requires the [core NuGet package](https://www.nuget.org/packages/TTT.TicTacTube/) *and* the [genius NuGet package](https://www.nuget.org/packages/TTT.TicTacTube.Genius/).
2. You have to acquire a Genius API-token from [here](https://genius.com/api-clients/new) and add it as an environment variable with the name `GENIUS_TOKEN`.

```C#
var scheduler = new FileSystemScheduler(new Executor(), // provide a multi-threaded executor
      Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), // watch user's music folder
      NotifyFilters.FileName | NotifyFilters.CreationTime, "*.mp3"); // trigger on new mp3-files

var pipelineBuilder = new DataPipelineBuilder(); // container for the pipeline

var extractor = new SongInfoExtractor(true); // an extractor capable of parsing filenames

// create a new song info fetcher that uses genius.com
// the fetcher will only be created if a valid API-token
// is stored inside GENIUS_TOKEN (environment variable)
GeniusSongInfoFetcher genius = null;
try {
   genius = new GeniusSongInfoFetcher();
}
catch (ArgumentException) {} // will be thrown if no token could be found

var merger = new SongInfoMerger(); // a class that can merge multiple SongInfo instances

// define the stop condition of the pipeline (not necessary)
pipelineBuilder.Append(
   // if a file starts with stop
   new ConditionalProcessor(source => source.FileName.StartsWith("stop"),
      new LambdaProcessor(source => {
         scheduler.Stop(); // stop the scheduler
         return null; // discard the source (pipeline not automatically stopped)
      })
   )
);

pipelineBuilder.Append(
   new ConditionalProcessor(source => source != null, // skip discarded sources
      new MultiProcessor( // add multiple steps to the condition
         new LambdaProcessor(source => {
            // load already stored ID3-tags (if any) from the specified file
            var originalInfo = SongInfo.ReadFromFile(source.FileInfo.FullName);

            // try to parse the filename and extract the title, and artist(s)
            var parsedInfo = extractor.ExtractFromString(source.FileName);
            // merge the metadata but ensure that they are (probably) identical
            // greedy ensures that null values are overridden
            originalInfo = merger.Merge(originalInfo, parsedInfo, greedy: true);

            if (genius != null) { // if a genius client is available
               // search for the info on genius
               parsedInfo = genius.ExtractAsyncTask(originalInfo).GetAwaiter().GetResult();
               // since the fetched result could be completely different, non-greedy merging is important
               originalInfo = merger.Merge(originalInfo, parsedInfo, greedy: false);
            }

            originalInfo.WriteToFile(source.FileInfo.FullName); // write new metadata back
         }),
         // next step is to rename the file based on the new info, and move it to the "organized" folder
         new MediaRenamer<SongInfo>("organized/{Artists} - {Title}", new SongInfoExtractor(false))
      )
   )
);

scheduler.Add(pipelineBuilder); // assign the pipeline

scheduler.Start(); // activate the scheduler
scheduler.Join(); // run until a stop*-file has been created
```

## Docker
This repository contains a Dockerfile capable of executing the TicTacTubeDemo project. Currently, it is configured with a Telegram download demo similar to the examples above. Creating a `telegram.token` is required for the docker container to run with the current demo and if a file `genius.token` exists, genius integration will be activated. Those files have to be added before build or passed with the `-v` option.

Build the docker image:

```
docker build -t tictactube .
```

Run the docker image:

```
docker run TicTacTube
```

Downloaded files will be stored in the `/downloads` folder, so specify a mounted directory with the docker `-v` option to route it to the host.

## Used Libraries
For reference, a list of all libraries used inside the core project:

| Library                             | Purpose                           |
| :-----------------------------------|:----------------------------------|
| [log4net](https://logging.apache.org/log4net/) | Logging (log4j for .NET) |
| [TagLib-Sharp](https://github.com/mono/taglib-sharp/) | Reading and writing meta tags |

For used libraries in projects other than core, see [GitHub Dependencies](https://github.com/plainerman/TicTacTube/network/dependencies).
