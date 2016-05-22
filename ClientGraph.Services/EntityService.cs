using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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

        public async Task<IList<T>> GetAllAsync()
        {
            IList<T> entities = new List<T>();

            ScanRequest scanRequest = new ScanRequest(TableName);

            ScanResponse response = await _dynamoDbClient.ScanAsync(scanRequest).ConfigureAwait(false);

            foreach (var item in response.Items)
            {
                AttributeValue entityIdAttributeValue = item[PrimaryKey];

                string entityId = entityIdAttributeValue.S;

                T entity = await LoadEntityFromS3Async(entityId).ConfigureAwait(false);

                if (entity != null)
                {
                    entities.Add(entity);
                }
            }

            return entities;
        }

        public async Task<T> GetByIdAsync(Guid entityId)
        {
            return await LoadEntityFromS3Async(entityId.ToString()).ConfigureAwait(false);
        }

        public async Task<bool> SaveAsync(T entity)
        {
            bool isSaved = false;

            try
            {
                SaveEntityToS3(entity);
                await SaveEntityToDynamoDBAsync(entity).ConfigureAwait(false);

                isSaved = true;
            }
            catch
            {
                // Logging here
            }

            return isSaved;
        }

        public async Task<bool> DeleteAsync(Guid entityId)
        {
            bool isDeleted = false;

            try
            {
                await DeleteEntityFromS3Async(entityId).ConfigureAwait(false);
                await DeleteEntityFromDynamoDBAsync(entityId).ConfigureAwait(false);

                isDeleted = true;
            }
            catch
            {
                // Logging here
            }

            return isDeleted;
        }

        private async Task<T> LoadEntityFromS3Async(string entityId)
        {
            GetObjectRequest getObjectRequest = new GetObjectRequest
            {
                BucketName = BucketName,
                Key = entityId
            };

            string entityJson;

            using (GetObjectResponse response = await _s3Client.GetObjectAsync(getObjectRequest).ConfigureAwait(false))
            using (Stream responseStream = response.ResponseStream)
            using (StreamReader reader = new StreamReader(responseStream))
            {
                entityJson = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<T>(entityJson);
        }

        private async Task SaveEntityToDynamoDBAsync(T entity)
        {
            Table clientGraphTable = Table.LoadTable(_dynamoDbClient, TableName);

            Document document = new Document();
            document[PrimaryKey] = entity.Id.ToString();
            document["description"] = entity.Name;

            await clientGraphTable.PutItemAsync(document).ConfigureAwait(false);
        }

        private void SaveEntityToS3(T entity)
        {
            string clientJson = JsonConvert.SerializeObject(entity);

            PutObjectRequest putObjectRequest = new PutObjectRequest { BucketName = BucketName, Key = entity.Id.ToString(), ContentBody = clientJson };

            _s3Client.PutObject(putObjectRequest);
        }

        private async Task DeleteEntityFromS3Async(Guid entityId)
        {
            DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest { BucketName = BucketName, Key = entityId.ToString() };

            await _s3Client.DeleteObjectAsync(deleteObjectRequest).ConfigureAwait(false);
        }

        private async Task DeleteEntityFromDynamoDBAsync(Guid entityId)
        {
            DeleteItemRequest deleteItemRequest = new DeleteItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue> { { PrimaryKey, new AttributeValue { S = entityId.ToString() } } }
            };

            await _dynamoDbClient.DeleteItemAsync(deleteItemRequest).ConfigureAwait(false);
        }
    }
}