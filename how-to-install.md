# How to install XtraUpload on Linux

 1. To install **.NET Runtime and SDKs** on your Linux machine, please refer to Microsoft documentation at:
[Install .NET Core on Linux](https://docs.microsoft.com/en-us/dotnet/core/install/linux), please note that the SDK is used to build/package the application and it's not needed once the application is deployed on the server.
 2. Check if the .NET runtime is install successfully, open your terminal and type `dotnet --info` you should get a result like this:
[SDKs & runtime](https://photos.google.com/share/AF1QipOPxqzGwFwxSRmboRJUBE5V2AJStsU_-hoOhFZZb9dcnsbZXqHZceZDCr9T1eulRg/photo/AF1QipN76ujJrqiwJW250M7Ioh_6yyMAljXqyTSdubX8?key=TXRBSTNLbW9UUktYRVhsSjJFRVFkc2V2NFFRT1ZB)
3. Generate the sql script and update appsettings.json:
 3.1 First install EF core tools, open a terminal and type `dotnet  tool install --global dotnet-ef`
 3.2 Edit *XtraUpload.ServerApp/appsettings.json* by providing your db connection string (XtraUpload support MySql and Sql databases) [Database config](https://photos.google.com/share/AF1QipMlU3GzqtgztxF5h6tL1S_R8N6P1tUeIYRWTcZEsAh68sIzjrcLXIwXk7P9qMrdWQ?key=U2FJdmZOM29jbkV2d01PMHdVTTVOWEtMRnJrM21B)
 3.3 Generate a Migration file: in a terminal open XtraUpload.ServerApp-master and type 
 `dotnet ef migrations add MyMigrationName -p ./Database/XtraUpload.Database.Migrations -s XtraUpload.ServerApp` you should get the following result:  [Migration file](https://photos.app.goo.gl/R4T7FXv4sVTze8M57)
 3.4 Now that we have the migration file we can generate the sql script, in the same terminal type: `dotnet ef migrations script -o ./Database/XtraUpload.Database.Migrations/script.sql -p ./Database/XtraUpload.Database.Migrations -s XtraUpload.ServerApp`, you should get the following resuls: [Sql file](https://photos.app.goo.gl/Zujrae9mTusPkJ189)
 3.5 Update your database with the generated sql script, you can either import the script.sql directly to your MySql server interface or run the following command 
 `dotnet ef database update MyMigrationName -p ./Database/XtraUpload.Database.Migrations -s XtraUpload.ServerApp` you should see the newly generated  [database](https://photos.app.goo.gl/ofABvefPSCz3sqcY6).
 if you got any error, please make sure you provide the correct database connection string in _XtraUpload.ServerApp/appsettings.json_
 
 4. Build XtraUpload: Extract *XtraUpload.ServerApp-master.zip* then open a terminal in the XtraUpload.ServerApp-master's root directory, type the following command `dotnet  publish --configuration Release`
[Release build](https://photos.app.goo.gl/HkzX7yu8pjkyeaGj7)
 
 test workflow