FROM nginx

RUN apt-get update
RUN apt-get install -y jq

ENV backendUrl=https://URLGATEWAY.korp.com.br

COPY /dist/license-management /usr/share/nginx/html
WORKDIR /usr/share/nginx/html

COPY default.conf /etc/nginx/conf.d/default.conf
RUN rm -f /usr/share/nginx/html/default.conf

COPY entrypoint.sh /
RUN chmod +x /entrypoint.sh
RUN rm -f /usr/share/nginx/html/entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]
