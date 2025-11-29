# Inventory Service API Documentation

This service handles all inventory-related operations including items, warehouses, stock operations, and transactions.

## Endpoints

### Inventory Items (`api/InventoryItems`)

| Method | Endpoint | Description                       | Query Parameters     |
| ------ | -------- | --------------------------------- | -------------------- |
| GET    | `/`      | Get all inventory items           | `category` (string?) |
| GET    | `/{id}`  | Get inventory item by ID          | -                    |
| POST   | `/`      | Create a new inventory item       | -                    |
| PUT    | `/{id}`  | Update an existing inventory item | -                    |
| DELETE | `/{id}`  | Delete an inventory item          | -                    |

### Inventory Summary (`api/inventory-summary`)

| Method | Endpoint | Description           |
| ------ | -------- | --------------------- |
| GET    | `/`      | Get inventory summary |

### Stock Operations (`api/stock-operations`)

| Method | Endpoint             | Description                       |
| ------ | -------------------- | --------------------------------- |
| POST   | `/adjust`            | Adjust stock for an item          |
| POST   | `/transfer`          | Transfer stock between warehouses |
| GET    | `/transaction-types` | Get available transaction types   |

### Stock Transactions (`api/StockTransactions`)

| Method | Endpoint | Description            | Query Parameters                                                                               |
| ------ | -------- | ---------------------- | ---------------------------------------------------------------------------------------------- |
| GET    | `/`      | Get stock transactions | `page` (int, default: 1), `pageSize` (int, default: 50), `itemId` (int?), `warehouseId` (int?) |

### Warehouses (`api/Warehouses`)

| Method | Endpoint | Description                  |
| ------ | -------- | ---------------------------- |
| GET    | `/`      | Get all warehouses           |
| GET    | `/{id}`  | Get warehouse by ID          |
| POST   | `/`      | Create a new warehouse       |
| PUT    | `/{id}`  | Update an existing warehouse |
| DELETE | `/{id}`  | Delete a warehouse           |
