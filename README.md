# SocialMediaAnalytics

1.	The code mainly contains four worker service instances and solution file is kept in SocialMediaAnalytics folder.
	DistributionService would be acting as distributor or producer service. This service would be getting all messages where service would check for incoming messages and distribute accordingly. This service would be receiving messages from all sources. In our code, we have hardcoded messages.

2.	Start Zookeeper and Kafka Servers using below commands:

	.\bin\windows\zookeeper-server-start.bat .\config\zookeeper.properties
	.\bin\windows\kafka-server-start.bat .\config\server.properties


3.	Topics would need to be created by using below commands:

	bash bin/kafka-topics.sh --create --topic message-distribution-posts --bootstrap-server localhost:9092
	bash bin/kafka-topics.sh --create --topic message-distribution-comments --bootstrap-server localhost:9092
	bash bin/kafka-topics.sh --create --topic message-distribution-popularity --bootstrap-server localhost:9092

4.	Modify solution file to configure all projects as start up projects.

5. 	All logs are printed on console.

Below pre-requisites are required to run solution:
1. jre1.8.0_421
2. kafka_2.12-3.8.0
3. .NET 6
4. Nugget package: Confluent.Kafka=2.5.3
