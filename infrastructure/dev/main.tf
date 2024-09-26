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

output "public_ip_address" {
  value = "${azurerm_linux_virtual_machine.ktf-vm.name}: ${data.azurerm_public_ip.ktf-ip-data.ip_address}"
}
