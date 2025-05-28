namespace AdminPanel.Mvc.Models.Dtos.Requests;

public class ChangePasswordRequest
{
    public string oldPassword { get; set; }
    public string newPassword { get; set; }
}