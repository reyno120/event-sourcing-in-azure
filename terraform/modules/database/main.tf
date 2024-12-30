variable "resource_group_name" {}
variable "account_name" {}
variable "database_name" {}

resource "azurerm_cosmosdb_sql_database" "database" {
  name                = var.database_name
  resource_group_name = var.resource_group_name
  account_name        = var.account_name 
}

resource "azurerm_cosmosdb_sql_container" "container" {
  name                  = "ToDoListEventStream"
  resource_group_name   = var.resource_group_name
  account_name          = var.account_name
  database_name         = azurerm_cosmosdb_sql_database.database.name
  partition_key_path    = "/streamId"
  partition_key_version = 1

  unique_key {
    paths = ["/version"]
  }
}