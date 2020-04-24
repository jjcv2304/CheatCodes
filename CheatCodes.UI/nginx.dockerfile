FROM nginx:alpine
LABEL author="Juan"
COPY ./config/nginx.conf /etc/nginx/conf.d/default.conf

# STEPS
# ng build --watch --delete-output-path false
# docker build -t nginx-angular -f nginx.dockerfile .
# docker run -p 4200:80  -v ${PWD}/dist:/usr/share/nginx/html nginx-angular




