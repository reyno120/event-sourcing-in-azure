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

      client_id = "655ff4c2-7491-4ef3-847b-5cc89f032dc7"
      client_secret = var.client_secret
      tenant_id = "d2fc10a4-9803-4231-bdd0-9c75bfbde13b"
      subscription_id = "0b6585a7-6c0e-4fe5-b3f2-a2e699a315e6"
    }
}

provider "azurerm" {
  features {}
  
  client_id = "bca89221-7bc9-4c94-91fb-f2a096003ff2"
  client_secret = var.client_secret
  tenant_id = "d2fc10a4-9803-4231-bdd0-9c75bfbde13b"
  subscription_id = "0b6585a7-6c0e-4fe5-b3f2-a2e699a315e6"
}