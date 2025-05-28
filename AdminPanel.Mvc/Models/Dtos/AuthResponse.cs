namespace AdminPanel.Mvc.Models.Dtos;

public class AuthResponse
{
    public required string accessToken { get; set; }
    public required string refreshToken { get; set; }
}