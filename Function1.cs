using Azure.Storage.Blobs;
using CLDV_Functions.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.Storage;
using Microsoft.Azure.Functions.Worker.Http;
using Azure.Storage.Files.Shares;
using Azure;
using System.IO;
using System.Text;
using Azure.Storage.Files.Shares.Models;
using System.Reflection.Metadata;
using System.IO.Pipes;
using Microsoft.Extensions.Azure;
using Microsoft.Data.SqlClient;
namespace CLDV_Functions
{
    public class TableFunction
    {
        private readonly ILogger<TableFunction> _logger;
        private static string connectionString = "Server=tcp:st10255930.database.windows.net,1433;Initial Catalog=ST10255930_CLDV_POE_P1;Persist Security Info=False;User ID=joshua;Password=Lesley*1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public TableFunction(ILogger<TableFunction> logger)
        {
            _logger = logger;
        }

        [Function("InsertUserTest")]
        public async Task<IActionResult> InsertUserTest(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req, ILogger log)
        {
            // Read form data from the request
            var formData = await req.ReadFormAsync();

            var userName = formData["User_Name"].ToString();
            var email = formData["Email"].ToString();
            var password = formData["Password"].ToString();

            // Insert user data into SQL database
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();
                string sql = "INSERT INTO Users (User_Name, Email, Password) VALUES (@UserName, @Email, @Password)";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return new OkObjectResult("User successfully added to SQL Database.");
        }
        //Rudman, G. (2024)
        //BCA2 CLDV Part 2 Workshop, YouTube.
        //Available at: https://www.youtube.com/watch?v=I_tiFJ-nlfE&list=LL&index=1&t=13s
        //(Accessed: 18 October 2024). 




        //Kharche, S. (2022)
        //How to upload files into Azure blog storage using Azure functions in C#, DEV Community.
        //Available at: https://dev.to/sumitkharche/how-to-upload-files-into-azure-blog-storage-using-azure-functions-in-c-4ga3
        //(Accessed: 01 October 2024). 

        [Function("InsertProductTest")]
        public async Task<IActionResult> InsertProductTest(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req, ILogger log)
        {
            // Parse form data
            var form = await req.ReadFormAsync();
            string productName = form["Product_Name"];
            string description = form["Description"];
            decimal price = decimal.Parse(form["Price"]);
            var imageFile = form.Files["File"];

            // Upload image to Blob Storage and get URL
            //Kharche, S. (2022)
            string azureConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string containerName = "products";
            string imageUrl;

            var blobClient = new BlobContainerClient(azureConnectionString, containerName);
            var blob = blobClient.GetBlobClient(imageFile.FileName);

            using (var imageStream = imageFile.OpenReadStream())
            {
                await blob.UploadAsync(imageStream, overwrite: true);
                imageUrl = blob.Uri.ToString();
            }

            // Insert product data into SQL database
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();
                string sql = "INSERT INTO Product (Product_Name, Description, ImageUrl, Price) VALUES (@name, @description, @imageUrl, @price)";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@name", productName);
                    cmd.Parameters.AddWithValue("@description", description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@imageUrl", imageUrl ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@price", price);

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return new OkObjectResult("Product added successfully");
        }
    }
    //Rudman, G. (2024)
    //BCA2 CLDV Part 2 Workshop, YouTube.
    //Available at: https://www.youtube.com/watch?v=I_tiFJ-nlfE&list=LL&index=1&t=13s
    //(Accessed: 18 October 2024). 

    public class QueueFunction
    {
        private readonly ILogger<QueueFunction> _logger;

        public QueueFunction(ILogger<QueueFunction> logger)
        {
            _logger = logger;
        }
        // Queue trigger method
        [Function("HttpQueueFunction")]
        [QueueOutput("processes")]
        public string RunHttp([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req, FunctionContext executionContext)
        {
            _logger.LogInformation("HTTP trigger function processed a request.");

            // Return a simple sentence to the queue
            string message = "This is a simple sentence sent to the queue from HTTP trigger.";

            // Log and return the message
            _logger.LogInformation("Sending message: {msg}", message);

            return message;
        }
    }
    // ggailey777 (2024)
    // Azure queue storage trigger for Azure functions, Microsoft Learn.
    // Available at: https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-queue-trigger?tabs=python-v2%2Cisolated-process%2Cnodejs-v4%2Cextensionv5&pivots=programming-language-csharp
    // (Accessed: 01 October 2024).

    //Kharche, S. (2022)
    //How to upload files into Azure blog storage using Azure functions in C#, DEV Community.
    //Available at: https://dev.to/sumitkharche/how-to-upload-files-into-azure-blog-storage-using-azure-functions-in-c-4ga3
    //(Accessed: 01 October 2024). 

    public static class UploadFileFunction
        {
            [Function("FileShare")]
            public static async Task<IActionResult> Run(
                [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req, ILogger log)
            {
                string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                string shareName = req.Form["shareName"].ToString() ?? "fileshare";
                string directoryName = req.Form["directoryName"].ToString() ?? "uploads";

                var file = req.Form.Files["file"];

                if (file == null || file.Length == 0)
                {
                    return new BadRequestObjectResult("File is required.");
                }

                var shareServiceClient = new ShareServiceClient(connectionString);
                var shareClient = shareServiceClient.GetShareClient(shareName);
                await shareClient.CreateIfNotExistsAsync();

                var directoryClient = shareClient.GetDirectoryClient(directoryName);
                await directoryClient.CreateIfNotExistsAsync();

                var fileClient = directoryClient.GetFileClient(file.FileName);
                using (var stream = file.OpenReadStream())
                {
                    await fileClient.CreateAsync(file.Length);
                    await fileClient.UploadRangeAsync(new HttpRange(0, stream.Length), stream);
                }

                return new OkObjectResult("File uploaded successfully to Azure File Share.");
            }
        }
    }

//Kharche, S. (2022)
//How to upload files into Azure blog storage using Azure functions in C#, DEV Community.
//Available at: https://dev.to/sumitkharche/how-to-upload-files-into-azure-blog-storage-using-azure-functions-in-c-4ga3
//(Accessed: 01 October 2024).