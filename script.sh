#!/bin/bash

# This script will clean up the current directory by removing common .NET project files
# and directories, then create a new .NET solution with the specified Clean Architecture projects.
# ** Run this script from the directory where your existing project files are located. **

echo "Cleaning up existing project files and directories in the current directory..."

# Remove common .NET build artifacts and project files based on the ls output
# Added more specific cleanup based on the nested structure observed
rm -rf bin/ obj/ Domain/ NeuroDustify.Domain/ NeuroDustify.Application/ NeuroDustify.Infrastructure/ NeuroDustify.WebApi/ services/ Properties/
rm -f *.csproj *.sln Program.cs *.http *.json LICENSE README.md "Smart Dustbin Management Backend Services.pdf"

echo "Cleanup complete."

# Define project names
SOLUTION_NAME="NeuroDustifyBackend"
DOMAIN_PROJECT="NeuroDustify.Domain"
APPLICATION_PROJECT="NeuroDustify.Application"
INFRASTRUCTURE_PROJECT="NeuroDustify.Infrastructure"
WEBAPI_PROJECT="NeuroDustify.WebApi"

# Create the new solution file in the current directory
echo "Creating new .NET solution: $SOLUTION_NAME.sln"
dotnet new sln -n $SOLUTION_NAME

# Create project directories and projects within those directories
echo "Creating project directories and projects..."

# Domain Project
echo "Creating Domain project: $DOMAIN_PROJECT"
mkdir $DOMAIN_PROJECT
# Create the class library project directly inside the $DOMAIN_PROJECT directory
dotnet new classlib -n $DOMAIN_PROJECT -o $DOMAIN_PROJECT
dotnet sln add $DOMAIN_PROJECT/$DOMAIN_PROJECT.csproj

# Application Project
echo "Creating Application project: $APPLICATION_PROJECT"
mkdir $APPLICATION_PROJECT
# Create the class library project directly inside the $APPLICATION_PROJECT directory
dotnet new classlib -n $APPLICATION_PROJECT -o $APPLICATION_PROJECT
dotnet sln add $APPLICATION_PROJECT/$APPLICATION_PROJECT.csproj

# Infrastructure Project
echo "Creating Infrastructure project: $INFRASTRUCTURE_PROJECT"
mkdir $INFRASTRUCTURE_PROJECT
# Create the class library project directly inside the $INFRASTRUCTURE_PROJECT directory
dotnet new classlib -n $INFRASTRUCTURE_PROJECT -o $INFRASTRUCTURE_PROJECT
dotnet sln add $INFRASTRUCTURE_PROJECT/$INFRASTRUCTURE_PROJECT.csproj

# Web API Project
echo "Creating Web API project: $WEBAPI_PROJECT"
mkdir $WEBAPI_PROJECT
# Create the webapi project directly inside the $WEBAPI_PROJECT directory
dotnet new webapi -n $WEBAPI_PROJECT -o $WEBAPI_PROJECT
dotnet sln add $WEBAPI_PROJECT/$WEBAPI_PROJECT.csproj

echo "Projects created and added to solution."

# Add project references based on Clean Architecture
echo "Adding project references..."

# Application references Domain
dotnet add $APPLICATION_PROJECT/$APPLICATION_PROJECT.csproj reference $DOMAIN_PROJECT/$DOMAIN_PROJECT.csproj

# Infrastructure references Application
dotnet add $INFRASTRUCTURE_PROJECT/$INFRASTRUCTURE_PROJECT.csproj reference $APPLICATION_PROJECT/$APPLICATION_PROJECT.csproj

# WebApi references Application
dotnet add $WEBAPI_PROJECT/$WEBAPI_PROJECT.csproj reference $APPLICATION_PROJECT/$APPLICATION_PROJECT.csproj

echo "Project references added."

echo "New NeuroDustify backend structure created successfully!"
echo "You can now navigate into the project directories and add your code."


