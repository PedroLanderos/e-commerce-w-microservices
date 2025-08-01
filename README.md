# 🛒 E-Commerce System - Llaveremos

This project is a microservices-based e-commerce solution, developed with .NET 8 and following Clean Architecture principles. Each service is decoupled, has a single responsibility, and includes its own unit tests. The system also provides a Postman collection to facilitate manual API testing.

---

## 📁 Project Structure

```plaintext
Llaveremos.ApiGatewaySolution
Llaveremos.AuthenticationApi.Solution
Llaveremos.OrderApiSolution
Llaveremos.ProductApiSolution
Llaveremos.SharedLibrarySolution
```

---

## ⚙️ Technologies Used

- .NET 8
- ASP.NET Core Web API
- Clean Architecture
  - Domain
  - Application
  - Infrastructure
  - Presentation
- JWT for authentication
- Unit tests in each microservice
- Postman (endpoint testing collection)

---

## 🧱 Microservices Description

### 🔐 Authentication API
Responsible for user authentication and JWT token generation.

### 📦 Product API
Manages products: creation, editing, deletion, and listing.

### 📑 Order API
Handles the processing of user orders.

### 🌐 API Gateway
Unified entry point for the system, routing requests to the appropriate microservices.

### 🧰 Shared Library
Contains reusable components such as models, constants, interfaces, exceptions, etc.

---

## 🧪 Testing

Each microservice has its own set of unit tests to ensure the correct operation of its internal logic.

A Postman collection is also included to manually test the available endpoints.

---

## 📂 Code Organization

Each microservice follows the **Clean Architecture** pattern with the following folder structure:

- `Domain`: Entities and pure business logic
- `Application`: Use cases, DTOs, service interfaces
- `Infrastructure`: Technical implementations (databases, external services)
- `Presentation`: Controllers, endpoints, and validations

