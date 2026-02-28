using 'main.bicep'

param name = 'billyrun-prod'
param location = 'eastus2'
param sku = 'Standard'
param repositoryUrl = 'https://github.com/danarkun/billyrun'
param branch = 'master'
param customDomainName = 'billyrun.com' // Placeholder for custom domain
