##### Stage 1
FROM node:latest as node
LABEL author="Juan"
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
COPY . .
#RUN npm run build -- --prod
RUN npm run build

##### Stage 2
FROM nginx:alpine
VOLUME /var/cache/nginx
COPY --from=node /app/dist /usr/share/nginx/html
COPY ./config/nginx.conf /etc/nginx/conf.d/default.conf

# docker build -t nginx-angular-prod:1.0 -f nginx.prod.dockerfile .
# docker run -p 4200:80 nginx-angular-prod

#or docker pull jjcv2304/cheat-codes-angular:1.2.0
# docker run -p 4200:80 jjcv2304/cheat-codes-angular:1.2.0



