terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.0"
    }
  }
    backend "azurerm" {
      resource_group_name = "automated-testing"
      storage_account_name = "jmreynolds03storage"
      container_name = "terraform-state"
      key = "terraform.tfstate"
    }
}

provider "azurerm" {
  features {}
}