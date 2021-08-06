Implementacion de de JWT en un API-Rest corriendo en NetCore version 5 con lenguaje C# 
#Build
##Para construir la base de datos es necesario instalr dotnet-ef (entityFrameWork)
<br>
```console
$ dotnet tool install --global dotnet-ef
```
<br>

## Ejecutar las migraciones

```console
$ dotnet ef migrations add UserRegistrationDto
```
<br>

## Correr API

```console
$ dotnet run
```
<br>
