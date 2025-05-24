using Contract.Domain.Enums;

namespace AdminPanel.Mvc.Constants;

public static class ValidRoles
{
    public static string SeniorManagerRole { get; } = Role.SeniorManager.ToString();
    public static string AdminRole { get; } = Role.Administrator.ToString();
    public static string ManagerRole { get; } = Role.Manager.ToString();
    public static IEnumerable<string> Roles { get; } = new[] { Role.Manager.ToString(), Role.Administrator.ToString(), Role.SeniorManager.ToString() };

    public static IEnumerable<Role> EnumRoles { get; } =
        new[] { Role.Manager, Role.SeniorManager, Role.Administrator };
}