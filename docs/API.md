# Referencia de API

Base URL predeterminada:

```text
https://localhost:7147
```

En modo HTTP:

```text
http://localhost:5055
```

Los endpoints marcados como protegidos requieren el encabezado:

```http
Authorization: Bearer <jwt>
```

Los tokens expiran después de 30 minutos.

## Convenciones

- Tipo de contenido: `application/json`.
- Los identificadores de usuario son cadenas generadas por ASP.NET Core Identity.
- Las fechas usan el formato ISO `YYYY-MM-DD`.
- Las rutas contienen algunos parámetros como parte del segmento; deben copiarse sin espacios.
- Los errores comunes devuelven `400`, `401`, `404` o `500` con texto plano o detalles de Identity.

## Sesiones

### Registrar usuario

```http
POST /registar
```

Entrada:

```json
{
  "name": "usuario",
  "email": "usuario@example.com",
  "password": "ClaveSegura123"
}
```

Validaciones:

- `name`: obligatorio y mínimo 3 caracteres.
- `email`: obligatorio y con formato de correo.
- `password`: obligatorio, mínimo 8 caracteres y según las reglas configuradas de Identity.

Respuesta exitosa: `200 OK`

```text
User Created Successfully
```

### Iniciar sesión

```http
POST /sign
```

Entrada:

```json
{
  "email": "usuario@example.com",
  "password": "ClaveSegura123"
}
```

Respuesta exitosa: `200 OK`

```json
{
  "name": "usuario",
  "token": "<jwt>",
  "scoredTaskDtos": [],
  "friendsDtos": [],
  "miTaskDtos": [],
  "taskNoteds": []
}
```

Errores:

- `401 Unauthorized`: credenciales inválidas.
- `500 Internal Server Error`: error interno.

### Cerrar sesión

```http
POST /signout
Authorization: Bearer <jwt>
```

Protegido con JWT.

Respuesta exitosa: `200 OK`

```text
Session Closed Successfully
```

El token se agrega a la lista negra en memoria y deja de ser aceptado por el middleware.

## Amigos

### Agregar una amistad

```http
POST /addfriend
Authorization: Bearer <jwt>
Content-Type: application/json

{
  "userId": "id-del-usuario",
  "friendId": "id-del-amigo"
}
```

Protegido con JWT.

Comportamiento:

- El servidor reemplaza `userId` por el identificador del usuario autenticado.
- Se crea la relación con estado `Pending`.
- Se registra la fecha actual.

Respuestas:

- `200 OK`: `"Friend Added Successfully"`
- `400 Bad Request`: datos inválidos.
- `500 Internal Server Error`: no fue posible crear la relación.

### Cambiar estado de una amistad

```http
PATCH /assignedstatus={id},{status}
Authorization: Bearer <jwt>
```

Ejemplo:

```http
PATCH /assignedstatus=12,Accepted
```

Estados admitidos:

- `Pending`
- `Accepted`
- `Rejected`
- `Block`

Solo los participantes de la relación pueden modificarla. El usuario que recibió la solicitud no puede asignar directamente `Block`.

Respuestas:

- `200 OK`: `"Status Assigned Successfully"`
- `400 Bad Request`: datos inválidos.
- `500 Internal Server Error`: no fue posible actualizar.

## Tareas

### Crear una tarea

```http
POST /addtask
Authorization: Bearer <jwt>
Content-Type: application/json

{
  "userId": "id-del-usuario",
  "taskId": 0,
  "description": "Preparar informe mensual",
  "prioritis": "High",
  "term": "2026-12-31",
  "resource": "Repositorio del proyecto",
  "status": "Pending",
  "dependecy": "Aprobación del supervisor",
  "subTasks": "Reunir datos",
  "comments": "Entregar antes del cierre",
  "expectedResults": "Informe completo"
}
```

Protegido con JWT.

Validaciones principales:

- `description`: máximo 400 caracteres.
- `prioritis`: máximo 100 caracteres.
- `resource`: máximo 100 caracteres.
- `status`: máximo 20 caracteres.
- `dependecy`, `subTasks`, `comments` y `expectedResults`: máximo 100 caracteres.
- `term`: fecha ISO.

El servidor reemplaza `userId` por el usuario autenticado y normaliza:

- Prioridad: `High`, `Medium` o `Low`; cualquier otro valor queda como `Low`.
- Estado: `Pending`, `InProgress` o `Completed`; cualquier otro valor queda como `Pending`.

