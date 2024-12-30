resource "azurerm_resource_group" "example" {
  name     = "automated-testing"
  location = "North Central US"
}

resource "azurerm_cosmosdb_account" "account" {
  name                = "automated-testing"
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

module "database" {
  source = "./modules/database"

  account_name        = azurerm_cosmosdb_account.account.name
  resource_group_name = azurerm_resource_group.example.name
  database_name       = var.database_name 
}
