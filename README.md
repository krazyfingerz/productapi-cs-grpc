# 🚀 ProductAPI (gRPC + In-Memory + Docker + CI/CD)

A production-style **gRPC-based Product Management API** built with .NET.
(Temporary README file)

This project demonstrates a clean, layered backend architecture with:
- gRPC services (HTTP/2)
- In-memory storage (no database)
- Caching layer
- AutoMapper mapping
- Interceptors for global error handling
- Unit + Integration testing
- Docker containerization
- GitHub Actions CI/CD pipeline

---

# 📦 Features

## CRUD Operations

- Create Product
- Get Product by ID
- List Products
- Update Product
- Delete Product

---

## Product Model

Each product contains:

- `Id` (GUID)
- `Name` (string)
- `Description` (string)
- `Price` (double)
- `Available` (bool)

---

# 🧱 Architecture Overview

```text
Client
  ↓
gRPC Service Layer
  ↓
Interceptor Layer (Global Exception Handling)
  ↓
Caching Layer (IMemoryCache)
  ↓
Repository Layer (In-Memory Thread-Safe Store)
  ↓
ConcurrentDictionary (Data Storage)