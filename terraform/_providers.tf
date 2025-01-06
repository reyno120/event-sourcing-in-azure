terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.0"
    }
  }
  backend "azurerm" {
    resource_group_name  = "automated-testing"
    storage_account_name = "jmreynolds03storage"
    container_name       = "terraform-state"
    key                  = "terraform.tfstate"
  }
}

provider "azurerm" {
  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }
  }

  client_id       = "bca89221-7bc9-4c94-91fb-f2a096003ff2"
  client_secret   = var.client_secret
  tenant_id       = "d2fc10a4-9803-4231-bdd0-9c75bfbde13b"
  subscription_id = "0b6585a7-6c0e-4fe5-b3f2-a2e699a315e6"
}