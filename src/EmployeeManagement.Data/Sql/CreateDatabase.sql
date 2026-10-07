IF DB_ID(N'EmployeeManagement') IS NULL
	CREATE DATABASE EmployeeManagement;
GO

USE EmployeeManagement;
GO

IF OBJECT_ID(N'dbo.Departments', N'U') IS NULL
CREATE TABLE dbo.Departments
(
	Id        INT IDENTITY(1,1) NOT NULL,
	Name      NVARCHAR(100)     NOT NULL,
	IsDeleted BIT               NOT NULL CONSTRAINT DF_Departments_IsDeleted DEFAULT 0,
	CONSTRAINT PK_Departments PRIMARY KEY (Id),
	CONSTRAINT UQ_Departments_Name UNIQUE (Name)
);
GO

IF OBJECT_ID(N'dbo.Positions', N'U') IS NULL
CREATE TABLE dbo.Positions
(
	Id        INT IDENTITY(1,1) NOT NULL,
	Title     NVARCHAR(100)     NOT NULL,
	IsDeleted BIT               NOT NULL CONSTRAINT DF_Positions_IsDeleted DEFAULT 0,
	CONSTRAINT PK_Positions PRIMARY KEY (Id),
	CONSTRAINT UQ_Positions_Title UNIQUE (Title)
);
GO

IF OBJECT_ID(N'dbo.Employees', N'U') IS NULL
CREATE TABLE dbo.Employees
(
	Id           INT IDENTITY(1,1) NOT NULL,
	FirstName    NVARCHAR(100)     NOT NULL,
	LastName     NVARCHAR(100)     NOT NULL,
	Email        NVARCHAR(200)     NULL,
	HireDate     DATE              NOT NULL,
	Salary       DECIMAL(18,2)     NOT NULL,
	DepartmentId INT               NOT NULL,
	PositionId   INT               NOT NULL,
	IsDeleted    BIT               NOT NULL CONSTRAINT DF_Employees_IsDeleted DEFAULT 0,
	CONSTRAINT PK_Employees PRIMARY KEY (Id),
	CONSTRAINT FK_Employees_Departments FOREIGN KEY (DepartmentId) REFERENCES dbo.Departments (Id),
	CONSTRAINT FK_Employees_Positions FOREIGN KEY (PositionId) REFERENCES dbo.Positions (Id),
	CONSTRAINT CK_Employees_Salary CHECK (Salary >= 0)
);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Employees_DepartmentId')
	CREATE INDEX IX_Employees_DepartmentId ON dbo.Employees (DepartmentId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Employees_PositionId')
	CREATE INDEX IX_Employees_PositionId ON dbo.Employees (PositionId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_Employees_Email')
	CREATE UNIQUE INDEX UQ_Employees_Email ON dbo.Employees (Email) WHERE Email IS NOT NULL;
GO

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Employees') AND name = N'HireDate' AND system_type_id <> TYPE_ID(N'date'))
	ALTER TABLE dbo.Employees ALTER COLUMN HireDate DATE NOT NULL;
GO
