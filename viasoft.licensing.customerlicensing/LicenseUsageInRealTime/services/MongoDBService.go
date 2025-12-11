package services

import (
	"context"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
	"log"
	"os"
)

func UsingCollection(databaseName string, collectionName string, action func(*mongo.Collection)) {
	client, collection := getCollectionConnection(databaseName, collectionName)
	defer disconnect(client)
	action(collection)
}

func disconnect(client *mongo.Client) {
	err := client.Disconnect(context.TODO())
	if err != nil {
		log.Fatal(err)
	}
}

func getCollectionConnection(databaseName string, collectionName string) (*mongo.Client, *mongo.Collection) {
	clientOptions := options.Client().ApplyURI("mongodb://"+os.Getenv("MONGODB_IP")+":"+os.Getenv("MONGODB_PORT"))

	var client, err = mongo.Connect(context.TODO(), clientOptions)
	if err != nil {
		log.Fatal(err)
	}

	err = client.Ping(context.TODO(), nil)
	if err != nil {
		log.Fatal(err)
	}

	collection := client.Database(databaseName).Collection(collectionName)
	return client, collection
}
