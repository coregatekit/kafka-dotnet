# Kafka with .Net 5 application

### An example for Kafka with .Net 5 web api and MongoDB base on CQRS concept

```Note: Consumer side still have some bug```

## Requirement
- .Net 5 sdk
- Docker
- MongoDB

## How to start application

1. Start mongo instance and kafka server with docker-compose
```
docker-compose up -d
```
2. Restore .Net solution
```
dotnet restore
```
3. Open 2 terminal and change directory to each service and start application
```
cd ProducerService
dotnet run
cd ConsumerService
dotnet run
```

```Note: Producer service start with port 5001 and Consumer service port 5002```

## API Docs
Add new record
```
POST: http://localhost:5001/api/Books
[body]
  {
    "Name": "Chick Chick",
    "Price": 9.99,
    "Category": "General",
    "Author": "Chickie",
    "CreateBy": "coregate",
    "UpdateBy": "coregate"
  }

For update record
POST: http://localhost:5001/api/Books/{code}
[body]
{
    "Code": "89b391ec-95b8-4d4c-b83d-bed5833a20a7",
    "Name": "Chick Chick",
    "Price": 19.99,
    "Category": "General",
    "Author": "Chickie",
    "CreateBy": "Joe Kim",
    "UpdateBy": "Snoopy"
  }
```

Get record
```
Get all records
POST: http://localhost:5002/api/Books

Get single record
POST: http://localhost:5002/api/Books/{code}
```