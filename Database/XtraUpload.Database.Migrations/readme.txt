Execute this steps if you want to run a migration:

1- Open PMC
2- Add-Migration MyMigrationName -Project XtraUpload.Database.Migrations -StartupProject XtraUpload.WebApi
3- check the generated files for any errors
4- run update-database if you want to publish to db