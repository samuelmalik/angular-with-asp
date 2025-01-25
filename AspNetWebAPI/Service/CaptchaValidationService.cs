using System.Text.Json;

namespace AspNetCoreAPI.Service;

public class CaptchaValidationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public CaptchaValidationService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<bool> ValidateCaptchaToken(string captchaToken)
    {
        if (string.IsNullOrEmpty(captchaToken))
            return false;

        var secretKey = _configuration["GoogleReCAPTCHA:SecretKey"];
        using var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.PostAsync(
            $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={captchaToken}",
            null
        );

        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Google API response content: {responseContent}");

        var captchaResponse = JsonSerializer.Deserialize<ReCaptchaResponse>(
            responseContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (captchaResponse == null)
        {
            Console.WriteLine("CAPTCHA response is null.");
            return false;
        }

        Console.WriteLine($"CAPTCHA validation result: {captchaResponse.Success}");
        Console.WriteLine($"Hostname: {captchaResponse.Hostname}");
        Console.WriteLine($"Error Codes: {(captchaResponse.ErrorCodes != null ? string.Join(", ", captchaResponse.ErrorCodes) : "None")}");

        if (!captchaResponse.Success)
        {
            Console.WriteLine("CAPTCHA validation failed.");
            return false;
        }

        Console.WriteLine("CAPTCHA validation succeeded.");
        return true;
    }
}

public class ReCaptchaResponse
{
    public bool Success { get; set; }
    public string ChallengeTs { get; set; }
    public string Hostname { get; set; }
    public string[] ErrorCodes { get; set; }
}