# Dependencies
```
dotnet add package Microsoft.Identity.Web
dotnet add package Microsoft.AspNetCore.Authentication.OpenIdConnect
dotnet add package Microsoft.Identity.Web.UI
```

# Docker
```
rename appsettings.example.json to appsettings.Development.json

For role based security you need to: https://learn.microsoft.com/en-us/azure/active-directory/develop/howto-add-app-roles-in-azure-ad-apps#assign-users-and-groups-to-roles
Configure Email fowarding: https://cumulusconstructor.com/nearly-free-full-domain-email-forwarding-with-aws/
docker-compose build
```
# Testing
```
docker-compose up

Navigate to http://localhost:5001
```
# Done Testing?
```
docker-compose down
```
# Deploy to microk8s
```

docker push 192.168.1.84:32000/plhhoa:1.0.120
microk8s helm3 install plhhoa ./plhhoa
```
# on client
```
npm run buildcss
docker-compose build
docker push 192.168.1.151:32000/plhhoa:1.0.120
helm upgrade plhhoa ./plhhoa

```