variable "location" {
  type = string
}

variable "resource_group_name" {
  type = string
}

variable "storage_account_name" {
  type = string
}

variable "service_plan_name" {
  type = string
}

variable "database_name" {
  type = string
}

variable "functionApp_name" {
  type = string
}

variable "client_secret" {
  type      = string
  sensitive = true
}

variable "app_insights_name" {
  type = string
}