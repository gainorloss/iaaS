FROM nginx:1.24
WORKDIR /usr/share/nginx/html
EXPOSE 80

COPY . .
ENTRYPOINT ["nginx", "-g", "daemon off;"]