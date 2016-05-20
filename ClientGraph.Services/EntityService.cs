using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using Amazon.S3.Model;
using ClientGraph.Domain;
using ClientGraph.Services.Interfaces;
using Newtonsoft.Json;

namespace ClientGraph.Services
{
    public abstract class EntityService<T> : IEntityService<T>
        where T : EntityBase
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IAmazonDynamoDB _dynamoDbClient;

        private readonly string PrimaryKey;
        private readonly string TableName;
        private readonly string BucketName;

        private string BucketPrefix = "client-graph-data/";

        protected EntityService(string primaryKey, string tableName, string bucketName)
        {
            _s3Client = new AmazonS3Client();
            _dynamoDbClient = new AmazonDynamoDBClient();

            PrimaryKey = primaryKey;
            TableName = tableName;
            BucketName = BucketPrefix + bucketName;
        }

        public IList<T> GetAll()
        {
            ScanRequest scanRequest = new ScanRequest(TableName);

            ScanResponse response = _dynamoDbClient.Scan(scanRequest);

            return response.Items
                .Select(attributeValues => attributeValues[PrimaryKey])
                .Select(entityIdAttributeValue => entityIdAttributeValue.S)
                .Select(LoadEntityFromS3)
                .Where(e => e != null)
                .ToList();
        }

        public T GetById(Guid entityId)
        {
            return LoadEntityFromS3(entityId.ToString());
        }

        public bool Save(T entity)
        {
            bool isSaved = false;

            try
            {
                SaveEntityToS3(entity);
                SaveEntityToDynamoDB(entity);

                isSaved = true;
            }
            catch
            {
                // Logging here
            }

            return isSaved;
        }

        public bool Delete(Guid entityId)
        {
            bool isDeleted = false;

            try
            {
                DeleteEntityFromS3(entityId);
                DeleteEntityFromDynamoDB(entityId);

                isDeleted = true;
            }
            catch
            {
                // Logging here
            }

            return isDeleted;
        }

        private T LoadEntityFromS3(string contactId)
        {
            GetObjectRequest getObjectRequest = new GetObjectRequest
            {
                BucketName = BucketName,
                Key = contactId
            };

            string entityJson;

            using (GetObjectResponse response = _s3Client.GetObject(getObjectRequest))
            using (Stream responseStream = response.ResponseStream)
            using (StreamReader reader = new StreamReader(responseStream))
            {
                entityJson = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<T>(entityJson);
        }

        private void SaveEntityToDynamoDB(T entity)
        {
            Table clientGraphTable = Table.LoadTable(_dynamoDbClient, TableName);

            Document document = new Document();
            document[PrimaryKey] = entity.Id.ToString();
            document["description"] = entity.GetDescription();

            clientGraphTable.PutItem(document);
        }

        private void SaveEntityToS3(T entity)
        {
            string clientJson = JsonConvert.SerializeObject(entity);

            PutObjectRequest putObjectRequest = new PutObjectRequest { BucketName = BucketName, Key = entity.Id.ToString(), ContentBody = clientJson };

            _s3Client.PutObject(putObjectRequest);
        }

        private void DeleteEntityFromS3(Guid entityId)
        {
            DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest { BucketName = BucketName, Key = entityId.ToString() };

            _s3Client.DeleteObject(deleteObjectRequest);
        }

        private void DeleteEntityFromDynamoDB(Guid entityId)
        {
            DeleteItemRequest deleteItemRequest = new DeleteItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue> { { PrimaryKey, new AttributeValue { S = entityId.ToString() } } }
            };

            _dynamoDbClient.DeleteItem(deleteItemRequest);
        }
    }
}