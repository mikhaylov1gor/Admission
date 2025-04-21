namespace UserService.Application.Dtos.Responses;

public class TokenResponseDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}