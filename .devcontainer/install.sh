#!/usr/bin/env bash
wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install dotnet-sdk-9.0 dotnet-runtime-8.0 git -y
git config --global --add safe.directory .
dotnet tool install powershell -g
mkdir -p /config/.config/powershell/
cp .devcontainer/Microsoft.PowerShell_profile.ps1 /config/.config/powershell/
pwsh
dotnet tool restore && dotnet restore