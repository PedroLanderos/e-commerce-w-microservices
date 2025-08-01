# 🛒 Sistema de E-Commerce - Llaveremos

Este proyecto es una solución de e-commerce basada en microservicios, desarrollada con .NET 8 y siguiendo principios de Clean Architecture. Cada servicio se encuentra desacoplado, con responsabilidad única, y cuenta con sus propias pruebas unitarias. Además, el sistema incluye una colección de Postman para facilitar las pruebas manuales de las APIs.

---

## 📁 Estructura del Proyecto

```plaintext
Llaveremos.ApiGatewaySolution
Llaveremos.AuthenticationApi.Solution
Llaveremos.OrderApiSolution
Llaveremos.ProductApiSolution
Llaveremos.SharedLibrarySolution
```

---

## ⚙️ Tecnologías Utilizadas

- .NET 8
- ASP.NET Core Web API
- Clean Architecture
  - Domain
  - Application
  - Infrastructure
  - Presentation
- JWT para autenticación
- Pruebas unitarias en cada microservicio
- Postman (colección de pruebas de endpoints)

---

## 🧱 Descripción de Microservicios

### 🔐 Authentication API
Encargado de la autenticación de usuarios y generación de tokens JWT.

### 📦 Product API
Gestión de productos: creación, edición, eliminación y listado.

### 📑 Order API
Procesamiento de pedidos realizados por los usuarios.

### 🌐 API Gateway
Punto de entrada unificado al sistema para enrutar las solicitudes a los microservicios correspondientes.

### 🧰 Shared Library
Contiene componentes reutilizables como modelos, constantes, interfaces, excepciones, etc.

---

## 🧪 Pruebas

Cada microservicio cuenta con su propio conjunto de pruebas unitarias para asegurar el correcto funcionamiento de su lógica interna.

Además, se incluye una colección de Postman para probar manualmente los endpoints disponibles.

---

## 📂 Organización del Código

Cada microservicio sigue el patrón de **Clean Architecture**, con la siguiente estructura de carpetas:

- `Domain`: Entidades y lógica de negocio pura
- `Application`: Casos de uso, DTOs, interfaces de servicios
- `Infrastructure`: Implementaciones técnicas (bases de datos, servicios externos)
- `Presentation`: Controladores, endpoints y validaciones


