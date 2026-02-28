targetScope = 'subscription'

param environmentName string
param location string = 'eastus2'
param sku string = 'Free'
param repositoryUrl string
param branch string
param customDomainName string = ''

var resourceGroupName = 'rg-${environmentName}'

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupName
  location: location
}

module swa './swa.bicep' = {
  name: 'swa-deployment'
  scope: rg
  params: {
    name: 'swa-${environmentName}'
    location: location
    sku: sku
    repositoryUrl: repositoryUrl
    branch: branch
    customDomainName: customDomainName
  }
}

output staticWebAppId string = swa.outputs.staticWebAppId
output defaultHostname string = swa.outputs.defaultHostname
output deploymentToken string = swa.outputs.deploymentToken
@secure()
output secureDeploymentToken string = swa.outputs.deploymentToken
