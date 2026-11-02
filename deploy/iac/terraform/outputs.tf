output "resource_group_name" {
  description = "The shared deployment resource group name."
  value       = azurerm_resource_group.platform.name
}

output "resource_group_location" {
  description = "The shared deployment resource group location."
  value       = azurerm_resource_group.platform.location
}
