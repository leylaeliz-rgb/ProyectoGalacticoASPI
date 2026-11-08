## Requisitos

- .NET 10 SDK
- MySQL Server 8.0 en ejecución
- Herramienta EF Core (ajustar `--version` a la de los paquetes del proyecto):

```
dotnet tool install --global dotnet-ef --version 9.0.0
dotnet ef --version
```

## 1. Crear la base de datos y el usuario

Con el usuario `root`:

```sql
CREATE DATABASE galactico CHARACTER SET utf8mb4;

CREATE USER 'galactico_user'@'localhost' IDENTIFIED BY 'ElijaUnaClaveSegura';

GRANT ALL PRIVILEGES ON galactico.* TO 'galactico_user'@'localhost';
FLUSH PRIVILEGES;
```

Cambia al nuevo usuario y comprueba que la base está vacía:

```
mysql -u galactico_user -p galactico
SHOW TABLES;
```

## 2. Clonar e instalar dependencias

```
git clone URL_DEL_REPOSITORIO
cd ProyectoGalactico
dotnet restore
```

Si faltan paquetes:

```
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0
dotnet build
```

## 3. Configurar los secretos

Los secretos no están en el repositorio. Cada persona genera los suyos.

```
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=galactico;User=galactico_user;Password=SU_CLAVE"
```

La clave de cifrado debe ser de 32 bytes aleatorios en Base64.

Windows (PowerShell):

```
$bytes = New-Object byte[] 32
[System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
[Convert]::ToBase64String($bytes)
```

macOS y Linux:

```
openssl rand -base64 32
```

Guardarla y verificar:

```
dotnet user-secrets set "Cifrado:Clave" "CLAVE_BASE64_DE_32_BYTES"
dotnet user-secrets list
```

## 4. Aplicar las migraciones

```
dotnet ef database update
```

Si falla la conexión, probar indicando la cadena directamente:

```
dotnet ef database update --connection "Server=127.0.0.1;Port=3306;Database=galactico;Uid=galactico_user;Password=SU_CLAVE;SslMode=none"
```

## 5. Revisar la base de datos

```
SHOW TABLES;
DESCRIBE Characters;
SHOW CREATE TABLE Events;
SELECT MigrationId FROM __EFMigrationsHistory;
```

## 6. Ejecutar la API

```
dotnet run
```

## Si hay problemas con la cadena de conexión

```
dotnet user-secrets list
dotnet user-secrets remove "ConnectionStrings:DefaultConnection"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=galactico;User=galactico_user;Password=SU_CLAVE"
```
