using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private readonly string _primaryKey;
        private readonly string _tableName;
        private readonly string _bucketPath;
        private readonly string _folderName;

        private string BucketName = "client-graph-data/";

        protected EntityService(string primaryKey, string tableName, string folderName)
        {
            _s3Client = new AmazonS3Client();
            _dynamoDbClient = new AmazonDynamoDBClient();

            _primaryKey = primaryKey;
            _tableName = tableName;
            _folderName = folderName;
            _bucketPath = BucketName + folderName;
        }

        public async Task<IList<T>> GetAllAsync()
        {
            IList<T> entities = new List<T>();

            ScanRequest scanRequest = new ScanRequest(_tableName);

            ScanResponse response = await _dynamoDbClient.ScanAsync(scanRequest).ConfigureAwait(false);

            foreach (var item in response.Items)
            {
                AttributeValue entityIdAttributeValue = item[_primaryKey];

                Guid entityId = new Guid(entityIdAttributeValue.S);

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
            T entity = await LoadEntityFromS3Async(entityId).ConfigureAwait(false);

            if (entity != null)
            {
                entity.Versions = await LoadEntityVersionsFromS3Async(entityId).ConfigureAwait(false);
            }

            return entity;
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

        public async Task RestoreVersionAsync(Guid entityId, string versionId)
        {
            GetObjectRequest getObjectRequest = new GetObjectRequest
            {
                BucketName = _bucketPath,
                Key = entityId.ToString(),
                VersionId = versionId
            };

            string entityJson;

            using (GetObjectResponse response = await _s3Client.GetObjectAsync(getObjectRequest).ConfigureAwait(false))
            using (Stream responseStream = response.ResponseStream)
            using (StreamReader reader = new StreamReader(responseStream))
            {
                entityJson = reader.ReadToEnd();
            }

            PutObjectRequest putObjectRequest = new PutObjectRequest { BucketName = _bucketPath, Key = entityId.ToString(), ContentBody = entityJson };

            _s3Client.PutObject(putObjectRequest);
        }

        private async Task<T> LoadEntityFromS3Async(Guid entityId)
        {
            GetObjectRequest getObjectRequest = new GetObjectRequest
            {
                BucketName = _bucketPath,
                Key = entityId.ToString()
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

        private async Task<IList<EntityVersion>> LoadEntityVersionsFromS3Async(Guid entityId)
        {
            IList<EntityVersion> entityVersions = new List<EntityVersion>();

            ListVersionsRequest listVersionsRequest = new ListVersionsRequest
            {
                BucketName = BucketName,
                Prefix = _folderName + "/" + entityId,
            };

            ListVersionsResponse response = await _s3Client.ListVersionsAsync(listVersionsRequest).ConfigureAwait(false);

            int versionNumber = 1;

            foreach (S3ObjectVersion s3ObjectVersion in response.Versions.OrderBy(v => v.LastModified))
            {
                EntityVersion entityVersion = new EntityVersion
                {
                    VersionId = s3ObjectVersion.VersionId,
                    VersionNumber = versionNumber,
                    LastModified = s3ObjectVersion.LastModified,
                    Size = s3ObjectVersion.Size
                };

                versionNumber++;

                entityVersions.Add(entityVersion);
            }

            return entityVersions;
        }

        private async Task SaveEntityToDynamoDBAsync(T entity)
        {
            Table clientGraphTable = Table.LoadTable(_dynamoDbClient, _tableName);

            Document document = new Document();
            document[_primaryKey] = entity.Id.ToString();
            document["description"] = entity.Name;

            await clientGraphTable.PutItemAsync(document).ConfigureAwait(false);
        }

        private void SaveEntityToS3(T entity)
        {
            string entityJson = JsonConvert.SerializeObject(entity);

            PutObjectRequest putObjectRequest = new PutObjectRequest { BucketName = _bucketPath, Key = entity.Id.ToString(), ContentBody = entityJson };

            _s3Client.PutObject(putObjectRequest);
        }

        private async Task DeleteEntityFromS3Async(Guid entityId)
        {
            DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest { BucketName = _bucketPath, Key = entityId.ToString() };

            await _s3Client.DeleteObjectAsync(deleteObjectRequest).ConfigureAwait(false);
        }

        private async Task DeleteEntityFromDynamoDBAsync(Guid entityId)
        {
            DeleteItemRequest deleteItemRequest = new DeleteItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue> { { _primaryKey, new AttributeValue { S = entityId.ToString() } } }
            };

            await _dynamoDbClient.DeleteItemAsync(deleteItemRequest).ConfigureAwait(false);
        }
    }
}