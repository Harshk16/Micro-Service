terraform {
  required_version = ">= 1.8.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
    }
  }
}

provider "azurerm" {
  features {}
  subscription_id = var.azure_subscription_id
}

resource "azurerm_resource_group" "platform" {
  name     = var.resource_group_name
  location = var.location

  tags = {
    project     = "microservie-rabbitmq"
    environment = var.environment
  }
}
