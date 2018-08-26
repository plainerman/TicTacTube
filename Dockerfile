FROM slavik0/docker-alpine-phantomjs

RUN apk update \
	&& apk add youtube-dl