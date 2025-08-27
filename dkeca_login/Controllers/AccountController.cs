using dkeca_login;
using dkeca_login.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl;

    public AccountController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiBaseUrl = apiSettings.Value.BaseUrl;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var json = JsonSerializer.Serialize(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Call your WebAPI endpoint (adjust the URL if needed)
        var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/auth/login", content);

        if (response.IsSuccessStatusCode)
        {
            var tokenJson = await response.Content.ReadAsStringAsync();
            // Save JWT to cookie or session
            HttpContext.Session.SetString("jwt", tokenJson);

            return RedirectToAction("Landing", "Home");
        }
        else
        {
            ViewBag.ErrorMessage = "Login failed.";
            return View();
        }
    }
}
