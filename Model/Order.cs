using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDV_Functions.Model
{
    public class Order : ITableEntity

    {
        public int Order_ID { get; set; } // Ensure property exists

        //ITableEntity implementation
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }

        public int User_ID { get; set; }

        public int Product_ID { get; set; }
        public DateTime Date { get; set; }

        public string? Location { get; set; }
    }
}
