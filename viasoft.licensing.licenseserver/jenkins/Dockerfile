FROM korp/dotnetcore:4.1

WORKDIR /app
ARG SERVICE_NAME
ARG COMMIT_HASH
ARG BUILD_VERSION
LABEL br.com.korp.commit=${COMMIT_HASH} \
      br.com.korp.version=${BUILD_VERSION}
ENV ASPNETCORE_ENVIRONMENT=Production
ENV URL_CONSUL=http://consul-1:8500
ENV SERVICE_NAME ${SERVICE_NAME}
COPY bin/Release/net6.0/publish/ .
COPY entrypoint.sh .

RUN rm -rf /app/LicenseServerSettings.json

RUN chmod +x ./entrypoint.sh

CMD ["./entrypoint.sh"]
