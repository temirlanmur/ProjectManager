namespace ProjectManager.WebAPI.Authentication.Utils;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string Audience { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60;
}
