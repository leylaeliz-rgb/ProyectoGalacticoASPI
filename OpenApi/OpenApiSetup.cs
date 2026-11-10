using ProyectoGalactico.Dtos;
using ProyectoGalactico.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;

namespace ProyectoGalactico.OpenApi
{
    public static class OpenApiSetup
    {
        public static IServiceCollection AddGalacticOpenApi(this IServiceCollection services)
        {
            services.AddOpenApi("v1", options =>
            {
                options.AddDocumentTransformer((document, context, ct) =>
                {
                    document.Info = new()
                    {
                        Title = "Proyecto Galáctico: Todos contra todos (Juego)",
                        Version = "v1",
                        Description = """
                            Consulta personajes, cartas y eventos, y simula batallas del universo Transformers.

                            **Cómo jugar**
                            1. `GET /personajes` para ver quién existe (filtra con `faction` y `sensoryForce`).
                            2. `GET /eventos` para ver los eventos en orden cronológico (filtra con `characterId`, `desde` y `hasta`).
                            3. `GET /eventos/{id}/mvp` para ver qué participante tiene más poder en un evento.
                            4. `POST /eventos/{id}/simular` para simular la batalla: Autobots contra Decepticons, y los neutrales apoyan al bando más débil.
                            5. `GET /personajes/{id}/historial` para ver qué le pasó a un personaje en cada batalla.
                            6. `GET /personajes/ranking?por=poder` para ver quién es el más poderoso.

                            **Conceptos básicos**
                            - **Personaje**: un combatiente. Puede tener varias cartas.
                            - **Carta**: la ficha de combate de un personaje (poder, habilidad especial, arma, peligrosidad y probabilidad de supervivencia). Al pelear se usa su carta disponible de mayor peligrosidad.
                            - **Evento**: un hecho en la línea de tiempo con sus participantes. Solo se puede simular una vez.

                            **Significado de los valores**
                            - Facción (`faction`): `Autobot`, `Decepticon` o `Neutral`. Los neutrales no pelean: solo dan apoyo.
                            - Estado del personaje (`characterState`): `Alive` (vivo), `Dead` (muerto, no puede volver a participar) y `Unknown` (desconocido: desapareció; al participar tiene 50% de probabilidad de reaparecer).
                            - Resultado de batalla (`outcome`):
                              - `Unharmed`: salió ileso (su bando ganó, hubo empate o es neutral).
                              - `Survived`: su bando perdió pero sobrevivió; su carta queda inutilizada.
                              - `Died`: su bando perdió y murió; su carta queda inutilizada.
                              - `Disappeared`: su bando perdió y desapareció; pasa a estado `Unknown`.
                              - `DidNotAppear`: estaba en estado `Unknown` y no reapareció para la batalla.
                            - Sensible a la Fuerza (`sensoryForce`): si es `true`, suma +2 al poder del personaje.
                            - Peligrosidad (`dangerousLevel`): de 1 a 10. Supervivencia (`survivalRatio`): de 1 a 100.
                            - Disponible (`isAvailable`): `false` significa que la carta quedó inutilizada.
                            - Años: negativo = `AP` (ej. `10 AP`), `0` = `Caída de los Primes` y positivo = `DP` (ej. `3 DP`). Los filtros `desde` y `hasta` usan este formato.

                            **Poder** = peligrosidad de la mejor carta + 2 si el personaje es sensible a la Fuerza.

                            **Cómo se simula una batalla**
                            1. Los personajes en estado `Unknown` tiran de 1 a 100: reaparecen con 50 o menos.
                            2. Cada participante usa su mejor carta disponible. Si alguien no tiene ninguna, la simulación falla.
                            3. Debe haber al menos un Autobot y un Decepticon.
                            4. Cada bando suma el poder de sus miembros. Los neutrales aportan la mitad de su poder total como apoyo al bando con menos poder (si empatan, se reparte por igual).
                            5. El total de cada bando se multiplica por un factor aleatorio entre 0.9 y 1.1. Gana el total más alto; si son iguales es `Empate`.
                            6. Cada miembro del bando perdedor tira de 1 a 100 contra el `survivalRatio` de su carta: menor = `Survived`, mayor = `Died` e igual = `Disappeared`.
                            7. El ganador se guarda cifrado en el evento.

                            **Seed**: cada simulación devuelve su `seed`. Mándala en `?seed=` para que el azar sea reproducible con los mismos datos de partida.
                            """
                    };
                    return Task.CompletedTask;
                });
            });

            services.AddOpenApi("v2", options =>
            {
                options.AddDocumentTransformer((document, context, ct) =>
                {
                    document.Info = new()
                    {
                        Title = "Proyecto Galáctico: Zona de Administración",
                        Version = "v2",
                        Description = """
                            Crea y modifica personajes, cartas y eventos. Las consultas y la simulación están en el documento v1.

                            **Valores de texto (enums)**
                            Se escriben como texto; no importan las mayúsculas.
                            - Facción (`faction`): `Autobot`, `Decepticon` o `Neutral`.
                            - Estado (`characterState`): `Alive` (vivo), `Dead` (muerto) y `Unknown` (desconocido). No se envía al crear ni al modificar: lo cambian las batallas.

                            **Conceptos básicos**
                            - **Personaje**: un combatiente. Al crearlo queda vivo (`Alive`) y activo (`isActive`). `sensoryForce` indica si es sensible a la Fuerza (+2 de poder).
                            - **Carta**: ficha de combate de un personaje existente. `dangerousLevel` va de 1 a 10, `survivalRatio` de 1 a 100 (50 por defecto) y `pictureUrl` debe ser una URL http o https. Empieza disponible (`isAvailable = true`) y se inutiliza en batalla.
                            - **Evento**: un hecho en la línea de tiempo. `year` es un entero: negativo = `AP`, `0` = `Caída de los Primes` y positivo = `DP`.

                            **Reglas**
                            - Un personaje pasa a muerto solo cuando una batalla lo registra en `deadIds`.
                            - Nadie puede participar en un evento del mismo año o posterior a su muerte.
                            - No se puede borrar un personaje que participa en eventos.
                            - Un personaje puede tener varias cartas: cuenta la de mayor peligrosidad disponible.
                            - Un evento necesita al menos un participante, sin repetidos y todos existentes.
                            - `autorizationCode` (de 8 a 100 caracteres) se guarda con hash y no se devuelve. Para modificar un evento hay que enviar el mismo código; si es incorrecto responde 403.
                            - Un evento ya simulado no se puede modificar.
                            - El ganador (`winner`) se guarda cifrado en la base y se muestra descifrado.
                            """
                    };
                    return Task.CompletedTask;
                });

                // Ejemplos prellenados en los cuerpos de POST y PUT
                options.AddSchemaTransformer((schema, context, ct) =>
                {
                    var type = context.JsonTypeInfo.Type;

                    if (type == typeof(CharacterCreateDto))
                        schema.Examples = new List < JsonNode>
                        {
                        new JsonObject 
                        {
                            ["name"] = "Bumblebee",
                            ["species"] = "Cybertroniano",
                            ["faction"] = "Autobot",
                            ["sensoryForce"] = false
                            }
                        };
                    else if (type == typeof(CardCharacterCreateDto))
                        schema.Examples = new List<JsonNode>
                        {
                        new JsonObject
                        {
                            ["characterId"] = 3,
                            ["power"] = "Liderazgo",
                            ["specialHability"] = "Discurso inspirador",
                            ["weapon"] = "Blaster",
                            ["dangerousLevel"] = 6,
                            ["pictureUrl"] = "https://i.pinimg.com/736x/01/63/78/01637820c08fda36de1cb929ebf4da5b.jpg"
                        }
                        };
                    else if (type == typeof(EventCreateDto))
                        schema.Examples = new List<JsonNode>
                        {
                        new JsonObject
                        {
                            ["autorizationCode"] = "ContraseñaSegura123",
                            ["name"] = "Resurgimiento de un nuevo Prime",
                            ["date"] = 4,
                            ["location"] = "Ciudad de Iacon",
                            ["description"] = "Los mineros, los cybertronianos buscan un futuro mejor, un fututo más justo.",
                            ["participantsIds"] = new JsonArray(1, 2, 3, 4),
                        }
                        };

                    return Task.CompletedTask;
                });
            });

            return services;
        }

        public static WebApplication UseGalacticSwagger(this WebApplication app)
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "Proyecto Galáctico: Todos contra todos - Juego");
                options.SwaggerEndpoint("/openapi/v2.json", "Proyecto Galáctico: Zona de Administración");
                options.DocumentTitle = "Manual de Operaciones - Proyecto Galáctico";
                options.EnableFilter();          // barra de búsqueda de endpoints
                options.DisplayRequestDuration();
            });
            return app;
        }
    }
}