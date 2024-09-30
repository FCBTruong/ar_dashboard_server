# Museum Data Management Backend Project

## Overview
This project was developed as my university graduation project. It is a backend system designed to manage and organize data for museums, such as artifact information, exhibits, and collections. The project provides a robust set of APIs that allow mobile and web applications to retrieve, update, and manage museum data efficiently. 

The project is built using **.NET Core** technologies and is deployed on **Azure Cloud**, leveraging Azure's **microservice architecture** for scalability and reliability.

## Key Features
- **API for Museum Data Management**: Exposes RESTful APIs to manage artifacts, exhibits, and other museum-related data.
- **Microservice Architecture**: Designed with a microservice architecture to enable scalability and modularity.
- **Cloud Deployment**: Fully hosted and deployed on **Azure Cloud**, utilizing services such as Azure App Services, Azure Functions, and Azure SQL.
- **Cross-platform**: The APIs can be consumed by both mobile and web applications.
  
## Technologies Used
- **.NET Core**: Backend framework for building APIs.
- **Entity Framework Core**: For managing data and database interactions.
- **Azure Cloud**: 
  - **Azure App Services**: Hosting the backend services.
  - **Azure Functions**: For lightweight, scalable event-driven processes.
  - **Azure SQL Database**: For storing and managing the museum data.
  - **Azure API Management**: To manage and secure the APIs.
- **Microservices**: Utilizing Azure's microservice architecture for flexible scaling and efficient resource management.

## Architecture
- **Microservices**: Each service is designed to handle a specific piece of functionality (e.g., Artifact Service, Exhibit Service).
- **API Gateway**: An API Gateway is used to route requests to the appropriate services.
- **Database**: Data is stored in an Azure SQL Database, managed using Entity Framework.

## API Endpoints
- **Artifacts**
  - `GET /api/artifacts` - Retrieve a list of all artifacts.
  - `GET /api/artifacts/{id}` - Retrieve details of a specific artifact.
  - `POST /api/artifacts` - Add a new artifact.
  - `PUT /api/artifacts/{id}` - Update an artifact.
  - `DELETE /api/artifacts/{id}` - Delete an artifact.
  
- **Exhibits**
  - `GET /api/exhibits` - Retrieve a list of all exhibits.
  - `GET /api/exhibits/{id}` - Retrieve details of a specific exhibit.
  - `POST /api/exhibits` - Add a new exhibit.
  - `PUT /api/exhibits/{id}` - Update an exhibit.
  - `DELETE /api/exhibits/{id}` - Delete an exhibit.

## Deployment
The project is deployed using Azure's cloud services:
1. **Build and Deployment**: Hosted on Azure App Services with CI/CD pipelines integrated via Azure DevOps.
2. **Microservices**: Each microservice is deployed as an independent service, managed via Azure's Service Fabric or Kubernetes.
3. **Database**: Managed with Azure SQL for data persistence.

## Setup Instructions
To run the project locally, follow these steps:
1. Clone the repository:
   ```bash
   git clone https://github.com/FCBTruong/ar_dashboard_server.git
   cd museum-backend
