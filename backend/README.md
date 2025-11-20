# hackathon
## Prereqs:
- Docker Desktop
- .NET 8.0 SDK

Dodac czwartego workera jako msssql observer i nadawacz wiadomosci zwrotnej do hostedservice w glownym api
ta wiadomosc bedzie przekazana wyzej do UI przez signalR z poziomu hostedservice
https://www.bing.com/search?pglt=297&q=hostedservice+signalR+example+c%23&cvid=6e28f1e459d04c6a9f977bba17712f8c&gs_lcrp=EgRlZGdlKgYIABBFGDkyBggAEEUYOTIGCAEQABhAMgYIAhAAGEAyBggDEAAYQDIGCAQQABhAMgYIBRAAGEAyBggGEAAYQDIGCAcQABhAMgYICBAAGEDSAQg2MDczajBqMagCALACAA&FORM=ANSPA1&PC=U531


## Docker command
```
podman run -p 9000:9000 -p 9009:9009 -p 8812:8812 -p 9003:9003 questdb/questdb:8.1.1
podman run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Password_123#" -p 1500:1433 --name sql_server_container mcr.microsoft.com/mssql/server
podman run -d -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management
```
