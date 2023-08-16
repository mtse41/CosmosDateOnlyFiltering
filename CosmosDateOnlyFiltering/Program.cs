// See https://aka.ms/new-console-template for more information
using CosmosDateOnlyFiltering.Models;
using Microsoft.Azure.Cosmos;

Console.WriteLine("Hello, World!");

const string CONNECTION_STRING = "";
const string DATABASE_NAME = "TestDatabase";
const string CONTAINER_NAME = "Tickets";

// create database
var cosmosClient = new CosmosClient(CONNECTION_STRING);
var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(DATABASE_NAME);

if (databaseResponse == null)
{
    Console.WriteLine("databaseResponse is null");
    Console.ReadLine();
}

// create container
var containerResponse = await databaseResponse!.Database.CreateContainerIfNotExistsAsync(CONTAINER_NAME, "/id");

if (containerResponse == null)
{
    Console.WriteLine("containerResponse is null");
    Console.ReadLine();
}

// create ticket record
var container = containerResponse!.Container;

var newTicket = new Ticket
{
    CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
    Id = Guid.NewGuid().ToString()
};

var createResponse = await container.CreateItemAsync(newTicket, new PartitionKey(newTicket.Id));

if (createResponse == null || createResponse.StatusCode != System.Net.HttpStatusCode.Created)
{
    Console.WriteLine("create failed");
    Console.ReadLine();
}

// query ticket
var queryRequestOptions = new QueryRequestOptions
{
    PartitionKey = new PartitionKey(newTicket.Id.ToString())
};

var queryable = container.GetItemLinqQueryable<Ticket>(
    allowSynchronousQueryExecution: true,
    linqSerializerOptions: new CosmosLinqSerializerOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase },
    requestOptions: queryRequestOptions
);

// Day/Month/Year is 0
var result = queryable
    .Select(x => new
    {
        x.Id,
        x.CreatedAt.Day,
        x.CreatedAt.Month,
        x.CreatedAt.Year
    })
    .ToList();

var a = 0;