

resource "azurerm_postgresql_flexible_server" "ktf-psql-server" {
  name                   = "ktf-psql-server"
  resource_group_name    = azurerm_resource_group.ktf-rg.name
  location               = azurerm_resource_group.ktf-rg.location
  version                = "12"
  administrator_login    = var.ktf_db_username
  administrator_password = var.ktf_db_password
  storage_mb             = 32768
  sku_name               = "B_Standard_B1ms"
  zone                   = "1"
}

resource "azurerm_postgresql_flexible_server_database" "ktf-psql-database" {
  name        = "ktf-db"
  server_id   = azurerm_postgresql_flexible_server.ktf-psql-server.id
  collation   = "en_US.utf8"
  charset     = "utf8"
}

resource "azurerm_postgresql_flexible_server_firewall_rule" "ktf-db-fw-allow-all" {
  name                = "allow-all"
  server_id           = azurerm_postgresql_flexible_server.ktf-psql-server.id
  start_ip_address    = "0.0.0.0"
  end_ip_address      = "255.255.255.255"
}

resource "azurerm_cosmosdb_account" "ktf_cosmosdb_server" {
  name                = "ktf-cosmosdb-server"
  resource_group_name = azurerm_resource_group.ktf-rg.name
  location            = azurerm_resource_group.ktf-rg.location
  offer_type          = "Standard"
  kind                = "GlobalDocumentDB"
  
  geo_location {
    location          = azurerm_resource_group.ktf-rg.location
    failover_priority = 0
  }
  
  consistency_policy {
    consistency_level = "BoundedStaleness"
    max_interval_in_seconds = 300
    max_staleness_prefix = 1000
  }
}

output "cosmosdb_server_endpoint" {
  value = azurerm_cosmosdb_account.ktf_cosmosdb_server.endpoint
}