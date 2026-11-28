#3  riesgos reales encontrados en el proyecto a la fecha de 27/11/2026#
- Post, Put y Delete en personajes
- Post, Put y Delete en cartas
- Post Eventos
Cualquiera sin token puede borrar un personaje, y con el cascade tambien puede borrar su historial de batallas

Hasta el momento lo unico con authorize es POST/eventos/{id}/simular y GET/autorizacon/evento-actual