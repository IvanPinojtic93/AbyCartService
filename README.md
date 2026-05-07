# Cart Service

Manages per-user shopping carts stored in Redis and handles checkout by publishing an order event. Built with ASP.NET Core Minimal APIs.

**Service port (Docker):** `5103`  
**Gateway prefix:** `/cart`

## Endpoints

| Method | Path               | Auth | Description                        |
|--------|--------------------|------|------------------------------------|
| GET    | `/cart/cart`       | User | Get current cart                   |
| PUT    | `/cart/cart`       | User | Replace cart contents              |
| DELETE | `/cart/cart`       | User | Clear cart                         |
| POST   | `/cart/checkout`   | User | Checkout — creates an order event  |

### PUT `/cart/cart` — request body

```json
{
  "items": [
    {
      "productId": "00000001-0000-0000-0000-000000000000",
      "productName": "Laptop Pro 15",
      "price": 1299.99,
      "quantity": 1,
      "stockItemId": "00000001-0000-0000-0002-000000000000"
    }
  ]
}
```

### POST `/cart/checkout` — request body

```json
{
  "deliveryAddress": "123 Main St, Springfield",
  "items": [
    {
      "productId": "00000001-0000-0000-0000-000000000000",
      "productName": "Laptop Pro 15",
      "price": 1299.99,
      "quantity": 1,
      "stockItemId": "00000001-0000-0000-0002-000000000000"
    }
  ]
}
```

## Configuration — `.env`

Copy `.env.example` to `.env` before starting:

```
cp .env.example .env
```

Key variables:

| Variable       | Description                             |
|----------------|-----------------------------------------|
| `SERVICE_PORT` | Host port (default `5103`)              |
| `JWT_SECRET`   | Must match the identity service secret  |
| `REDIS_URL`    | Redis connection string                 |
| `RABBITMQ_*`   | RabbitMQ connection details             |

## Running

```powershell
docker compose up -d --build
```

To start all services together, use the script in `gateway/`:

```powershell
..\gateway\start-all.ps1
```

## Debugging locally

1. Start all other services via Docker: `..\gateway\start-all.ps1`
2. In the gateway `.env`, switch the cart service URL to `host.docker.internal`:
   ```
   CART_URL=http://host.docker.internal:5103/
   ```
3. Copy `.env.example` to `.env`, set `ASPNETCORE_ENVIRONMENT=Development`.
4. Run the service from VS Code (`F5`) or:
   ```powershell
   cd src/CartService.API
   dotnet run
   ```
5. The Scalar API explorer is available at `http://localhost:5103/scalar/v1`.
6. Inspect Redis contents via Redis Commander at `http://localhost:8081` (start with `..\gateway\start-all.ps1 -DevTools`).

## Testing

```powershell
dotnet test tests/CartService.Tests/
```

## Postman

Use the collection at `gateway/webshop.postman_collection.json`. The **Cart** folder contains all requests.
