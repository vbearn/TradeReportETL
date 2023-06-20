# Trade Report ETL

A .NET application to import (large) Trade Report CSV files -containing trade transactions information- into an ETL pipeline, enriching the transactions using several Apis, and then loading it in a datastore for export. 

## Getting Started

Make sure you have [installed](https://docs.docker.com/docker-for-windows/install/) docker in your environment. Clone the repository in your local machine, open up your favorite terminal and then execute the below commands on the root repo directory:

```powershell
cd src
docker-compose build
docker-compose up
```

Wait for a few moments for all the services to come up, and then start using the provided Api Endpoints to import and export CSV datasets:

### 1. Importing a CSV dataset:

```
curl --location 'http://localhost:45000/api/DatasetImport' \
--form 'f=@"/path/to/your/input_dataset.csv"'
```

The Api returns a path that can be used to check on the progress of the import:
`/DatasetExport/ProgressPercentage?tradeReportId=0ab677be-bbf9-4980-abb8-0daa15f87287`

Note that the import will be executed asynchronously, and it might take a while before finishing, depending on the dataset size.

### 2. Check on the import progress:

```
curl --location 'http://localhost:45000/api/DatasetExport/ProgressPercentage?tradeReportId=0ab677be-bbf9-4980-abb8-0daa15f87287'
```

The Api returns the completion percentage of the import process:
`75%`


### 3. Download the enriched report containing all the transactions:


```
curl --location 'http://localhost:45000/api/DatasetExport/DownloadEnrichedReport?tradeReportId=0ab677be-bbf9-4980-abb8-0daa15f87287'
```
The Api returns the enroched dataset in Json format. Other export formats could easily be added via extending `TradeReportLoadService` class.


## Architecture overview


This ETL pipeline is made with product environment in mind, capable of handling large loads of data imports. It is highly scalable and based on a microservice oriented architecture. Each step of the pipeline (Extract, Transform, Load) can be scaled out as needed, depending on the load levels of production environment.

All asyncronous connections between services follow microservice-design best practices, including resiliency through retries, cachings, circuit breakers, and using message brokers. 

![](https://raw.githubusercontent.com/vbearn/TradeReportETL/master/images/architecture.jpg)


### ETL Pipeline

The application is based on Batch-Stream data pipeline architecture.

It moves the data through the Extract (parsing CSV inputs), Transform (enriching transactions with GLEIF and TransactionCost Api), and Load (uploading the transactions to data store, ready for export) stages, while batching the transactions to transform them through the third-party GLEIF api without hitting its DDoS limitations.

## Troubleshooting

- If there is any error happening while running the docker compose, please make sure that all the ports used in the file `docker-compose.yml` are free on your system, or alternatively, feel free to change the ports as needed.

- Troubleshooting, monitoring, and tracing the ETL pipeline can be done through the Centralized Seq Log portal, accesible at ![http://localhost:45100](http://localhost:45100)

![](https://raw.githubusercontent.com/vbearn/TradeReportETL/master/images/seq.jpg)

## Deployment

Deployment to the production environment can be done through Kubernetes (+Helm), as well as through the usual Cloud-based Container services (Azure Containers, AWS EC2, ...). 

For the TransactionCost Calculator service, deployment through a Serverless platform (Azure Functions, AWS Lambda) is best recommended.