using DotNet_Weather;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Define the summaries for weather conditions, expanded for more variety.
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
    "Partly Cloudy", "Cloudy", "Rainy", "Stormy", "Foggy", "Snowy", "Windy", "Clear", "Overcast", "Drizzly"
};

app.MapGet("/health", () => "Healthy");

// Define the handler for the /weather endpoint.
app.MapGet("/weather", async (HttpContext context) => {
    var logger = app.Logger;
        
    // Set Content-Type header to application/json.
    context.Response.ContentType = "application/json";

    // Get response size from query parameter, default to 5 if not provided or invalid.
    // The range is 10 to 100 as per previous requirements.
    int size = 10; // Default size
    if (context.Request.Query.ContainsKey("size") && int.TryParse(context.Request.Query["size"], out int parsedSize))
    {
        if (parsedSize >= 10 && parsedSize <= 100)
        {
            size = parsedSize;
        }
        else
        {
            logger.LogWarning("Invalid 'size' parameter value: {Size}. Defaulting to 10. (Allowed range: 10-100)", parsedSize);
        }
    }
    else
    {
        logger.LogWarning("Missing or invalid 'size' query parameter. Defaulting to 10.");
    }

    // Introduce a random delay between 0 and 5 seconds.
    int delayMs = Random.Shared.Next(0, 5001); // 0 to 5000 milliseconds
    logger.LogInformation("Introducing a delay of {DelayMs}ms for this request.", delayMs);
    await Task.Delay(delayMs);

    // Get a random status code.
    var statusCode = Weather.GetResponseStatusCode();
    context.Response.StatusCode = statusCode;

    // Depending on the status code, provide appropriate response body.
    if (statusCode >= StatusCodes.Status200OK && statusCode < StatusCodes.Status300MultipleChoices)
    {
        // Generate dummy weather forecast data.
        var forecast = Enumerable.Range(1, size).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();
        logger.LogInformation("Responding with {StatusCode} status code and {Count} weather forecasts.", statusCode, forecast.Length);

        if (statusCode == StatusCodes.Status204NoContent)
        {
            await context.Request.Body.FlushAsync();
        }
        else
        {
            await context.Response.WriteAsJsonAsync(forecast);
        }
    }
    else
    {
        // For 4xx and 5xx errors, provide a generic error message.
        var errorMessage = new { Message = $"An error occurred with status code {statusCode}. This is a dummy error for testing." };
        logger.LogInformation("Responding with {StatusCode} status code and error message: {Message}", statusCode, errorMessage.Message);
        await context.Response.WriteAsJsonAsync(errorMessage);
    }
})
.WithName("GetWeatherForecast"); // Keep the existing name for OpenAPI.

// Start the HTTP server.
app.Logger.LogInformation("Starting C# .NET REST API server on port 5000 (or as configured).");
app.Run("http://0.0.0.0:5000");

// Record definition for WeatherForecast (kept as is from your provided code).
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

