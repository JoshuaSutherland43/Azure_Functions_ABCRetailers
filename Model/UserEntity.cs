using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDV_Functions.Model
{
    // User Model to help fetch and store user data.
    public class UserEntity : ITableEntity
    {
        public int User_Id { get; set; }
        public string? User_Name { get; set; } 
        public string? Email { get; set; }
        public string? Password { get; set; }

        public string PartitionKey { get; set; } = string.Empty;
        public string RowKey { get; set; } = string.Empty;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
