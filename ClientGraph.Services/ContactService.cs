﻿using ClientGraph.Domain;

namespace ClientGraph.Services
{
    public class ContactService : EntityService<Contact>
    {
        private const string PrimaryKey = "contactId";
        private const string TableName = "contacts-graph-table";
        private const string BucketName = "contacts";

        public ContactService() :
            base(PrimaryKey, TableName, BucketName)
        {
        }
    }
}