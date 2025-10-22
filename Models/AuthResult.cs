namespace ContactManagerApp.Models
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? LocalId { get; set; }
        public string? Email { get; set; }
        public string? DisplayName { get; set; }
        public string? FirebaseToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
