# Terraform infrastructure

This directory is the Azure cloud-infrastructure boundary for the project. The
starter configuration creates a tagged resource group; service hosting,
PostgreSQL, RabbitMQ, networking, and secret-management resources should be
added here as those deployment decisions are finalized.

Secrets and Terraform state must be supplied by the deployment environment and
must not be committed to the repository. Provide
`azure_subscription_id` through a secure variable source.
