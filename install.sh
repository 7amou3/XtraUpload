#!/bin/bash

# Install script for Ubuntu 20.04
# To execute this install script, open a terminal and type: sudo chmod +x install.sh && sudo ./install.sh

# Install dotnet runtime (Ubuntu)
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update; \
  sudo apt-get install -y apt-transport-https && \
  sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-3.1  && \
  sudo apt-get install -y aspnetcore-runtime-3.1
  
# Install MySql
function updateMySqlAuth
{
  read -p "Enter new MySql password: " -s mySqlPassword && printf '\n'
  read -p "Confirm new password: " -s retypePassword && printf '\n'
  
  if [ $mySqlPassword == $retypePassword ]; then
    
    sudo mysql << EOF
     ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY '$mySqlPassword';
     FLUSH PRIVILEGES;
     exit;
EOF
    read -p "Please update the [XtraUpload.WebApi/appsettings.json] with the new database password, once it is done press [Enter] to continue." waiting
  else
    echo "Confirmation password does not match, try again"
    updateMySqlAuth
  fi
}

read -p "Do you want to install MySql database? (Y/N): " confirmMySql
if [ $confirmMySql == "Y" ] || [ $confirmMySql == "y" ]; then
  sudo apt-get install mysql-server && \
  	mysql_secure_installation && \
  	updateMySqlAuth
fi

# Install nginx
sudo apt install nginx && 
  sudo systemctl enable nginx
sudo ufw allow 'Nginx HTTP' && \ 
  sudo ufw allow 'Nginx HTTPS'


# Install nodejs to build the Angular App
sudo apt install nodejs && \
    sudo apt install npm

# Build the solution
buildDir="/var/www/xtraupload"
mkdir -m 755 -p $buildDir
dotnet publish --configuration Release -o $buildDir
mv "{$buildDir}/AngularApp/dist/*" "{$buildDir}/AngularApp" && rm "{$buildDir}/AngularApp/dist"

# Install entity framewrok tools to generate migrations and update the db
dotnet tool install --global dotnet-ef
dotnet ef migrations add initCommit -p ./Database/XtraUpload.Database.Migrations -s XtraUpload.WebApi
dotnet ef database update initCommit -p ./Database/XtraUpload.Database.Migrations -s XtraUpload.WebApi

# Uncomment if you want to generate the db manually
# dotnet ef migrations script -o ./Database/XtraUpload.Database.Migrations/script.sql -p ./Database/XtraUpload.Database.Migrations -s XtraUpload.WebApi
# mv ./Database/XtraUpload.Database.Migrations/script.sql $buildDir

# generate ssl certs for localhost. if you don't plan on using a reverse proxy, you can supply certs signed a by public CA (letsencrypt for exemple)
certDir="/home/certificates/localhost"
sudo mkdir -m 755 -p $certDir && cd $_

sudo cat <<EOF >/$certDir/https.config
[req]
default_bits       = 2048
default_md         = sha256
default_keyfile    = key.pem
prompt             = no
encrypt_key        = no

distinguished_name = req_distinguished_name
req_extensions     = v3_req
x509_extensions    = v3_ca

[req_distinguished_name]
commonName             = "XtraUpload localhost"

[v3_req]
subjectAltName      = @alt_ca_main
basicConstraints    = critical, CA:false
keyUsage            = critical, keyEncipherment
extendedKeyUsage    = critical, 1.3.6.1.5.5.7.3.1

[v3_ca]
basicConstraints            = critical, CA:TRUE
subjectKeyIdentifier        = hash
authorityKeyIdentifier      = keyid:always, issuer:always
subjectAltName              = @alt_ca_main
keyUsage                    = critical, cRLSign, digitalSignature, keyCertSign

[alt_ca_main]
DNS.1   = localhost
IP.1    = 127.0.0.1
EOF

sudo openssl req -config https.config -new -x509 -sha256 -newkey rsa:2048 -nodes -keyout localhost.key -days 3650 -out localhost.crt
sudo openssl pkcs12 -export -out localhost.pfx -inkey localhost.key -in localhost.crt -password pass:";xE)^C8wUH#vP)@5.YGpzv"
sudo chmod 644 localhost.crt localhost.pfx
# Trusting the self signed root certificate
sudo cp localhost.crt /usr/share/ca-certificates && sudo update-ca-certificates

# Generate a monitoring service 
sudo cat <<EOF >/etc/systemd/system/api-xtraupload.service
[Unit]
Description=XtraUpload Api Service

[Service]
WorkingDirectory=/var/www/xtraupload
ExecStart=/usr/bin/dotnet /var/www/xtraupload/XtraUpload.WebApi.dll
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=dotnet-example
User=root
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
EOF

sudo systemctl enable api-xtraupload.service
sudo systemctl start api-xtraupload
