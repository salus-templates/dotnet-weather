namespace DotNet_Weather;

public static class Weather
{
    // Helper method to randomly select a 2xx, 4xx, or 5xx status code.
    public static int GetResponseStatusCode()
    {
        int[] statusCodes2xx = { StatusCodes.Status200OK, StatusCodes.Status201Created, StatusCodes.Status202Accepted, StatusCodes.Status204NoContent };
        int[] statusCodes4xx = { StatusCodes.Status400BadRequest, StatusCodes.Status401Unauthorized, StatusCodes.Status404NotFound, StatusCodes.Status403Forbidden, StatusCodes.Status405MethodNotAllowed };
        int[] statusCodes5xx = { StatusCodes.Status500InternalServerError, StatusCodes.Status501NotImplemented };

        // Randomly decide the type of response: 2xx, 4xx, or 5xx
        // Weights: 70% 2xx, 15% 4xx, 15% 5xx
        int randomNumber = Random.Shared.Next(100); // 0-99
        if (randomNumber < 70) // 70% chance for 2xx
        {
            return statusCodes2xx[Random.Shared.Next(statusCodes2xx.Length)];
        }
        else if (randomNumber < 85) // 15% chance for 4xx (70-84)
        {
            return statusCodes4xx[Random.Shared.Next(statusCodes4xx.Length)];
        }
        else // 15% chance for 5xx (85-99)
        {
            return statusCodes5xx[Random.Shared.Next(statusCodes5xx.Length)];
        }
    }

}