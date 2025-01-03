variable "functionApp_name" {}
variable "resource_group" {}
variable "storage_account" {}
variable "service_plan" {}
variable "database_name" {}
variable "cosmosdb_connectionstring" {}

resource "azurerm_windows_function_app" "functionApp" {
  name                = var.functionApp_name
  resource_group_name = var.resource_group.name
  location            = var.resource_group.location

  storage_account_name       = var.storage_account.name
  storage_account_access_key = var.storage_account.primary_access_key
  service_plan_id            = var.service_plan.id
  app_settings = {
    DatabaseName  = var.database_name
    ContainerName = "ToDoListEventStream"
    CosmosDBConnectionString = var.cosmosdb_connectionstring
  }
  connection_string {
    name  = "CosmosDBConnectionString"
    type  = "DocDb"
    value = var.cosmosdb_connectionstring
  }

  site_config {}
}