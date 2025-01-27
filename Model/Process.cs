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
    internal class Process : ITableEntity
    {
        [Key]
        public int Process_Id { get; set; } // Ensure property exists

        //ITableEntity implementation
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }

        [Required(ErrorMessage = "Please select a user.")]
        public int User_ID { get; set; }

        [Required(ErrorMessage = "Please select a product.")]
        public int Product_ID { get; set; }

        [Required(ErrorMessage = "Please select a Date.")]
        public DateTime Process_Date { get; set; }

        [Required(ErrorMessage = "Please enter a location.")]
        public string? Process_Location { get; set; }


    }
}
