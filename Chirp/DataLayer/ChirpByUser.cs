using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chirp.DataLayer
{
    public class ChirpByUser : TableEntity
    {
        public ChirpByUser(Chirp.Models.Message message)
        {
            PartitionKey = message.User;
            RowKey = Guid.NewGuid().ToString();
            Text = message.Text;
        }

        public ChirpByUser() { }

        public string Text { get; set; }
    }
}