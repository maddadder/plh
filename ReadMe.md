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
# on client
```
npm run buildcss
docker-compose build
docker push neon-registry.4e88-13d3-b83a-9fc9.neoncluster.io/leenet/plhhoa:1.0.130
helm upgrade plhhoa ./plhhoa --namespace leenet

```