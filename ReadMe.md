# Dependencies

dotnet add package Microsoft.Identity.Web
dotnet add package Microsoft.AspNetCore.Authentication.OpenIdConnect
dotnet add package Microsoft.Identity.Web.UI

# Docker
docker-compose build

# Testing
docker-compose up

# Deploy to microk8s
docker push 192.168.1.84:32000/plhhoa:1.0.12
microk8s helm3 install plhhoa ./plhhoa

# Deploy to daffy
docker push 192.168.1.151:32000/plhhoa:1.0.12
helm install plhhoa ./plhhoa
