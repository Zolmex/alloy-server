FROM mysql

ENV MYSQL_ROOT_PASSWORD=123123
ENV MYSQL_DATABASE=alloy

COPY schema.sql /docker-entrypoint-initdb.d/01-schema.sql
