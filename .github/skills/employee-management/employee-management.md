# GitHub Copilot Instructions

## Project Overview

This repository contains an Employee Management System.

Technology stack:

- C#
- Windows Forms (WinForms)
- .NET 10
- Microsoft SQL Server
- Entity Framework

The application manages employees, departments, positions, and related
employee information.

---

## General Development Rules

When generating or modifying code:

- Follow the existing project architecture.
- Prefer modifying existing classes over creating unnecessary new classes.
- Do not introduce new NuGet packages unless necessary.
- Keep solutions simple and maintainable.
- Do not rewrite unrelated code.
- Preserve existing functionality when implementing changes.
- Follow the naming conventions already used by the project.
- Use nullable-reference-type-safe code where applicable.
- Dispose `IDisposable` objects correctly.
- Test Driven Development must be used

Before creating a new helper, service, repository, model, or utility,
check whether equivalent functionality already exists in the project.

---

## C# Coding Style

Use PascalCase for:

- Classes
- Methods
- Properties
- Public members

Use camelCase for:

- Local variables
- Method parameters
- Private variables unless the existing project uses another convention

Use meaningful names.

Prefer:

```csharp
Employee employee;
departmentId;
LoadEmployees();
SaveEmployee();