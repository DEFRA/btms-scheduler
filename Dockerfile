FROM alpine:3.20 AS rq-build

RUN apk add --update supervisor vim curl
RUN mkdir -p /var/log/supervisor
COPY supervisord.conf /etc/supervisor/conf.d/supervisord.conf

ENV REPLICATE_DATALAKE=${REPLICATE_DATALAKE:-"false"}

#RUN echo "*/1    *       *       *       *       run-parts /etc/periodic/5min" | crontab -
#RUN echo "0    2       *       *       *       run-parts /etc/periodic/nightly-2amUTC" | crontab -
COPY ./schedules /etc/periodic
COPY ./crontabs.root /etc/crontabs/root

RUN mkdir /tmp/null

ARG PORT=8085
ENV PORT ${PORT}
CMD ["/usr/bin/supervisord", "-c", "/etc/supervisor/conf.d/supervisord.conf"]