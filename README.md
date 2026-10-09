# Employee Management

An example project demonstrating some of my programming skills.

## Development Approach

This project was built using **Augmented Coding**. I didn't write a single line of code myself. Instead, I reviewed the generated code, made architectural decisions, and instructed the AI to follow **Test-Driven Development (TDD)** practices.

The web frontend was built using **Vibe Coding**, as I don't have experience with web frontend development.

## Building the Applications

Run the `build.bat` script from the solution root directory. It will build and publish all applications to the `bin` folder in project `root`.

## Database Configuration

Before running the applications, update the database connection string in the `appsettings.json` file for each of the following applications:

- `EmployeeManagement.UI`
- `EmployeeManagement.Api`
- `EmployeeManagement.Web`

Database scripts for creating the required tables and inserting test data are located at:

```text id="d6ivks"
..\EmployeeManagement\src\EmployeeManagement.Data\Sql\
```

Run the database scripts before starting the applications.

## Running the Applications

After running `build.bat`, the applications are available in their respective directories under the `bin` folder.

### WinForms Application

Start the WinForms desktop application by running:

```text id="cdy02g"
bin\EmployeeManagement.UI\EmployeeManagement.UI.exe
```

The WinForms application can be used independently.

### Web Application

The web application requires both the **API** and **Web** applications to be running.

#### 1. Start the API

Run:

```text id="wgnz0b"
bin\EmployeeManagement.Api\EmployeeManagement.Api.exe
```

The API will be available at:

```text id="79q3hd"
http://localhost:5000
```

Keep the API console window running.

#### 2. Start the Web Application

Run:

```text id="a62o9y"
bin\EmployeeManagement.Web\EmployeeManagement.Web.exe
```

The Web application will be available at:

```text id="5prcqx"
http://localhost:5001
```

Keep the Web console window running.

#### 3. Open the Application

Open the following address in your web browser:

```text id="5tyh1r"
http://localhost:5001
```

The Web application communicates with the API running at `http://localhost:5000`, so both applications must remain running while using the Web interface.
