# Setup

## Creating database

You need to create a database named `SecurityAnalysisDemo` in your SQL Server Management Studio app. After this, run the following SQL in order to create the tables with the data necessary for the test:

```SQL
USE SecurityAnalysisDemo;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL,
        Email NVARCHAR(100) NOT NULL
    );

    INSERT INTO Users (Username, Email) VALUES
        ('juan', 'juan@example.com'),
        ('pedro', 'pedro@example.com'),
        ('jose', 'jose@example.com');
END
GO
```

## Running the project

After setting up the database, you can run the project in the Visual Studio IDE.

### Testing SQL injection

If you write `' OR '1' = '1` in the initial input and choose the unsecured method, you will get a list of all users with their emails. This is an example of how SQL injections can work.