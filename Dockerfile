FROM alpine:3.20 as rq-build

RUN apk add --update supervisor vim curl
RUN mkdir -p /var/log/supervisor
COPY supervisord.conf /etc/supervisor/conf.d/supervisord.conf

RUN echo "*/1    *       *       *       *       run-parts /etc/periodic/5min" | crontab -
COPY ./schedules /etc/periodic

RUN mkdir /tmp/null

ARG PORT=8085
ENV PORT ${PORT}
CMD ["/usr/bin/supervisord", "-c", "/etc/supervisor/conf.d/supervisord.conf"]