Respuesta exitosa: `200 OK`

```text
Task Added Successfully
```

### Cambiar estado de una tarea

```http
PATCH /taskstatus={taskId},{status}
Authorization: Bearer <jwt>
```

Ejemplo:

```http
PATCH /taskstatus=25,InProgress
```

Solo el propietario de la tarea puede cambiar su estado.

Estados admitidos:

- `Pending`
- `InProgress`
- `Completed`

Respuestas:

- `200 OK`: `"Successfully assigned status"`
- `400 Bad Request`: datos inválidos.
- `404 Not Found`: no fue posible actualizar.

### Eliminar una tarea

```http
DELETE /taskremove={taskId}
Authorization: Bearer <jwt>
```

Ejemplo:

```http
DELETE /taskremove=25
```

Solo el propietario puede eliminarla.

Respuestas:

- `204 No Content`
- `400 Bad Request`: datos inválidos.
- `404 Not Found`: no fue posible encontrar o eliminar.

## Tareas asignadas

### Crear una asignación

```http
POST /addscoredtask
Authorization: Bearer <jwt>
Content-Type: application/json

{
  "idUser": "id-del-usuario-asignado",
  "idTask": 25,
  "details": "Responsable de revisar y completar la tarea"
}
```

Protegido con JWT.

Comportamiento:

- Solo el propietario de la tarea indicada puede crear la asignación.
- El estado inicial se establece como `Pending`.
- `details` admite máximo 400 caracteres.

Respuestas:

- `200 OK`: `"Task assigned Successfully"`
- `400 Bad Request`: datos inválidos.
- `500 Internal Server Error`: no fue posible crear la asignación.

### Cambiar estado de una asignación

```http
PATCH /scoretaskstatus={scoredTaskId},{status}
Authorization: Bearer <jwt>
```

Ejemplo:

```http
PATCH /scoretaskstatus=9,Accepted
```

Estados admitidos:

- `Pending`
- `Accepted`
- `Rejected`
- `Block`

Pueden participar el propietario de la tarea y el usuario asignado, con excepción de que el usuario asignado no puede establecer `Block`.

Respuestas:

- `200 OK`: `"Successfully assigned status"`
- `400 Bad Request`: datos inválidos.
- `404 Not Found`: no fue posible actualizar.

### Eliminar una asignación

```http
DELETE /removescoretask={taskId}
Authorization: Bearer <jwt>
```

Ejemplo:

```http
DELETE /removescoretask=25
```

Solo el propietario de la tarea original puede eliminar la asignación asociada.

Respuestas:

- `204 No Content`
- `400 Bad Request`: datos inválidos.
- `404 Not Found`: no fue posible encontrar o eliminar.

## Contratos de datos

### Register

| Campo | Tipo | Obligatorio | Restricciones |
| --- | --- | --- | --- |
| `name` | string | Sí | Mínimo 3 caracteres |
| `email` | string | Sí | Formato de correo |
| `password` | string | Sí | Mínimo 8 caracteres |

### Login

| Campo | Tipo | Obligatorio | Restricciones |
| --- | --- | --- | --- |
| `email` | string | Sí | Formato de correo |
| `password` | string | Sí | Mínimo 8 caracteres |

### FriendsDto

| Campo | Tipo | Obligatorio |
| --- | --- | --- |
| `userId` | string | Sí |
| `friendId` | string | Sí |

### MiTaskDto

| Campo | Tipo | Obligatorio | Restricción |
| --- | --- | --- | --- |
| `userId` | string | Sí | Se reemplaza por el usuario autenticado |
| `taskId` | integer | No | Generado por la base de datos |
| `description` | string | Sí | Máximo 400 |
| `prioritis` | string | Sí | Máximo 100 |
| `term` | string (date) | Sí | `YYYY-MM-DD` |
| `resource` | string | Sí | Máximo 100 |
| `status` | string | Sí | Máximo 20 |
| `dependecy` | string | Sí | Máximo 100 |
| `subTasks` | string | Sí | Máximo 100 |
| `comments` | string | Sí | Máximo 100 |
| `expectedResults` | string | Sí | Máximo 100 |

### ScoredTaskDto

| Campo | Tipo | Obligatorio | Restricción |
| --- | --- | --- | --- |
| `idUser` | string | Sí | Usuario asignado |
| `idTask` | integer | Sí | ID de una tarea existente |
| `details` | string | Sí | Máximo 400 |
