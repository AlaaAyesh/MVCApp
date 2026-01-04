using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class AuthController : Controller
{
    private readonly ApiAuthService _apiAuthService;

    public AuthController(ApiAuthService apiAuthService)
    {
        _apiAuthService = apiAuthService;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("", "Email and password are required.");
            return View();
        }
        var loginResponse = await _apiAuthService.LoginAsyncWithUser(email, password);
        if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
        {
            ModelState.AddModelError("", "Invalid email or password. Please try again.");
            return View();
        }
        HttpContext.Session.SetString("JwtToken", loginResponse.Token);
        HttpContext.Session.SetString("FirstName", loginResponse.FirstName ?? "User");
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(string email, string password, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            ModelState.AddModelError("", "All fields are required.");
            return View();
        }
        var registerRequest = new { email, password, firstName, lastName };
        var success = await _apiAuthService.RegisterAsync(registerRequest);
        if (!success)
        {
            ModelState.AddModelError("", "Registration failed. Please check your details and try again. Email may already be in use.");
            return View();
        }
        // Optionally, auto-login after registration:
        // var loginResponse = await _apiAuthService.LoginAsyncWithUser(email, password);
        // if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
        // {
        //     HttpContext.Session.SetString("JwtToken", loginResponse.Token);
        //     HttpContext.Session.SetString("FirstName", loginResponse.FirstName ?? "User");
        //     return RedirectToAction("Index", "Home");
        // }
        return RedirectToAction("Login");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("JwtToken");
        HttpContext.Session.Remove("FirstName");
        return RedirectToAction("Login");
    }
} 