# Netsphere

## Requirements
- [.NET Core 3.1 Runtime](https://dotnet.microsoft.com/download/dotnet-core/3.1)

## Build
Open a terminal and run the command below in both Server and Client folder
```
dotnet build
```

## Running
Before starting the client application, make sure you've created a folder named "Repository" inside Client's folder. After that, place all files you would like to share with another peers inside of it and start the client, which can be done by running the command below(the same command can be used to run the Server).
```
dotnet run
```

## Demonstration
On first run, NetsphereClient will synchronize your repository folder with NetsphereServer

Demonstration of one peer requesting files to another peer, which is located in a MacOS system.
![Demonstration](Usage.gif)
