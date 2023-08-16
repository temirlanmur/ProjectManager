namespace ProjectManager.WebAPI.Authentication.Utils;

public interface IJwtTokenGenerator
{
    public string GenerateToken(Guid userId, string firstName, string lastName);
}
