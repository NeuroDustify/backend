#!/bin/bash

# This script adds the necessary project references between the NeuroDustify backend projects.
# ** Run this script from the directory containing the NeuroDustifyBackend.sln file. **

# Define project names
DOMAIN_PROJECT="NeuroDustify.Domain"
APPLICATION_PROJECT="NeuroDustify.Application"
INFRASTRUCTURE_PROJECT="NeuroDustify.Infrastructure"
WEBAPI_PROJECT="NeuroDustify.WebApi"

echo "Adding project references based on Clean Architecture..."

# Application references Domain
echo "Adding reference from $APPLICATION_PROJECT to $DOMAIN_PROJECT..."
dotnet add $APPLICATION_PROJECT/$APPLICATION_PROJECT.csproj reference $DOMAIN_PROJECT/$DOMAIN_PROJECT.csproj

# Infrastructure references Application
echo "Adding reference from $INFRASTRUCTURE_PROJECT to $APPLICATION_PROJECT..."
dotnet add $INFRASTRUCTURE_PROJECT/$INFRASTRUCTURE_PROJECT.csproj reference $APPLICATION_PROJECT/$APPLICATION_PROJECT.csproj

# WebApi references Application
echo "Adding reference from $WEBAPI_PROJECT to $APPLICATION_PROJECT..."
dotnet add $WEBAPI_PROJECT/$WEBAPI_PROJECT.csproj reference $APPLICATION_PROJECT/$APPLICATION_PROJECT.csproj

echo "Project references added successfully!"
echo "You can verify the references in the .csproj files."


