resource "azurerm_resource_group" "example" {
  name     = var.resource_group_name
  location = "Central US"
}

resource "azurerm_cosmosdb_account" "account" {
  name                = azurerm_resource_group.example.name
  resource_group_name = azurerm_resource_group.example.name
  offer_type          = "Standard"
  location            = "Central US"
  tags = {
    "defaultExperience" = "Core (SQL)"
  }
  depends_on = [
    azurerm_resource_group.example
  ]

  consistency_policy {
    consistency_level       = "Session"
    max_interval_in_seconds = 5
    max_staleness_prefix    = 100
  }

  geo_location {
    location          = "centralus"
    failover_priority = 0
    zone_redundant    = false
  }
}

resource "azurerm_storage_account" "storage_account" {
  name                     = "automatedtestinga47f"
  resource_group_name      = azurerm_resource_group.example.name
  location                 = azurerm_resource_group.example.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_service_plan" "service_plan" {
  name                = "ASP-automatedtesting-a68c"
  resource_group_name = azurerm_resource_group.example.name
  location            = "centralus"
  os_type             = "Windows"
  sku_name            = "Y1"
}

module "functionApp" {
  source = "./modules/functionapp"

  functionApp_name          = "jmreynolds03-functionApp"
  resource_group            = azurerm_resource_group.example
  storage_account           = azurerm_storage_account.storage_account
  service_plan              = azurerm_service_plan.service_plan
  database_name             = "fancy-db"
  cosmosdb_connectionstring = azurerm_cosmosdb_account.account.primary_sql_connection_string
}

module "database" {
  source = "./modules/database"

  account_name        = azurerm_cosmosdb_account.account.name
  resource_group_name = azurerm_resource_group.example.name
  database_name       = "fancy-db"
}
