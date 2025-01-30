#!/usr/bin/env bash
sudo apt-get update
sudo apt-get install -y nala
wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb``
rm packages-microsoft-prod.deb
sudo nala update
sudo nala upgrade -y
sudo nala install dotnet-sdk-9.0 dotnet-runtime-8.0 git -y
dotnet tool install powershell -g
dotnet tool restore && dotnet restore