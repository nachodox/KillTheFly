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
