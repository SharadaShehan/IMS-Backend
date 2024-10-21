# Inventory Management System for computer laboratories - Backend Project

## Local Development Setup

### Prerequisites

- [.NET v8+](https://dotnet.microsoft.com/download/dotnet/8.0) installed on your local machine.
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) installed on your local machine or running on a server.
- [Azure Blob Storage](https://azure.microsoft.com/en-us/services/storage/blobs/) with a container created.
- [Application Insights](https://azure.microsoft.com/en-us/services/monitor/) resource created.
- [SendGrid](https://sendgrid.com/) account with an API key and a verified sender email.
- [Authentication Server](https://github.com/CS3203-SEP-21-Group-22/authentication-server) setup and running.

#### Save the connection strings for the SQL Server database, Application Insights resource and Azure Blob Storage container as well as the API key and sender email for SendGrid. You will need these values to configure the application.

### Steps

1. Clone the repository.

   ```
   git clone https://github.com/CS3203-SEP-21-Group-22/IMS-Backend.git
   ```

2. Navigate to the project directory.

   ```
   cd IMS-Backend
   ```

3. Checkout to the development branch.

   ```
   git checkout dev
   ```

4. Navigate to the `IMS.Presentation` directory.

   ```
   cd IMS.Presentation
   ```

5. Restore missing packages (optional but recommended).

   ```
   dotnet restore
   ```

6. Create a new file named `appsettings.json` with the following content. Replace values in Uppercase with values you saved in the prerequisites step. For QRToken Secret, you can use any random string.

   ```json
   {
     "AllowedHosts": "*",
     "ConnectionStrings": {
       "DBConnection": "CONNECTON_STRING_OF_YOUR_SQL_SERVER_DATABASE",
       "ApplicationInsights": "CONNECTON_STRING_OF_YOUR_APPLICATION_INSIGHTS_RESOURCE"
     },
     "AuthenticationServer": {
       "Endpoint": "URL_OF_YOUR_AUTHENTICATION_SERVER",
       "ClientId": "CLIENT_ID_REGISTERED_IN_AUTHENTICATION_SERVER",
       "ClientSecret": "CLIENT_SECRET_REGISTERED_IN_AUTHENTICATION_SERVER"
     },
     "Jwt": {
       "Key": "CLIENT_SECRET_REGISTERED_IN_AUTHENTICATION_SERVER",
       "Issuer": "JwtIssuer",
       "Audience": "JwtAudience",
       "Subject": "JwtSubject"
     },
     "AzureBlobStorage": {
       "ContainerName": "NAME_OF_YOUR_AZURE_BLOB_STORAGE_CONTAINER",
       "ConnectionString": "CONNECTON_STRING_OF_YOUR_AZURE_BLOB_STORAGE"
     },
     "EmailClient": {
       "APIKey": "API_KEY_OF_YOUR_EMAIL_SERVICE_PROVIDER",
       "SenderEmail": "EMAIL_ADDRESS_OF_THE_SENDER"
     },
     "QRToken": {
       "Secret": "SECRET_FOR_GENERATING_QR_TOKENS"
     },
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     }
   }
   ```

7. Apply database migrations by running the following command.

   ```
   dotnet ef database update
   ```

8. Run the following command to start the application.
   ```
   dotnet run
   ```
