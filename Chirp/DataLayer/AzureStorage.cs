using Chirp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace Chirp.DataLayer
{
    public class AzureStorage
    {
        #region sample data (used before addition of Azure code)
        private static Message[] sampleData = new Message[]
        {
            new Message() { User = "cicero", Text = "Lorem ipsum dolor sit amet",Timestamp = new DateTime(2014,1,1,13,50,0) },
            new Message() { User = "jimmy", Text = "Tiam pellentesque ligula purus #learnlatin",Timestamp = new DateTime(2014,1,3,18,15,0) },
            new Message() { User = "denise", Text = "Enean quis odio consequat, aliquet lacus ut",Timestamp = new DateTime(2014,1,5,21,34,0) },
            new Message() { User = "cicero", Text = "Integer sagittis ante id erat molestie volutpat #learnlatin #deadlanguage",Timestamp = new DateTime(2014,1,9,19,12,0) },
            new Message() { User = "jimmy", Text = "#latinrocks Enean quis odio consequat, aliquet lacus ut",Timestamp = new DateTime(2014,1,9,23,56,0) }
        };
        #endregion

        private CloudTableClient _tableClient;
        

        public AzureStorage()
        {
            var accountName = "devgroup2401";
            var accountKey = @"vXldTVd2UjMRYPcdgU70uXNw6SJnrlsNOlwHaZGzHPzTofhqSD9HyRF7mjOMERNdcwwCKlet7KqE/7HEPM0t5w==";
            var credentials = new StorageCredentials(accountName, accountKey);

            var storageAccount = new CloudStorageAccount(credentials, true);
            _tableClient = storageAccount.CreateCloudTableClient();
        }

        public void PostMessage(Message newMessage)
        {
            var chirpByUser = new ChirpByUser(newMessage);
            var insertByUserOperation = TableOperation.Insert(chirpByUser);

            var userTable = _tableClient.GetTableReference("chirpsByUser");
            userTable.CreateIfNotExists();
            if (userTable.Exists())
            {
                userTable.Execute(insertByUserOperation);
            }

            var tagTable = _tableClient.GetTableReference("chirpsByTag");
            tagTable.CreateIfNotExists();
            if(newMessage.Tags.Count>0 && tagTable.Exists())
            {
                foreach(string tag in newMessage.Tags)
                {
                    var chirpByTag = new ChirpByTag(newMessage, tag);
                    var insertByTagOperation = TableOperation.Insert(chirpByTag);
                    tagTable.Execute(insertByTagOperation);
                }
            }
        }

        public IEnumerable<Message> GetAllMessages()
        {
            var messages = new List<Message>();

            var table = _tableClient.GetTableReference("chirpsByUser");
            if (table.Exists())
            {
                DateTimeOffset lastMonth = new DateTimeOffset(DateTime.Today.AddMonths(-1));
                var tableQuery = new TableQuery<ChirpByUser>().
                    Where(TableQuery.GenerateFilterConditionForDate("Timestamp",
                    QueryComparisons.GreaterThanOrEqual, lastMonth));
                var chirps = table.ExecuteQuery(tableQuery);
                foreach(var c in chirps)
                {
                    messages.Add(new Message() { User = c.PartitionKey, Timestamp = c.Timestamp.DateTime, Text = c.Text });
                }
            }

            return messages.OrderByDescending(m => m.Timestamp);
        }

        public IEnumerable<Message> SearchMessagesByUser(string username)
        {
            var messages = new List<Message>();

            var table = _tableClient.GetTableReference("chirpsByUser");
            if (table.Exists())
            {
                var tableQuery = new TableQuery<ChirpByUser>().
                    Where(TableQuery.GenerateFilterCondition("PartitionKey",
                    QueryComparisons.GreaterThanOrEqual, username));
                var chirps = table.ExecuteQuery(tableQuery);
                foreach (var c in chirps)
                {
                    messages.Add(new Message() { User = c.PartitionKey, Timestamp = c.Timestamp.DateTime, Text = c.Text });
                }
            }

            return messages.OrderByDescending(m => m.Timestamp);
        }

        public IEnumerable<Message> SearchMessagesByTag(string tag)
        {
            var messages = new List<Message>();

            var table = _tableClient.GetTableReference("chirpsByTag");
            if (table.Exists())
            {
                var tableQuery = new TableQuery<ChirpByTag>().
                    Where(TableQuery.GenerateFilterCondition("PartitionKey",
                    QueryComparisons.GreaterThanOrEqual, tag));
                var chirps = table.ExecuteQuery(tableQuery);
                foreach (var c in chirps)
                {
                    messages.Add(new Message() { User = c.User, Timestamp = c.Timestamp.DateTime, Text = c.Text });
                }
            }

            return messages.OrderByDescending(m => m.Timestamp);
        }
    }
}