Implementacion de de JWT en un API-Rest corriendo en NetCore version 5 con lenguaje C# 
#Build
##Para construir la base de datos es necesario instalr dotnet-ef (entityFrameWork)
´dotnet tool install --global dotnet-ef´
## Ejecutar las migraciones
´dotnet ef migrations add UserRegistrationDto´
## Correr API
´dotnet run´