resource "azurerm_resource_group" "example" {
  name     = var.resource_group_name
  location = var.location
}

resource "azurerm_cosmosdb_account" "account" {
  name                = azurerm_resource_group.example.name
  resource_group_name = azurerm_resource_group.example.name
  offer_type          = "Standard"
  location            = var.location
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
    location          = var.location
    failover_priority = 0
    zone_redundant    = false
  }
}

resource "azurerm_storage_account" "storage_account" {
  name                     = var.storage_account_name
  resource_group_name      = azurerm_resource_group.example.name
  location                 = azurerm_resource_group.example.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_service_plan" "service_plan" {
  name                = var.service_plan_name
  resource_group_name = azurerm_resource_group.example.name
  location            = var.location
  os_type             = "Windows"
  sku_name            = "Y1"
}

resource "azurerm_application_insights" "app_insights" {
  name                = var.app_insights_name
  location            = var.location
  resource_group_name = var.resource_group_name
  application_type    = "web"
  depends_on = [
    azurerm_resource_group.example
  ]
}

module "functionApp" {
  source = "./modules/functionapp"

  functionApp_name          = var.functionApp_name
  resource_group            = azurerm_resource_group.example
  storage_account           = azurerm_storage_account.storage_account
  service_plan              = azurerm_service_plan.service_plan
  database_name             = var.database_name
  cosmosdb_connectionstring = azurerm_cosmosdb_account.account.primary_sql_connection_string
  app_insights_key          = azurerm_application_insights.app_insights.instrumentation_key
}

module "database" {
  source = "./modules/database"

  account_name        = azurerm_cosmosdb_account.account.name
  resource_group_name = azurerm_resource_group.example.name
  database_name       = var.database_name
}
