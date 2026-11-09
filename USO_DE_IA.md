Uso de Inteligencia Artificial
Se utilizó ChatGPT (OpenAI)
1. Configuración y trabajo con Git
   - Se solicitó ayuda para resolver problemas de sincronización,
     conflictos y publicación de cambios en GitHub.
   - También se pidió orientación para explicar a los integrantes del
     equipo cómo replicar el procedimiento de trabajo.
   - La asistencia se utilizó como guía para comprender los mensajes
     de Git y elegir los pasos adecuados.
2. Implementación de la lógica de batallas
   - Se consultó cómo organizar la simulación de batallas y sus
     consecuencias para los personajes.
   - Se trabajó sobre reglas de supervivencia, muerte y desaparición
     según el resultado aleatorio y el porcentaje de supervivencia de
     la carta.
   - Se definió que, cuando el resultado aleatorio es igual al
     porcentaje de supervivencia, el personaje desaparece y queda en
     estado Unknown.
   - También se revisó la reaparición de personajes y la selección de
     cartas disponibles.
3. Historial de batallas e interfaces de servicios
   - Se pidió orientación para incorporar el historial de batallas y
     exponerlo mediante un endpoint.
   - Se revisó la necesidad de definir una interfaz IBattleService,
     implementar el método de consulta del historial y registrar el
     servicio en la configuración de dependencias.
   - Se buscó mantener separados los contratos de los servicios
     existentes y evitar agregar responsabilidades de batalla a una
     interfaz que no correspondía.
4. Pruebas funcionales
   - Después de comprobar que el proyecto compilaba, se solicitó una
     lista de pruebas para ejecutar desde Swagger.
   - La lista contempló consultas de personajes, cartas y eventos;
     simulación; consecuencias; reaparición; historial y persistencia
     en la base de datos.
   - Se utilizaron los datos existentes de personajes, cartas y
     eventos para proponer escenarios de prueba.
5. Preparación de la base de datos
   - Se pidió una guía para eliminar los eventos y sus registros de
     historial, y establecer a todos los personajes como vivos.
   - La respuesta propuso realizar una copia de seguridad, ejecutar
     las operaciones dentro de una transacción y comprobar los
     resultados antes de confirmar los cambios.
   - También se señaló que la restauración de disponibilidad de las
     cartas era una operación opcional y separada.
6. Corrección de una petición JSON
   - Se compartió un error HTTP 400 al intentar crear un evento.
   - Se pidió ayuda para interpretar el mensaje y corregir el JSON.
   - Se identificó una coma sobrante al final de la lista
     participantsIds, que impedía deserializar correctamente la
     petición.
Qué se aceptó, qué se corrigió y por qué
- Se aceptó la orientación para organizar las pruebas por
  funcionalidad y ejecutarlas progresivamente, porque permite detectar
  errores sin alterar todos los datos de una sola vez.
- Se aceptó la propuesta de separar el contrato de batalla en
  IBattleService, manteniendo las interfaces de personajes, cartas y
  eventos con sus responsabilidades correspondientes.
- Se mantuvo la regla de supervivencia acordada: un resultado
  menor al porcentaje implica supervivencia; uno mayor implica muerte;
  la igualdad implica desaparición y estado Unknown. Esta regla se
  conservó porque forma parte de las reglas definidas para el
  proyecto.
- Se corrigió el JSON de creación del evento eliminando la coma
  final de participantsIds, porque el formato JSON no admite ese
  separador después del último elemento.
- Se recomendó proteger la base de datos con una copia de
  seguridad y una transacción antes de borrar eventos o modificar
  estados, para reducir el riesgo de pérdida accidental de
  información.
- Las recomendaciones y fragmentos propuestos por la IA se revisaron
  en función de la estructura del proyecto y de los datos existentes.
  No se debe interpretar que cada sugerencia fue aplicada
  automáticamente.
Alcance y responsabilidad
ChatGPT se utilizó como apoyo, no como sustituto de las decisiones del
equipo. Las reglas del juego, la revisión de los cambios, la ejecución
de las pruebas y la validación de los resultados corresponden al equipo
de desarrollo.

La compilación correcta fue comprobada por el usuario. Las pruebas
funcionales propuestas deben ejecutarse y verificarse en el entorno del
proyecto; una recomendación de la IA no constituye por sí sola evidencia
de que una funcionalidad funciona.
Declaración de uso
Sí se utilizó inteligencia artificial. Se utilizó ChatGPT para
asistencia técnica, explicación de errores, orientación de
implementación y planificación de pruebas. No se afirma que la IA haya
desarrollado o validado por sí sola la totalidad del proyecto.
