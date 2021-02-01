# Varsity Project with Authentication Example
This example demonstrates how to use the base Individual User Accounts template code in an MVC project to add login functionality.

## To Run This Project
- Clone Codebase
- **Right Click Project > View Project in File Explorer > Create Folder "App_Data"**
- Tools > Nuget Package Manager > Package Manager Console
- enable-migrations
- add-migration {migration_name}
- update-database

![Schema of Individual User Accounts](https://github.com/christinebittle/varsity_w_auth/blob/master/varsity_w_auth/assets/varsity_with_auth_schema.png)
*The base schema of individual user accounts*

![Created Profile](https://github.com/christinebittle/varsity_w_auth/blob/master/varsity_w_auth/assets/varsity_account.png)
*When registering, you can see account information and hashed password field here*

![Local Cookies](https://github.com/christinebittle/varsity_w_auth/blob/master/varsity_w_auth/assets/local_cookies.png)
*After logging in, in local storage, you can see token cookies. These are later used to authenticate requests (without having to log in for each action).*

![Detailed Cookies](https://github.com/christinebittle/varsity_w_auth/blob/master/varsity_w_auth/assets/cookie_requestauth.png)
*Detailed look at the cookies in developer tools.*

![Curl Cookies](https://github.com/christinebittle/varsity_w_auth/blob/master/varsity_w_auth/assets/curl_cookie.png)
*An example of using the cookie as part of a curl request. Notice how the initial request fails, but the second request (with cookie data token) allows us to access the data. This cookie should only work for the domain I'm using it on (localhost), otherwise the app is vulnerable to [Session hijacking attacks](https://en.wikipedia.org/wiki/Session_hijacking).*
