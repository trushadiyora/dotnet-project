using ContactManagerApp.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ContactManagerApp.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ContactManagerApp.Configuration.FirebaseOptions _options;

        public AuthService(IHttpClientFactory httpClientFactory, IOptions<ContactManagerApp.Configuration.FirebaseOptions> options)
        {
            _httpClient = httpClientFactory.CreateClient("FirebaseAuthClient");
            _options = options.Value ?? throw new ArgumentException("Firebase options not configured");

            if (string.IsNullOrEmpty(_options.WebApiKey))
                throw new ArgumentException("Firebase WebApiKey is not configured.");
        }

        private string Api(string path) => $"https://identitytoolkit.googleapis.com/v1/{path}?key={_options.WebApiKey}";

        // Register new user (signUp)
        public async Task<AuthResult> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                var payload = new { email = model.Email, password = model.Password, returnSecureToken = true, displayName = model.DisplayName };
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var resp = await _httpClient.PostAsync(Api("accounts:signUp"), content);
                var body = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                {
                    return new AuthResult { Success = false, Message = ExtractFirebaseError(body) };
                }

                dynamic data = JsonConvert.DeserializeObject(body) ?? new { };
                return new AuthResult
                {
                    Success = true,
                    Message = "Registration successful",
                    LocalId = data?.localId?.ToString(),
                    Email = data?.email?.ToString(),
                    DisplayName = data?.displayName?.ToString(),
                    FirebaseToken = data?.idToken?.ToString(),
                    RefreshToken = data?.refreshToken?.ToString()
                };
            }
            catch (Exception ex)
            {
                return new AuthResult { Success = false, Message = ex.Message };
            }
        }

        // Login user (signInWithPassword)
        public async Task<AuthResult> LoginAsync(LoginViewModel model)
        {
            try
            {
                var payload = new { email = model.Email, password = model.Password, returnSecureToken = true };
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var resp = await _httpClient.PostAsync(Api("accounts:signInWithPassword"), content);
                var body = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                {
                    return new AuthResult { Success = false, Message = ExtractFirebaseError(body) };
                }

                dynamic data = JsonConvert.DeserializeObject(body) ?? new { };
                return new AuthResult
                {
                    Success = true,
                    Message = "Login successful",
                    LocalId = data?.localId?.ToString(),
                    Email = data?.email?.ToString(),
                    DisplayName = data?.displayName?.ToString(),
                    FirebaseToken = data?.idToken?.ToString(),
                    RefreshToken = data?.refreshToken?.ToString()
                };
            }
            catch (Exception ex)
            {
                return new AuthResult { Success = false, Message = ex.Message };
            }
        }

        // Refresh token using securetoken endpoint
        public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var url = $"https://securetoken.googleapis.com/v1/token?key={_options.WebApiKey}";
                var content = new StringContent($"grant_type=refresh_token&refresh_token={refreshToken}", Encoding.UTF8, "application/x-www-form-urlencoded");
                var resp = await _httpClient.PostAsync(url, content);
                var body = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                    return new AuthResult { Success = false, Message = ExtractFirebaseError(body) };

                dynamic data = JsonConvert.DeserializeObject(body) ?? new { };
                return new AuthResult
                {
                    Success = true,
                    Message = "Token refreshed",
                    FirebaseToken = data?.id_token?.ToString(),
                    RefreshToken = data?.refresh_token?.ToString(),
                    LocalId = data?.user_id?.ToString()
                };
            }
            catch (Exception ex)
            {
                return new AuthResult { Success = false, Message = ex.Message };
            }
        }

        // Reset password (sendOobCode)
        public async Task<(bool success, string message)> ResetPasswordAsync(string email)
        {
            try
            {
                var payload = new { requestType = "PASSWORD_RESET", email };
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var resp = await _httpClient.PostAsync(Api("accounts:sendOobCode"), content);
                var body = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                    return (false, ExtractFirebaseError(body));

                return (true, "Password reset email sent");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // Get current user by idToken (accounts:lookup)
        public async Task<AuthResult> GetCurrentUserAsync(string idToken)
        {
            try
            {
                var payload = new { idToken };
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var resp = await _httpClient.PostAsync(Api("accounts:lookup"), content);
                var body = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                    return new AuthResult { Success = false, Message = ExtractFirebaseError(body) };

                dynamic data = JsonConvert.DeserializeObject(body) ?? new { };
                var user = data?.users?[0];
                if (user == null)
                    return new AuthResult { Success = false, Message = "User not found" };

                return new AuthResult
                {
                    Success = true,
                    Message = "User retrieved",
                    LocalId = user?.localId?.ToString(),
                    Email = user?.email?.ToString(),
                    DisplayName = user?.displayName?.ToString()
                };
            }
            catch (Exception ex)
            {
                return new AuthResult { Success = false, Message = ex.Message };
            }
        }

        private string ExtractFirebaseError(string body)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<dynamic>(body);
                var msg = obj?.error?.message;
                return msg != null ? (string)msg : body;
            }
            catch
            {
                return body;
            }
        }
    }
}
