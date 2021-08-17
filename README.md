# ⚠️ DEPRECATION NOTICE ⚠️

This repository has been deprecated. Please visit the [new Gameboard project](https://github.com/cmu-sei/gameboard) for the latest code.

---

# Gameboard API

Developed by Carnegie Mellon University's Software Engineering Institute (SEI), **Gameboard** is a flexible web platform that provides game design capabilities and a competition-ready user interface. The Gameboard API works in conjunction with the [Gameboard UI](https://github.com/cmu-sei/gameboard-ui-v2) web client to deliver a full competition environment.

## Dependencies

The Gameboard API requires the .NET Core 2.2 framework.

## Getting Started

1. Install .Net Core SDK 2.2
2. Start the application using the following command: `dotnet run`
3. Browse to `http://localhost:5075/`

## Development `appsettings`

.NET Core supports multiple `appsettings` files that are read based on the environment. Currently, only the `Development` environment is used. This means that you can make a file named `appsettings.Development.json` in the same directory as `appsettings.json` and any settings in `appsettings.Development.json` will override the settings given in `appsettings.json`. Most importantly, it means you won't have any issues if the settings file changes and you want the latest code.

## Database
Currently, only PostgreSQL and SqlServer are supported.

Once you have your database configured, you'll need to add the connection string from `appsettings.json` into your `appsettings.Development.json`. Add the Section and the key/value you want to use:

    "ConnectionStrings": {
      "SqlServer": "Server=localhost;Port=5432;Database=gameboard_api;Username=;Password=;",
      "PostgreSQL": "Server=localhost;Port=5432;Database=gameboard_api;Username=;Password=;"
    }

Add the Username and Password for your database into this line.

## Authentication and Authorization
The gameboard requires an Identity server. Configuring Identity is outside the scope of this Readme. However, on your Identity server, add a client named `gameboard-api` and a scope also named `gameboard-api`.

Once the identity server is up and running, you'll need to add some additional lines in
`appsettings.Development.json`. Find the following lines:

    "Authorization": {
      "Authority": "https://localhost:5000",
      "AuthorizationUrl": "https://localhost:5000/connect/authorize",
    }

...and update the domains/ports in these lines to point to your Identity server.

## Game Engine Dependency
The API leverages the GameEngine services to be the primary point of data storage for game and challenge
scores and details. You will need to configure your GameEngine settings as described below:

    "GameEngine": {
      "GameId": "dev-game",
      "GameEngineUrl": "http://localhost:5001",
      "GameEngineKey": "solo#f1229d4c97"
    }

## Leaderboard In Game Caching
The leaderboard can become increasingly volatile during a large competition. To address this the leaderboard is built for consumption on a set interval, using the defined cache key, and gives the ability to hide participant names, based on the following lines:

    "Leaderboard": {
      "CacheKey": "leaderboard",
      "IntervalMinutes": 1,
      "Anonymize": false
    }

## Organization Selection Definition
The API supports associating teams with different organizations. These organizations can be defined within the `appsettings.json` file. Organization adoption can be restricted to a **ClaimKey** which must match when a user associates their team with an organization. Alternatively, organizations can be disabled.

    "Organization": {
      "IsEnabled": true,
      "ClaimKey": "",
      "Items": [
        {
          "Name": "",
          "Title": "",
          "Logo": ""
        },
    	{
          "Name": "",
          "Title": "",
          "Logo": ""
        }
      ]
    }

## Mail Integration
If you are using the Foundry Identity Server, which supports a simple email relay service, you can configure it using the settings below. Find the following lines:

    "Authorization": {
      "Authority": "http://localhost:5000",
      "AuthorizationScope": "foundry",
      "ClientId": "gameboard-api",
      "ClientSecret": "",
      "Endpoint":  "/api/priv/msg"
    }

...and update the settings to point to your instance.

## Change Port
By default, Gameboard API listens on port 5075. In case you want to change this, open the file
`Properties/launchSettings.json` and find the line:

    "applicationUrl": "http://localhost:5075",

...and change the port in this line.
