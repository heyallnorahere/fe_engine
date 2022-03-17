# fe_engine

## About
This project aims to recreate the Fire Emblem: Three Houses game engine. All game mechanics are referenced from [this website](https://fe3h.com/).

## What's this Fire Emblem?
Fire Emblem is a tactical role-playing game developed by Intelligent Systems and published by Nintendo. The premise of each map is that you (the player) have a set of units (your army) and need to defeat the enemy's army, controlled by the game's AI. Each unit has a set amount of HP, and once that HP count reaches zero, they die, and once you lose a unit, it's gone from later maps.

## Building
Simply install [.NET Core](https://dotnet.microsoft.com/download) and run `dotnet build -c Release` within the root directory of this repository.

In this repository there is an example command line application in `FEEngine.Cmdline`. Once the solution is built, run:
```bash
dotnet FEEngine.Cmdline/bin/Release/net6.0/FEEngine.Cmdline.dll
```

When on Unix, the console does not resize automatically. If the console is too small when the application starts up, resize it and press C. This will clear the console, and re-render the application.