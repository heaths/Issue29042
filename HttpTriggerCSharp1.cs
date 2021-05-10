using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using System.Text.Json;

namespace Company.Function
{
    public static class HttpTriggerCSharp1
    {
        [FunctionName("HttpTriggerCSharp1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var vaultUri = Environment.GetEnvironmentVariable("AZURE_KEYVAULT_URL") ?? "https://heathskv.vault.azure.net";
            var client = new KeyClient(
                new Uri(vaultUri),
                new DefaultAzureCredential());

            try
            {
                var response = await client.CreateRsaKeyAsync(
                    new CreateRsaKeyOptions("issue20942")
                    {
                        KeySize = 2048,
                    });

                log.LogInformation($"Created {response.Value.Name}");
                return new OkResult();
            }
            catch (Exception ex)
            {
                log.LogWarning(ex, $"System.Text.Json version {typeof(JsonEncodedText).Assembly.GetName().Version}");
                throw;
            }
        }
    }
}
