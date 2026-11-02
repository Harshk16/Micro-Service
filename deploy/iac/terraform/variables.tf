variable "azure_subscription_id" {
  description = "Azure subscription used for deployment."
  type        = string
  sensitive   = true
}

variable "resource_group_name" {
  description = "Name of the shared deployment resource group."
  type        = string
  default     = "rg-microservie-rabbitmq"
}

variable "location" {
  description = "Azure region for shared deployment resources."
  type        = string
  default     = "centralindia"
}

variable "environment" {
  description = "Deployment environment name."
  type        = string
  default     = "dev"
}
