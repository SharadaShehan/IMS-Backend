# Inventory Management System for computer laboratories - Backend Project

## Local Development Setup

### Prerequisites

- [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Azure Blob Storage](https://azure.microsoft.com/en-us/services/storage/blobs/)
- [Application Insights](https://azure.microsoft.com/en-us/services/monitor/)
- [SendGrid](https://sendgrid.com/)
- [Authentication Server](https://github.com/CS3203-SEP-21-Group-22/authentication-server)

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

5. Create a new file named `appsettings.json` with the following content. Replace values in Uppercase with your own values.

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

6. Run the following command to start the application.
   ```
    dotnet run
   ```
