terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=3.0.0"
    }
  }
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "ktf-rg" {
  name     = "ktf-resources"
  location = "East Us"
  tags = {
    environment = "dev"
  }
}

resource "azurerm_virtual_network" "ktf-vn" {
  name                = "ktf-network"
  resource_group_name = azurerm_resource_group.ktf-rg.name
  location            = azurerm_resource_group.ktf-rg.location
  address_space       = ["10.123.0.0/16"]

  tags = {
    environment = "dev"
  }
}

resource "azurerm_subnet" "ktf-subnet" {
  name                 = "ktf-subnet"
  resource_group_name  = azurerm_resource_group.ktf-rg.name
  virtual_network_name = azurerm_virtual_network.ktf-vn.name
  address_prefixes     = ["10.123.1.0/24"]
}

resource "azurerm_network_security_group" "ktf-sg" {
  name                = "ktf-sg"
  location            = azurerm_resource_group.ktf-rg.location
  resource_group_name = azurerm_resource_group.ktf-rg.name

  tags = {
    environment = "dev"
  }
}

resource "azurerm_network_security_rule" "ktf-dev-rule" {
  name                        = "ktf-dev-rule"
  priority                    = 100
  direction                   = "Inbound"
  access                      = "Allow"
  protocol                    = "*"
  source_port_range           = "*"
  destination_port_range      = "*"
  source_address_prefix       = "*" # revisar bloquear por ip
  destination_address_prefix  = "*"
  resource_group_name         = azurerm_resource_group.ktf-rg.name
  network_security_group_name = azurerm_network_security_group.ktf-sg.name
}

resource "azurerm_subnet_network_security_group_association" "ktf-dev-sga" {
  subnet_id                 = azurerm_subnet.ktf-subnet.id
  network_security_group_id = azurerm_network_security_group.ktf-sg.id
}

resource "azurerm_public_ip" "ktf-ip" {
  name                = "ktf-ip"
  resource_group_name = azurerm_resource_group.ktf-rg.name
  location            = azurerm_resource_group.ktf-rg.location
  allocation_method   = "Dynamic"

  tags = {
    environment = "dev"
  }
}

resource "azurerm_network_interface" "ktf-nic" {
  name                = "ktf-nic"
  location            = azurerm_resource_group.ktf-rg.location
  resource_group_name = azurerm_resource_group.ktf-rg.name

  ip_configuration {
    name                          = "internal"
    subnet_id                     = azurerm_subnet.ktf-subnet.id
    private_ip_address_allocation = "Dynamic"
    public_ip_address_id          = azurerm_public_ip.ktf-ip.id
  }

  tags = {
    environment = "dev"
  }
}

resource "azurerm_linux_virtual_machine" "ktf-vm" {
  name                  = "ktf-vm"
  resource_group_name   = azurerm_resource_group.ktf-rg.name
  location              = azurerm_resource_group.ktf-rg.location
  size                  = "Standard_B1s"
  admin_username        = "adminuser"
  network_interface_ids = [azurerm_network_interface.ktf-nic.id]

  custom_data = filebase64("customdata.tpl")

  admin_ssh_key {
    username   = "adminuser"
    public_key = file("~/.ssh/ktf_dev.pub")
  }

  os_disk {
    caching              = "ReadWrite"
    storage_account_type = "Standard_LRS"
  }

  source_image_reference {
    publisher = "Canonical"
    offer     = "0001-com-ubuntu-server-jammy"
    sku       = "22_04-lts"
    version   = "latest"
  }

  provisioner "local-exec" {
    command = templatefile("${var.host_os}-ssh-script.tpl", {
      hostname     = self.public_ip_address,
      user         = "adminuser",
      identityfile = "~/.ssh/ktf_dev"
    })
    interpreter = var.host_os == "windows" ? ["Powershell", "-Command"] : ["bash", "-c"]
  }

  tags = {
    environment = "dev"
  }
}

data "azurerm_public_ip" "ktf-ip-data" {
  name                = azurerm_public_ip.ktf-ip.name
  resource_group_name = azurerm_resource_group.ktf-rg.name
}

resource "azurerm_dns_zone" "zone" {
  name = (
    "matamosca.cl"
  )
  resource_group_name = azurerm_resource_group.ktf-rg.name
}

resource "azurerm_dns_a_record" "record" {
  name                = "@"
  resource_group_name = azurerm_resource_group.ktf-rg.name
  zone_name           = azurerm_dns_zone.zone.name
  ttl                 = 3600
  records             = [azurerm_public_ip.ktf-ip.ip_address]
}

output "public_ip_address" {
  value = "${azurerm_linux_virtual_machine.ktf-vm.name}: ${data.azurerm_public_ip.ktf-ip-data.ip_address}"
}

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
