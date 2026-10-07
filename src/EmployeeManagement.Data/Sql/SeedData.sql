USE EmployeeManagement;
GO

INSERT INTO dbo.Departments (Name)
SELECT v.Name FROM (VALUES (N'Engineering'), (N'Human Resources'), (N'Sales'), (N'Finance'), (N'Marketing'), (N'IT Support'), (N'Operations')) v(Name)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Departments d WHERE d.Name = v.Name);
GO

INSERT INTO dbo.Positions (Title)
SELECT v.Title FROM (VALUES (N'Developer'), (N'Team Lead'), (N'HR Specialist'), (N'Sales Representative'), (N'Accountant'), (N'Marketing Specialist'), (N'Support Engineer'), (N'Operations Manager'), (N'Financial Analyst')) v(Title)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Positions p WHERE p.Title = v.Title);
GO

INSERT INTO dbo.Employees (FirstName, LastName, Email, HireDate, Salary, DepartmentId, PositionId)
SELECT v.FirstName, v.LastName, v.Email, v.HireDate, v.Salary,
	   (SELECT Id FROM dbo.Departments WHERE Name = v.Dept),
	   (SELECT Id FROM dbo.Positions WHERE Title = v.Pos)
FROM (VALUES
	(N'John',   N'Smith',   N'john.smith@example.com',   CAST('2020-03-01' AS DATE), CAST(65000 AS DECIMAL(18,2)), N'Engineering',     N'Developer'),
	(N'Emily',  N'Johnson', N'emily.johnson@example.com', CAST('2018-07-15' AS DATE), CAST(85000 AS DECIMAL(18,2)), N'Engineering',     N'Team Lead'),
	(N'Michael', N'Brown',  N'michael.brown@example.com', CAST('2021-01-10' AS DATE), CAST(48000 AS DECIMAL(18,2)), N'Human Resources', N'HR Specialist'),
	(N'Sarah',  N'Davis',   N'sarah.davis@example.com',   CAST('2022-05-23' AS DATE), CAST(52000 AS DECIMAL(18,2)), N'Sales',           N'Sales Representative'),
	(N'David',  N'Wilson',  NULL,                         CAST('2019-11-04' AS DATE), CAST(55000 AS DECIMAL(18,2)), N'Finance',         N'Accountant'),
	(N'Karen',  N'Lewis',   N'karen.lewis@example.com',   CAST('2017-02-20' AS DATE), CAST(50000 AS DECIMAL(18,2)), N'Marketing',       N'Marketing Specialist'),
	(N'Peter',  N'Walker',  N'peter.walker@example.com',  CAST('2016-09-12' AS DATE), CAST(60000 AS DECIMAL(18,2)), N'Operations',      N'Operations Manager')
) v(FirstName, LastName, Email, HireDate, Salary, Dept, Pos)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Employees e WHERE e.FirstName = v.FirstName AND e.LastName = v.LastName);
GO

UPDATE dbo.Employees SET IsDeleted = 1
WHERE Email IN (N'karen.lewis@example.com', N'peter.walker@example.com') AND IsDeleted = 0;
GO
