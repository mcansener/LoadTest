using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using LoadTest;
using Microsoft.Extensions.Configuration;

class Program
{
    private static readonly HttpClient httpClient = new HttpClient();

    static async Task Main()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var requestConfig = new RequestConfig();
        configuration.GetSection("RequestConfig").Bind(requestConfig);

        var responseConfig = new ResponseConfig();
        configuration.GetSection("ResponseConfig").Bind(responseConfig);

        var appConfig = new AppConfig();
        configuration.GetSection("AppConfig").Bind(appConfig);

        Console.WriteLine("Start: " + DateTime.Now.ToString());

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        int count = 0;
        while (stopwatch.ElapsedMilliseconds < appConfig.RequestDurationInMin * 60 * 1000 && count < appConfig.RequestCount)
        {
            await MakeRequest(requestConfig, responseConfig);
            count++;
            await Task.Delay(appConfig.RequestIntervalMilliseconds);
        }

        stopwatch.Stop();

        Console.WriteLine("End: " + DateTime.Now.ToString());
        Console.WriteLine("Total Request Count: " + count);
    }

    static async Task MakeRequest(RequestConfig requestConfig, ResponseConfig responseConfig)
    {
        using (var request = new HttpRequestMessage(new HttpMethod(requestConfig.RequestMethod), requestConfig.RequestUrl))
        {
            foreach (var header in requestConfig.RequestHeaders)
            {
                // Skip setting Content-Type header directly on Headers
                if (header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                    continue;

                request.Headers.Add(header.Key, header.Value);
            }

            if (requestConfig.RequestBody != null)
            {
                var json = System.Text.Json.JsonSerializer.Serialize(requestConfig.RequestBody);
                request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            }

            // Set Content-Type header on HttpContent
            if (request.Content != null && requestConfig.RequestHeaders.ContainsKey("Content-Type"))
            {
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(requestConfig.RequestHeaders["Content-Type"]);
            }

            var response = await httpClient.SendAsync(request);

            if ((int)response.StatusCode == responseConfig.ExpectedStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                if (responseContent.Contains(responseConfig.ExpectedResponseContains))
                {
                    Console.WriteLine("Request successful!");
                }
                else
                {
                    Console.WriteLine("Unexpected response content.");
                }
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
            }
        }
    }
}