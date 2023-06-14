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

output "public_ip_address" {
  value = "${azurerm_linux_virtual_machine.ktf-vm.name}: ${data.azurerm_public_ip.ktf-ip-data.ip_address}"
}
