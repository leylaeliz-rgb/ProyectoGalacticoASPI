using System.Text.Json.Nodes;
using ProyectoGalactico.Models;
using ProyectoGalactico.Dtos;

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
                            Consulta personajes, cartas y eventos, y simula batallas.

                            **Cómo jugar**
                            1. `GET /personajes` para ver quién existe (filtra con `faccion` y `fuerzaSensitivo`).
                            2. `GET /eventos` para ver los eventos en orden cronológico.
                            3. `POST /eventos/{id}/simular` para simular una batalla: Rebelde contra Imperio, y los neutrales dan apoyo al bando más débil.
                            4. `GET /personajes/ranking?por=poder` para ver quién es el más poderoso.

                            **Poder** = peligrosidad de la mejor carta + 2 si el personaje es sensible a la Fuerza.

                            **Seed**: cada simulación devuelve su `seed`. Mándala en `?seed=` para repetir exactamente la misma batalla.
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
                            Crea y modifica personajes, cartas y eventos.

                            **Códigos**
                            - Facción: `R` Rebelde, `I` Imperio, `N` Neutral.
                            - Estado: `V` vivo, `M` muerto, `D` desconocido.

                            **Fechas** de eventos: `10 BBY`, `3 ABY` o `Batalla de Yavin`.

                            **Reglas**
                            - Un personaje pasa a muerto solo cuando un evento lo registra en `deadIds`.
                            - Nadie puede participar en un evento posterior a su muerte.
                            - No se puede borrar un personaje que participa en eventos.
                            - Un personaje puede tener varias cartas: cuenta la de mayor peligrosidad.
                            """
                    };
                    return Task.CompletedTask;
                });

                // Ejemplos prellenados en los cuerpos de POST y PUT
                options.AddSchemaTransformer((schema, context, ct) =>
                {
                    var type = context.JsonTypeInfo.Type;

                    if (type == typeof(CharacterCreateDto))
                        schema.Example = new JsonObject
                        {
                            ["name"] = "Bumblebee",
                            ["species"] = "Cybertroniano",
                            ["faccion"] = "I",
                            ["afiliacion"] = "Autobots",
                            ["estado"] = "V",
                            ["fuerzaSensitivo"] = false
                        };
                    else if (type == typeof(CardCharacterCreateDto))
                        schema.Example = new JsonObject
                        {
                            ["characterId"] = 3,
                            ["power"] = "Liderazgo",
                            ["specialHability"] = "Discurso inspirador",
                            ["weapon"] = "Blaster",
                            ["dangerousLevel"] = 6,
                            ["pictureUrl"] = "https://ejemplo.com/leia2.png"
                        };
                    else if (type == typeof(EventCreateDto))
                        schema.Example = new JsonObject
                        {
                            ["name"] = "Rescate en Bespin",
                            ["date"] = "3 ABY",
                            ["location"] = "Bespin",
                            ["description"] = "Los rebeldes intentan escapar de la Ciudad de las Nubes.",
                            ["participantIds"] = new JsonArray(1, 2, 3, 4),
                            ["deadIds"] = new JsonArray()
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