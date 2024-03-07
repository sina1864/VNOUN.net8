
# VNOUN - ASP.NET Core

This project implements an API for an e-commerce online store based on the VNOUN source code available on Codecanyon. You can see the features of this source code on Codecanyon. 
https://codecanyon.net/item/vnoun-vue3-nodejs-fully-responsive-online-store-mevn/38810680#

The original API was developed using Express.js technology for the API and Vue.js technology for the client side. Instead, this project uses ASP.NET Core technology to implement the API. All original API routes have been rewritten with no functional changes.








## Deployment

Steps to deploy this project:

**Step 1 (Install Git and clone project):**

Update the software list, install Git, and then download a copy of the Vnoun-dotnet8 project from GitHub:
```bash
  sudo apt update
  sudo apt-get install git
  git clone https://github.com/sina1864/Vnoun-dotnet8
```

**Step 2 (Install .NET SDK):**

Update the software list and install the .NET SDK version 8.0 with automatic acceptance of any prompts (like licenses):
```bash
  sudo apt-get update
  sudo apt-get install -y dotnet-sdk-8.0
```

**Step 3 (Install Nginx):**

Install Nginx web server:
```bash
  sudo apt install nginx
```

**Step 4 (Configure Nginx):**

Remove the default configuration file for Nginx, and then modify it:
```bash
  sudo rm /etc/nginx/sites-enabled/default
  nano /etc/nginx/sites-enabled/default
```
- copy and paste this sample:
```
server {
        listen 80 default_server;
        listen [::]:80 default_server;

        location / {   
                proxy_pass http://localhost:5120;
        }
}
```

**Step 5 (Restart Nginx):**

Kill and restart Nginx processes:
```bash
  lsof -i :80
  kill -9 pid
  kill -9 pid
  nginx
  systemctl status nginx
```
- If the status of the Nginx is not active, run the following commands and then run the commands of step 4 again:
```bash
  sudo rm /etc/nginx/sites-enabled/default
  sudo service nginx restart
```

**Step 6 (Run project):**

Go to the project directory and run it:
```bash
  ls
  cd Vnoun-dotnet8
  ls
  cd Vnoun.API
  nohup dotnet run & 1
```

**Step 7 (Obtain SSL certificate):**

Connect your domain to the server's IP by using an A record.

Install Certbot as a Snap package, create a symbolic link to make it globally accessible, and then use Certbot to obtain an SSL certificate for the Nginx server:
```bash
  sudo apt install snapd
  sudo snap install --classic certbot
  sudo ln -s /snap/bin/certbot /usr/bin/certbot
  sudo certbot --nginx
```
- add your email address and domain and also accept the rules (Y).

**Step 8 (View website):**

Go to `https://your_domain/swagger/index.html` to view your website.

**Redeploy after changes:**

If you intend to redeploy after the source code changes, run the following commands:
```bash
  ls
  cd Vnoun-dotnet8
  git pull https://github.com/sina1864/Vnoun-dotnet8
  lsof -i :5120
  kill -9 pid
  ls
  cd Vnoun.API
  nohup dotnet run & 1
```
