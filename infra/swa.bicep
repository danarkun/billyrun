param name string
param location string = 'eastus2'
param sku string = 'Free'
param repositoryUrl string
param branch string
param customDomainName string = ''

resource swa 'Microsoft.Web/staticSites@2022-03-01' = {
  name: name
  location: location
  sku: {
    name: sku
    tier: sku
  }
  properties: {
    repositoryUrl: repositoryUrl
    branch: branch
    provider: 'GitHub'
    stagingEnvironmentPolicy: 'Enabled'
    allowConfigFileUpdates: true
  }
}

// Custom domain configuration
resource customDomain 'Microsoft.Web/staticSites/customDomains@2022-03-01' = if (!empty(customDomainName)) {
  parent: swa
  name: customDomainName
  properties: {}
}

output staticWebAppId string = swa.id
output defaultHostname string = swa.properties.defaultHostname
output deploymentToken string = swa.listSecrets().properties.apiKey
