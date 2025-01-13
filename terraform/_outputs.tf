output "cosmosDB_connectionString" {
  value     = azurerm_cosmosdb_account.account.primary_sql_connection_string
  sensitive = true
}