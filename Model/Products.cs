using Azure.Data.Tables;
using Azure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDV_Functions.Model
{
    public class Product : ITableEntity
    {
        [Key]

        public int? Product_Id { get; set; } // Ensure property exists
        public string? Product_Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? Price { get; set; }

        //ITableEntity implementation
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
