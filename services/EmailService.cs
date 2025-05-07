using System;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using System.Net.Http;
using System.Text.Json;

namespace CanvasCertificateGenerator.Services;

public class EmailService
{
    public string To { get; set; } = "";
    public string Name { get; set; } = "";
    public string Course { get; set; } = "";
    public string Base64Pdf { get; set; } = "";
    public string FileName { get; set; } = "";
    public int CertificateType { get; set; } = 1;

    public static async Task SendEmailViaLambdaAsync(string email, string name, byte[] pdfBytes, string fileName, string emailPassword)
    {
        const string lambdaEndpoint = "https://pa6htm91eb.execute-api.us-east-2.amazonaws.com/prod/email";
        
        var payload = new EmailService
        {
            To = email,
            Name = name,
            Base64Pdf = Convert.ToBase64String(pdfBytes),
            FileName = fileName,
            CertificateType = 2
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        using var httpClient = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Post, lambdaEndpoint)
        {
            Content = content
        };
        
        request.Headers.Add("x-api-password", emailPassword);

        var response = await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to send email: {response.StatusCode}\n{error}");
        }
    }

    public static bool Validate(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var emailAttribute = new EmailAddressAttribute();
        return emailAttribute.IsValid(email);
    }
}