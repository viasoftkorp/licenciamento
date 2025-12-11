FROM korp/dotnetcore:4.0

WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production
ENV URL_CONSUL=http://consul-1:8500

COPY bin/Release/net6.0/publish/ .
COPY entrypoint.sh .

RUN chmod +x ./entrypoint.sh

CMD ["./entrypoint.sh"]