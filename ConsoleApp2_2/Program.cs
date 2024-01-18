using System.Collections;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Numerics;
using System.Text.Json;
using System.IO;
using System.Net.Mail;
using System.Text.Json.Serialization;


try
{
    var s = File.ReadAllText("creds.json");
    Config creds = JsonSerializer.Deserialize<Config>(s);
    IDictionary<string, string> personCredentials = new Dictionary<string, string> { { "username", creds.Username }, { "password", creds.Password } };
    HttpClient client = new HttpClient();
    using HttpResponseMessage response = await client.PostAsJsonAsync<IDictionary<string, string>>("https://dummyjson.com/auth/login", personCredentials);
    response.EnsureSuccessStatusCode();
    string responseBody = await response.Content.ReadAsStringAsync();
    Console.WriteLine("Got person data for the login: ");
    var personData = JsonSerializer.Deserialize<Person>(responseBody);
    Console.WriteLine(JsonSerializer.Serialize<Person>(personData));
    var currentTimestamp = new BigInteger(DateTime.UtcNow.Ticks);

    File.AppendAllText("tokenLog.txt", $"New token at {currentTimestamp} ({(currentTimestamp.IsEven ? "even" : "odd")} tick) for user ID {personData.Id}: {personData.Token}\n");

    var smtpClient = new SmtpClient("smtp.gmail.com")
    {
        Port = 587,
        Credentials = new NetworkCredential(creds.SmtpEmail, creds.SmtpPassword),
        EnableSsl = true,
    };

    smtpClient.Send("fieldlavender70@gmail.com", "fieldlavender70@gmail.com", "test_msg", $"Hello from {personData.Username}");
}
catch (HttpRequestException e)
{
    Console.WriteLine("\nException Caught!");
    Console.WriteLine("Message :{0} ", e.Message);
}

public class Config
{
    [JsonPropertyName("username")]
    public string Username { get; set; }
    [JsonPropertyName("password")]
    public string Password { get; set; }
    [JsonPropertyName("smtpEmail")]
    public string SmtpEmail { get; set; }
    [JsonPropertyName("smtpPassword")]
    public string SmtpPassword { get; set; }
}

public struct Person
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("username")]
    public string Username { get; set; }
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("firstname")]
    public string FirstName { get; set; }
    [JsonPropertyName("lastname")]
    public string LastName { get; set; }
    [JsonPropertyName("gender")]
    public string Gender { get; set; }
    [JsonPropertyName("image")]
    public string Image { get; set; }
    [JsonPropertyName("token")]
    public string Token { get; set; }
}
