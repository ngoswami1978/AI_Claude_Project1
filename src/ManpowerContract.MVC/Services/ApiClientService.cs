using System.Net.Http.Headers;
using System.Text;
using ManpowerContract.Application.Common;
using Newtonsoft.Json;

namespace ManpowerContract.MVC.Services;

public class ApiClientService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiClientService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("ManpowerContractAPI");
        var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return client;
    }

    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
    {
        var client = CreateClient();
        var response = await client.GetAsync(endpoint);
        return await DeserializeResponse<T>(response);
    }

    public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data)
    {
        var client = CreateClient();
        var json = JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(endpoint, content);
        return await DeserializeResponse<T>(response);
    }

    public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data)
    {
        var client = CreateClient();
        var json = JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PutAsync(endpoint, content);
        return await DeserializeResponse<T>(response);
    }

    public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint)
    {
        var client = CreateClient();
        var response = await client.DeleteAsync(endpoint);
        return await DeserializeResponse<T>(response);
    }

    public async Task<List<T>> GetListAsync<T>(string endpoint)
    {
        var client = CreateClient();
        var response = await client.GetAsync(endpoint);
        var body = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<List<T>>(body) ?? new List<T>();
        }
        return new List<T>();
    }

    private async Task<ApiResponse<T>> DeserializeResponse<T>(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(body))
        {
            return response.IsSuccessStatusCode
                ? ApiResponse<T>.Ok(default!)
                : ApiResponse<T>.Fail("No response from server.");
        }

        try
        {
            var result = JsonConvert.DeserializeObject<ApiResponse<T>>(body);
            return result ?? ApiResponse<T>.Fail("Failed to parse response.");
        }
        catch
        {
            return response.IsSuccessStatusCode
                ? ApiResponse<T>.Ok(default!)
                : ApiResponse<T>.Fail($"Error: {response.StatusCode}");
        }
    }
}
