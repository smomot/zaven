# Ticket System
## Description

Ticket system is an application that provides separate api's for administrator and customer, to create events and make appointments to that events.


### Interface

By default, sysytem provides two different api's. 

[http://localhost:5007/]()

[http://localhost:5008/]()

In addition to this, it is possible to test those interfaces by swagger. 
 
[http://localhost:5007/swagger/index.html]()

[http://localhost:5008/swagger/index.html]()


#### Ordering Api

Ordering interface provides functionality for customer to make and manage an appoinment.


#### Catalog Api

Catalog interface provides functionality for administrator to manage eventn's catalog.  


### Storage

Tickest systems use MongoDb database. In development mode, the database servers starts in docker container. 
You can find and manage configuration in docker-compose file. 
The system hovever use classes that allow cooperate with Azure Cosmo DB after few changes. 

### Queuing mechanism

Ticket system contains two microservices. 
Making update by administrator in events catalog, should be visible in ordering service. 
Ordering service contains additional database, that recreates catalog from catalog service. 
This creation is done by comunication with messaging system base on Rabbit MQ container in development mode. 
The system has additional configuration for Azure Service Bus for production mode. 

You can find configuration for rabbit mq in docker-compose file.  
Communication, between services and queue mechanism is support by Dapr library. 


#### Dapr

Communication between ordering and catalog service is supported by Dapr libraries. 
Dapr aprovides simple implementation of different components by provide sideway services that already 
contain implementation. 

You can find dapr sideway components configuration in Services/Dapr catalog. 
In addtion to this docker compose contains main configuration, between services and their sideways and sideways to sideways. 

You can find more by read docekr-compose config, looking for elements : 
- catalogapi-dapr 
- ordering-dapr 

#### Configuration 

You can manipulate different settings, including connections strings to database in docker-compose for orderingapi and catalogapi 
elements. 

Queing system can be configured by making changes in Dapr catalog, where dapr components are described.


