
## PARTE 1: ##
# Riesgos reales encontrados en el proyecto a la fecha de 27/11/2026#
- Post, Put y Delete en personajes
- Post, Put y Delete en cartas
- Post Eventos
Cualquiera sin token puede borrar un personaje, y con el cascade tambien puede borrar su historial de batallas

Hasta el momento lo unico con authorize es POST/eventos/{id}/simular y GET/autorizacon/evento-actual

Ya corregido la seguridad, solo el administrador esta autorizao a hacer correr esas rutas.
## PARTE 2: ##
# Autenticación y autorización #


## PARTE 3: ##

## Fragmento de log: petición completa

14:28:29.050 INF cid=1ebbd3ca ip=::1 fp=0ad6a98ea917 POST /autorizacion/admin respondió 200 en 33 ms (evento anónimo)
14:29:55.642 INF cid=3b158e1d ip=127.0.0.1 fp=2b9073812fea Personaje 15 creado
14:29:55.671 INF cid=3b158e1d ip=127.0.0.1 fp=2b9073812fea POST /personajes respondió 201 en 1548 ms (evento anónimo)


### Qué muestra cada dato

| `14:28:29.050` | Hora en que se escribió el log, con milisegundos. 
| `INF` | Nivel del log. `INF` es información normal; `WRN` indicaría algo que merece atención, como un 403. 
| `cid` | Identificador de correlación: 8 caracteres generados al azar para cada petición. Se devuelve también en la cabecera `X-Correlation-Id` y permite reunir todas las líneas de una misma petición. 
| `ip` | Dirección del cliente. `::1` y `127.0.0.1` son la computadora local (localhost) en IPv6 e IPv4. 
| `fp` | Huella del cliente: los primeros 12 caracteres del SHA-256 de la IP junto con el User-Agent. El mismo cliente produce siempre la misma huella, sin guardar datos más precisos de la persona. 
| Mensaje | Método y ruta, código de estado, tiempo de respuesta en milisegundos y evento autorizado. 

### Lectura del fragmento

1. **Primera línea:** una petición `POST /autorizacion/admin` que respondió 200 en 33 ms. Es el login de administrador: se envió el código y la API devolvió el token. Aún no había token, por eso dice "evento anónimo".
2. **Segunda y tercera línea:** comparten `cid=3b158e1d`, así que pertenecen a la misma petición, `POST /personajes`. La segunda es un log del servicio ("Personaje 15 creado"), escrito mientras se procesaba la petición. La tercera es el resumen del middleware, escrito al terminar: respondió 201 (creado) en 1548 ms. El `cid` compartido permite ver juntas las dos.
3. **Huellas distintas:** la primera línea tiene `fp=0ad6a98ea917` e `ip=::1`, y las otras dos `fp=2b9073812fea` e `ip=127.0.0.1`. Son dos clientes diferentes, porque la huella depende de la IP y del User-Agent.
4. **Tiempos:** 33 ms contra 1548 ms. La creación del personaje tardó más porque escribe en la base de datos, y es probable que además fuera la primera consulta tras arrancar la aplicación.