# NET5-EventDriven-Kafka
An event driven approach for microservices sending information to Kafka and a hosted service consuming it


To execute, first run the docker compose.
docker-compose up

This will build the necessary environment ( Kafka, Mongo, Zookeeper, kafdrop ).

You can access the Kafka topics using Kafdrop at http://localhost:19000

The project is split into 2 applications.

1- API => This one executes a web api to simulate a microservice, which on the post ( add ), will send a message to Kafka, on the search, it will read MongoDB


2- Consumer => This is a hosted service that will read a Kafka topic, and will save the stored message into the MongoDb ( can be searched with the web api )
