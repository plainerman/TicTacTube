FROM microsoft/dotnet:2.1-sdk   

RUN apt-get update && apt-get install -y ffmpeg locales python3-pip
RUN pip3 install youtube-dl

# Set the locale (required for youtube-dl)
RUN sed -i '/en_US.UTF-8/s/^# //g' /etc/locale.gen && \
    locale-gen
ENV LANG en_US.UTF-8  
ENV LANGUAGE en_US:en  
ENV LC_ALL en_US.UTF-8  

WORKDIR /ttt/src/
COPY . .

RUN dotnet restore

RUN cp /ttt/src/TicTacTubeDemo/log4net.config /ttt/src/log4net.config

CMD ["dotnet", "run", "--project", "TicTacTubeDemo"]