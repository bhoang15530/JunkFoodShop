
# Junk Food Shop

A final project of K19. This project uses ASP.NET Core and SQL Server.



## Deployment

To deploy this project:

1. Clone the project using git

```bash
  git clone https://github.com/bhoang15530/JunkFoodShop.git
```

2. Open the `.sln` file with Microsoft Studio

3. Create a database named `JunkFoodShop` and run the `SQLCreateTable.sql` file in the `JunkFoodShop` folder

4. In the `SQLDatabaseQuery` folder, copy and run the `SQLCreateTable.sql` file in your database, then do the same with `CreateCategory.sql` and the remaining files with the number prefix respectively.

5. From your newly created database, right click on it and select Properties, in the ConnectionString cell, copy the value and paste it in the `appsettings.json` like this:
```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "AllowedHosts": "*",
    "ConnectionStrings": {
        "JunkFoodShop": <your-connection-string-here>
    }
}
```
