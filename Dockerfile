FROM microsoft/dotnet:2.1-sdk

RUN apt-get update && apt-get install -y libunwind8

WORKDIR /ttt/src/
COPY . .

RUN dotnet restore

CMD ["dotnet", "run", "--project", "TicTacTubeDemo"]