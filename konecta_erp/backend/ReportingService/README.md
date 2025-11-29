# Reporting Service API Documentation

This service aggregates data from other services and provides reports and summaries.

## Endpoints

Base URL: `/api/reports`

| Method | Endpoint        | Description                            | Response Type                                                       |
| ------ | --------------- | -------------------------------------- | ------------------------------------------------------------------- |
| GET    | `/overview`     | Get general overview of the ERP system | `OverviewDto`                                                       |
| GET    | `/finance`      | Get finance module summary             | `FinanceSummaryDto`                                                 |
| GET    | `/hr`           | Get HR module summary                  | `HrSummaryDto`                                                      |
| GET    | `/inventory`    | Get inventory module summary           | `InventorySummaryDto`                                               |
| GET    | `/export/pdf`   | Export comprehensive report as PDF     | `application/pdf`                                                   |
| GET    | `/export/excel` | Export comprehensive report as Excel   | `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet` |
