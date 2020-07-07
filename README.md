![Build pipelines](https://github.com/veteran1/XtraUpload/workflows/Build%20pipelines/badge.svg)


# How to install XtraUpload on Linux
You can Install XtraUpload by executing a shell script or manually, we recommend using the automated script.
## Things to Know Before Installing
Before you begin the install, there are a few things you need to have and do.
1. Access to your web server (eg. FTP, PuTTY, Remote Desktop...)
1. Ability to create MySql Server databases (XtraUpload also support SQL Server databases)
1. An [FTP Client](https://filezilla-project.org/)
## 5-Minute Installation (shell script)
1. Upload and unzip the XtraUpload package on your server.
1. Create a MySQL database user who has all privileges for accessing and modifying it.
1. In XtraUpload.WebApp folder edit `appsettings.json` and provide your database connection string (user, and password) [Database config](https://photos.app.goo.gl/fqz4A5WC5Yrhb1Z38).
1. In XtraUpload rootfolder give execution permission to `install.sh` script by running the command `chmod +x ./install.sh`
1. Run the script, once the installation is done navigate to localhost:5000 to login to your admin panel.
1. XtraUpload is running on Kestrel webserver at http://localhost:5000, Kestrel is great for serving .Net but it's not a full-blown webserver, we highly recommand putting a reverse proxy (Nginx or Apache) in front of Kestrel, follow this guide to setup a [reverse proxy](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-3.1#configure-a-reverse-proxy-server)
## Manual Installation, 

1. To install **.NET Runtime and SDKs** on your Linux machine, please refer to Microsoft documentation at:
[Install .NET Core on Linux](https://docs.microsoft.com/en-us/dotnet/core/install/linux), please note that the SDK is used to build/package the application and it's not needed once the application is deployed (only the .NET runtime is needed on your production server).
1. Check if the .NET runtime is install successfully, open your terminal and type `dotnet --info` you should get a result like this:
[SDKs & runtime](https://photos.google.com/share/AF1QipOPxqzGwFwxSRmboRJUBE5V2AJStsU_-hoOhFZZb9dcnsbZXqHZceZDCr9T1eulRg/photo/AF1QipN76ujJrqiwJW250M7Ioh_6yyMAljXqyTSdubX8?key=TXRBSTNLbW9UUktYRVhsSjJFRVFkc2V2NFFRT1ZB)
1. Install NodeJs and npm: node js and npm are used to build the angular application and they are not needed on your production server, to install nodejs run the following command `sudo apt install nodejs` then type `nodejs -v` to check your nodejs version [node version](https://photos.app.goo.gl/tvLbAGkMuz5AXCnp7).  
 Then install npm: `sudo apt install npm`
1. Build XtraUpload: in a terminal navigate to `XtraUpload-master`'s root directory, type the following command `dotnet  publish --configuration Release`, it will take several minutes because the build process needs to restore and build nuget and npm packages. [Release build](https://photos.app.goo.gl/hA8q1Fw8iZL5NwjD9).   
 PS: if you are building on a local machine the generated **publish** folder contain the files that you can deploy to your production server.
1. Once the build is done, we need to generate the sql script and update appsettings.json:
   1. First install EF core tools, open a terminal and type `dotnet  tool install --global dotnet-ef`
   1. In the publish folder edit *appsettings.json* by providing your db connection string (XtraUpload support both MySql and Sql databases) [Database config](https://photos.app.goo.gl/fqz4A5WC5Yrhb1Z38)
   1. Generate a Migration file: in a terminal navigate to the root folder XtraUpload-master and type 
 `dotnet ef migrations add MyMigrationName -p ./Database/XtraUpload.Database.Migrations -s XtraUpload.WebApp` you should get the following result:  [Migration folder](https://photos.app.goo.gl/4GoWuAocxoKHYVLw8)
   1. Now that we have the migration folder we can generate the sql script, in the same terminal type: `dotnet ef migrations script -o ./Database/XtraUpload.Database.Migrations/script.sql -p ./Database/XtraUpload.Database.Migrations -s XtraUpload.WebApp`, you should get the following result: [Sql file](https://photos.app.goo.gl/cJgnnNq3nxwaLhvCA)
   1. Update your database with the generated sql script, you can either import the script.sql directly to your MySql server interface or run the following command 
 `dotnet ef database update MyMigrationName -p ./Database/XtraUpload.Database.Migrations -s XtraUpload.WebApp` you should see the newly generated  [database](https://photos.app.goo.gl/z5XZvwgKRb4KBzaW9).  
 *PS*: if you would like to run the command, you need to update the *ConnectionString* in *XtraUpload.WebApp/appsettings.json*
 if you got any error, please make sure you provide the correct database connection string in _XtraUpload.WebApp/appsettings.json_
1. Run the application: the .NET runtime came with kestrel webserver, you can start  this http server by running the command`dotnet XtraUpload.WebApp.dll` navigate to localhost:5000/login you should get the following [login page](https://photos.app.goo.gl/TLerv4DRMrUU9tgZ8)  
PS: I moved the publish folder to desktop you can put it wherever you like on your server, `/var/www/` for instance.
1. The default admin account credentials are:  
	email: admin@admin.com  
	password: admin01  
	Please check the application health in your [admin panel](https://photos.app.goo.gl/1tqTDfphSde14zSf8) 
	and fix any issue, generally you need to specify the upload directory and grant read/write permission to 	the filestore folder, settings can be found in the `Settings` tab.
