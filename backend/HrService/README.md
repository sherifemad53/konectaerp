# HrService API Documentation

This document lists all the API endpoints exposed by the HrService.

## Attendance

Base URL: `api/Attendance`

| Method | Endpoint | Description                                                                 |
| ------ | -------- | --------------------------------------------------------------------------- |
| GET    | `/`      | Get attendance records. Query params: `employeeId`, `startDate`, `endDate`. |
| GET    | `/{id}`  | Get a specific attendance record by ID.                                     |
| POST   | `/`      | Create a new attendance record.                                             |
| PUT    | `/{id}`  | Update an existing attendance record.                                       |
| DELETE | `/{id}`  | Delete an attendance record.                                                |

## Department

Base URL: `api/Department`

| Method | Endpoint        | Description                       |
| ------ | --------------- | --------------------------------- |
| GET    | `/`             | Get all departments.              |
| GET    | `/{id}`         | Get a specific department by ID.  |
| POST   | `/`             | Create a new department.          |
| PUT    | `/{id}`         | Update an existing department.    |
| DELETE | `/{id}`         | Delete a department.              |
| PUT    | `/{id}/manager` | Assign a manager to a department. |

## Employee

Base URL: `api/employee` or `api/employees`

| Method | Endpoint           | Description                      |
| ------ | ------------------ | -------------------------------- |
| GET    | `/`                | Get all employees.               |
| GET    | `/{id}`            | Get a specific employee by ID.   |
| POST   | `/`                | Add a new employee.              |
| PUT    | `/{id}`            | Update an existing employee.     |
| POST   | `/{id}/bonuses`    | Issue bonuses to an employee.    |
| POST   | `/{id}/deductions` | Issue deductions to an employee. |
| POST   | `/{id}/fire`       | Terminate an employee.           |

## HrSummary

Base URL: `api/HrSummary`

| Method | Endpoint | Description                                                                                            |
| ------ | -------- | ------------------------------------------------------------------------------------------------------ |
| GET    | `/`      | Get HR summary statistics (total employees, active employees, department count, pending resignations). |

## Interviews

Base URL: `api/Interviews`

| Method | Endpoint | Description                                      |
| ------ | -------- | ------------------------------------------------ |
| GET    | `/`      | Get interviews. Query param: `jobApplicationId`. |
| GET    | `/{id}`  | Get a specific interview by ID.                  |
| POST   | `/`      | Schedule a new interview.                        |
| PUT    | `/{id}`  | Update an interview.                             |
| DELETE | `/{id}`  | Delete an interview.                             |

## JobApplications

Base URL: `api/JobApplications`

| Method | Endpoint | Description                                        |
| ------ | -------- | -------------------------------------------------- |
| GET    | `/`      | Get job applications. Query param: `jobOpeningId`. |
| GET    | `/{id}`  | Get a specific job application by ID.              |
| POST   | `/`      | Create a new job application.                      |
| PUT    | `/{id}`  | Update a job application.                          |
| DELETE | `/{id}`  | Delete a job application.                          |

## JobOpenings

Base URL: `api/JobOpenings`

| Method | Endpoint | Description                                                            |
| ------ | -------- | ---------------------------------------------------------------------- |
| GET    | `/`      | Get job openings. Query params: `departmentId`, `includeApplications`. |
| GET    | `/{id}`  | Get a specific job opening by ID. Query param: `includeApplications`.  |
| POST   | `/`      | Create a new job opening.                                              |
| PUT    | `/{id}`  | Update a job opening.                                                  |
| DELETE | `/{id}`  | Delete a job opening.                                                  |

## LeaveRequests

Base URL: `api/LeaveRequests`

| Method | Endpoint | Description                                                    |
| ------ | -------- | -------------------------------------------------------------- |
| GET    | `/`      | Get leave requests. Query params: `employeeId`, `pendingOnly`. |
| GET    | `/{id}`  | Get a specific leave request by ID.                            |
| POST   | `/`      | Create a new leave request.                                    |
| PUT    | `/{id}`  | Update a leave request.                                        |
| DELETE | `/{id}`  | Delete a leave request.                                        |

## ResignationRequests

Base URL: `api/ResignationRequests`

| Method | Endpoint         | Description                                      |
| ------ | ---------------- | ------------------------------------------------ |
| POST   | `/`              | Submit a resignation request.                    |
| GET    | `/`              | Get resignation requests. Query param: `status`. |
| GET    | `/{id}`          | Get a specific resignation request by ID.        |
| PUT    | `/{id}/decision` | Approve or reject a resignation request.         |
