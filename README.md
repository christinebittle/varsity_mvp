# Varsity Project MVP
This example demonstrates how to create the MVP functionality for Create, Read, Update, and Delete.

![Listing Players](https://github.com/christinebittle/varsity_mvp/blob/master/varsity_w_auth/assets/listplayers.png)

## To Run This Project
- Clone Codebase
- **Right Click Project > View Project in File Explorer > Create Folder "App_Data"**
- Tools > Nuget Package Manager > Package Manager Console
- enable-migrations
- add-migration {migration_name}
- update-database

## Common Issues
# Unable to access part of path ... roslyn/csc.exe
- Right click solution > Clean
- Right click solution > Build
- Right click solution > Rebuild

# Update Database failed. Could not create .mdf file
- Create "App_Data" Folder in project

# The name or namespace 'Script'/'JavaScriptSerializer' could not be found
- Right click "References"
- Add System.Web.Extensions
