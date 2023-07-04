#!/bin/bash
sudo apt-get update -y &&
sudo apt-get install -y \
apt-transport-https \
ca-certificates \
curl \
gnupg-agent \
software-properties-common &&
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add - &&
sudo add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" &&
sudo apt-get update -y &&
sudo apt-get install docker-ce docker-ce-cli containerd.io -y &&
sudo usemod -aG docker ubuntu
sudo apt install -yu dotnet-sdk-7.0
sudo apt install -y nginx

echo "server {" | sudo tee -a /etc/nginx/conf.d/my-nginx.conf
echo "    listen 80;" | sudo tee -a /etc/nginx/conf.d/my-nginx.conf
echo "    server_name matamosca.cl;  # Replace with your domain name or server IP address" | sudo tee -a /etc/nginx/conf.d/my-nginx.conf
echo "" | sudo tee -a /etc/nginx/conf.d/my-nginx.conf
echo "    location / {" | sudo tee -a /etc/nginx/conf.d/my-nginx.conf
echo "        # Add your desired configuration here" | sudo tee -a /etc/nginx/conf.d/my-nginx.conf
echo "        # For example, proxy_pass to forward requests to another server:" | sudo tee -a /etc/nginx/conf.d/my-nginx.conf
echo "        proxy_pass http://localhost:5089;" | sudo tee -a /etc/nginx/conf.d/my-nginx.conf
echo "        proxy_set_header Host \$host;" | sudo tee -a /etc/nginx/conf.d/my-nginx.conf
echo "        proxy_set_header X-Real-IP \$remote_addr;" | sudo tee -a /etc/nginx/conf.d/my-nginx.conf
echo "        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;" | sudo tee -a /etc/nginx/conf.d/my-nginx.conf
echo "        proxy_set_header X-Forwarded-Proto \$scheme;" | sudo tee -a /etc/nginx/conf.d/my-nginx.conf
echo "    }" | sudo tee -a /etc/nginx/conf.d/my-nginx.conf
echo "}" | sudo tee -a /etc/nginx/conf.d/my-nginx.conf

sudo service nginx reload
