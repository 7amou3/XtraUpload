# How to install XtraUpload on Linux

 1. To install **.NET Runtime and SDKs** on your Linux machine, please refer to Microsoft documentation at:
[Install .NET Core on Linux](https://docs.microsoft.com/en-us/dotnet/core/install/linux), please note that the SDK is used to build/package the application and it's not needed once the application is deployed on the server (only the .NET runtime is needed on your procuction server).
 2. Check if the .NET runtime is install successfully, open your terminal and type `dotnet --info` you should get a result like this:
[SDKs & runtime](https://photos.google.com/share/AF1QipOPxqzGwFwxSRmboRJUBE5V2AJStsU_-hoOhFZZb9dcnsbZXqHZceZDCr9T1eulRg/photo/AF1QipN76ujJrqiwJW250M7Ioh_6yyMAljXqyTSdubX8?key=TXRBSTNLbW9UUktYRVhsSjJFRVFkc2V2NFFRT1ZB)
 3. Install NodeJs and npm: node js and npm are used to build the angular application and they are not needed on your production server, to install nodejs run the following command `sudo apt install nodejs` then type `nodejs -v` to check your nodejs version [node version](https://photos.app.goo.gl/tvLbAGkMuz5AXCnp7).
 Then install npm: `sudo apt install npm`
 4. Build XtraUpload: Open a terminal in the XtraUpload-master's root directory, type the following command `dotnet  publish --configuration Release`, it will take several minutes because the build process needs to restore and build nuget and npm packages. [Release build](https://photos.app.goo.gl/hA8q1Fw8iZL5NwjD9), 
 PS: if you are building on a local machine the generated **publish** folder contain the files that you can deploy to your production server.
5. Once the build is done, we need to generate the sql script and update appsettings.json:
 5.1 First install EF core tools, open a terminal and type `dotnet  tool install --global dotnet-ef`
 5.2 In the publish folder edit *appsettings.json* by providing your db connection string (XtraUpload support both MySql and Sql databases) [Database config](https://photos.app.goo.gl/fqz4A5WC5Yrhb1Z38)
 5.3 Generate a Migration file: in a terminal navigate to the root folder XtraUpload-master and type 
 `dotnet ef migrations add MyMigrationName -p ./Database/XtraUpload.Database.Migrations -s XtraUpload.WebApp` you should get the following result:  [Migration folder](https://photos.app.goo.gl/4GoWuAocxoKHYVLw8)
 5.4 Now that we have the migration folder we can generate the sql script, in the same terminal type: `dotnet ef migrations script -o ./Database/XtraUpload.Database.Migrations/script.sql -p ./Database/XtraUpload.Database.Migrations -s XtraUpload.WebApp`, you should get the following resuls: [Sql file](https://photos.app.goo.gl/cJgnnNq3nxwaLhvCA)
 5.5 Update your database with the generated sql script, you can either import the script.sql directly to your MySql server interface or run the following command 
 `dotnet ef database update MyMigrationName -p ./Database/XtraUpload.Database.Migrations -s XtraUpload.WebApp` you should see the newly generated  [database](https://photos.app.goo.gl/z5XZvwgKRb4KBzaW9).
 *PS*: if you would like to run the command, you need to update the *ConnectionString* in *XtraUpload.WebApp/appsettings.json*
 if you got any error, please make sure you provide the correct database connection string in _XtraUpload.WebApp/appsettings.json_
 6. Run the application: the .NET runtime came with kestrel webserver, you can start  this http server by running the command`dotnet XtraUpload.WebApp.dll` navigate to localhost:5000/login you should get the following [login page](https://photos.app.goo.gl/TLerv4DRMrUU9tgZ8) 
PS: I moved the publish folder to desktop you can put it wherever you like on your server.
7. The default admin account credentials are:
	email: admin@admin.com
	password: admin01
	Please check the application health in your [admin panel](https://photos.app.goo.gl/1tqTDfphSde14zSf8) 
	and fix any issue, generally you need to specify the upload directory and grant read/write permission to 	the filestore folder, settings can be found in the `Settings` tab.
