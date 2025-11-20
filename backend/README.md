# hackathon
## Prereqs:
- Docker Desktop
- .NET 8.0 SDK

## Docker command
```
podman run -p 9000:9000 -p 9009:9009 -p 8812:8812 -p 9003:9003 questdb/questdb:8.1.1
podman run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Password_123#" -p 1500:1433 --name sql_server_container mcr.microsoft.com/mssql/server
podman run -d -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management
```
