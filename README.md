# fe_engine [![build](https://img.shields.io/github/workflow/status/yodasoda1219/fe_engine/build)](https://github.com/yodasoda1219/fe_engine/actions/workflows/build.yml) [![build-native](https://img.shields.io/github/workflow/status/yodasoda1219/fe_engine/build-native/native?label=build%20%28native%29)](https://github.com/yodasoda1219/fe_engine/actions/workflows/build-native.yml)
## About
This project aims to recreate the Fire Emblem: Three Houses game engine. The gameplay side, I mean. All game mechanics are referenced off of [this](https://fe3h.com/) website.
## What's this Fire Emblem?
Fire Emblem is a tactical role-playing game developed by Intelligent Systems and published by Nintendo. The premise of each map is that you (the player) have a set of units (your army) and need to defeat the enemy's army, controlled by the game's AI. Each unit has a set amount of HP, and once that HP count reaches zero, they die, and once you lose a unit, it's gone from later maps.
## Building
Simply install [.NET Core](https://dotnet.microsoft.com/download) and run `dotnet build` while in the root directory of this repository.