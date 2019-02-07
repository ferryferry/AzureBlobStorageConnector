# Azure blob storage connector
This simple library can be used to read from and write to the Azure Blob Storage.
It's written in .NET Standard and can be used in .NET Core and .NET Framework projects.

## How to get started?
To use the class library, import it in your own project.

There is also an example project which shows how it works.
To get this running, replace the connection string from the Azure Portal in this line of code: `const string connectionStringAzureStorageAccount = "YOUR-CONNECTIONSTRING-HERE";`
You can get the connection string by navigating to "Storage Accounts" followed by clicking on your storage account. Then click on Access keys and copy the whole connection string under Key1.

You have to make sure you have a blob container named `mediafiles`. The example makes use of this container.

## Dependencies
This library depends on:
- [WindowsAzure.Storage](https://www.nuget.org/packages/WindowsAzure.Storage/)