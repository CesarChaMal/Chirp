using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chirp.DataLayer
{
    public class ChirpByTag : TableEntity
    {
        public ChirpByTag(Chirp.Models.Message message, string tag)
        {
            PartitionKey = tag;
            RowKey = Guid.NewGuid().ToString();
            User = message.User;
            Text = message.Text;
        }

        public ChirpByTag() { }

        public string Text { get; set; }
        public string User { get; set; }
    }
}