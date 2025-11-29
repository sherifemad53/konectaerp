# Finance Service API Documentation

This service handles all finance-related operations including budgets, expenses, invoices, payroll, and employee compensation.

## Endpoints

### Budgets (`api/Budgets`)

| Method | Endpoint | Description               | Query Parameters                                          | Response Type                    |
| ------ | -------- | ------------------------- | --------------------------------------------------------- | -------------------------------- |
| GET    | `/`      | Get all budgets           | `fiscalYear` (int?), `includeLines` (bool, default: true) | `IEnumerable<BudgetResponseDto>` |
| GET    | `/{id}`  | Get budget by ID          | `includeLines` (bool, default: true)                      | `BudgetResponseDto`              |
| POST   | `/`      | Create a new budget       | -                                                         | `BudgetResponseDto`              |
| PUT    | `/{id}`  | Update an existing budget | -                                                         | `void` (No Content)              |
| DELETE | `/{id}`  | Delete a budget           | -                                                         | `void` (No Content)              |

### Employee Compensation (`api/EmployeeCompensation`)

| Method | Endpoint                   | Description                                      | Response Type                               |
| ------ | -------------------------- | ------------------------------------------------ | ------------------------------------------- |
| POST   | `/`                        | Create or update employee compensation account   | `EmployeeCompensationResponseDto`           |
| GET    | `/{employeeId}`            | Get compensation account summary for an employee | `EmployeeCompensationResponseDto`           |
| PUT    | `/{employeeId}`            | Update compensation account details              | `EmployeeCompensationResponseDto`           |
| POST   | `/{employeeId}/bonuses`    | Add bonuses to an employee                       | `IEnumerable<EmployeeBonusResponseDto>`     |
| POST   | `/{employeeId}/deductions` | Add deductions to an employee                    | `IEnumerable<EmployeeDeductionResponseDto>` |

### Expenses (`api/Expenses`)

| Method | Endpoint | Description                | Query Parameters                                           | Response Type                     |
| ------ | -------- | -------------------------- | ---------------------------------------------------------- | --------------------------------- |
| GET    | `/`      | Get all expenses           | `category` (string?), `from` (DateTime?), `to` (DateTime?) | `IEnumerable<ExpenseResponseDto>` |
| GET    | `/{id}`  | Get expense by ID          | -                                                          | `ExpenseResponseDto`              |
| POST   | `/`      | Create a new expense       | -                                                          | `ExpenseResponseDto`              |
| PUT    | `/{id}`  | Update an existing expense | -                                                          | `void` (No Content)               |
| DELETE | `/{id}`  | Delete an expense          | -                                                          | `void` (No Content)               |

### Finance Summary (`api/FinanceSummary`)

| Method | Endpoint | Description         | Response Type       |
| ------ | -------- | ------------------- | ------------------- |
| GET    | `/`      | Get finance summary | `FinanceSummaryDto` |

### Invoices (`api/Invoices`)

| Method | Endpoint | Description                | Query Parameters                                          | Response Type                     |
| ------ | -------- | -------------------------- | --------------------------------------------------------- | --------------------------------- |
| GET    | `/`      | Get all invoices           | `status` (string?), `includeLines` (bool, default: false) | `IEnumerable<InvoiceResponseDto>` |
| GET    | `/{id}`  | Get invoice by ID          | `includeLines` (bool, default: true)                      | `InvoiceResponseDto`              |
| POST   | `/`      | Create a new invoice       | -                                                         | `InvoiceResponseDto`              |
| PUT    | `/{id}`  | Update an existing invoice | -                                                         | `void` (No Content)               |
| DELETE | `/{id}`  | Delete an invoice          | -                                                         | `void` (No Content)               |

### Payroll Runs (`api/PayrollRuns`)

| Method | Endpoint | Description                    | Query Parameters                                                              | Response Type                        |
| ------ | -------- | ------------------------------ | ----------------------------------------------------------------------------- | ------------------------------------ |
| GET    | `/`      | Get all payroll runs           | `from` (DateTime?), `to` (DateTime?), `includeEntries` (bool, default: false) | `IEnumerable<PayrollRunResponseDto>` |
| GET    | `/{id}`  | Get payroll run by ID          | `includeEntries` (bool, default: true)                                        | `PayrollRunResponseDto`              |
| POST   | `/`      | Create a new payroll run       | -                                                                             | `PayrollRunResponseDto`              |
| PUT    | `/{id}`  | Update an existing payroll run | -                                                                             | `void` (No Content)                  |
| DELETE | `/{id}`  | Delete a payroll run           | -                                                                             | `void` (No Content)                  |
