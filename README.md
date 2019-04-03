# ClashRoyale
[![clash royale](https://img.shields.io/badge/Clash%20Royale-1.9.2-brightred.svg?style=flat")](https://clash-royale.en.uptodown.com/android/download/1632865)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)

A .NET Core Clash Royale Server (v1.9) using DotNetty

## Battles
The server supports battles, for those a patched client is neccessary.

[See the wiki for a tutorial](https://github.com/retroroyale/ClashRoyale/wiki/Patch-for-battles)

## How to start

#### Requirements:
  - [.NET Core SDK 2.x](https://dotnet.microsoft.com/download/dotnet-core/2.2)

for Ubuntu use these commands:
```
git clone https://github.com/retroroyale/ClashRoyale.git && cd ClashRoyale/ClashRoyale/ClashRoyale

dotnet restore "ClashRoyale.csproj"

dotnet publish "ClashRoyale.csproj" -c Release -o app
```
To configurate your server, such as the database you have to edit the ```config.json``` file.

#### Run the server:

```dotnet app/ClashRoyale.dll```

#### Update the server:

```git pull && dotnet publish "ClashRoyale.csproj" -c Release -o app && dotnet app/ClashRoyale.dll```

## Need help?
Contact me on Discord (Incredible#2109) or open an issue.
