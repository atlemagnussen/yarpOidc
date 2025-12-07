using System.Security.Claims;

namespace YarpOidc.Model;

public static class Auth
{
    public static AuthInfo GetInfo(ClaimsPrincipal user)
    {
        var info = new AuthInfo();
        if (user.Identity is null || !user.Identity.IsAuthenticated)
            return info;
        
        info.LoggedIn = true;
        
        var claims = user.Claims.ToArray();
        var firstClaim = claims.First();

        info.Issuer = firstClaim.Issuer;

        info.Claims = claims.Select(c => new ClaimDTO(c.Type, c.Value)).ToList();
        return info;
    }
}

public record AuthInfo
{
    public bool LoggedIn {get;set;}
    public string? Issuer {get;set;}
    public List<ClaimDTO> Claims {get;set;} = [];
}

public record ClaimDTO(string Type, string Value)
{
    public string Type {get;init;} = Type;
    public string Value {get;init;} = Value;
}

public static class ClaimsHelper
{
    public static ApplicationUser? GetAppUser(ClaimsPrincipal external)
    {
        var allClaims = external.Claims.ToList();
        if (allClaims is null)
            return null;

        var userName = allClaims.GetValueByType("name", "preferred_username");
        if (userName is null)
            return null;

        var sub = allClaims.GetValueByType("sub");
        if (sub is null)
            return null;

        return new ApplicationUser
        {
            Id = sub,
            UserName = userName
        };
    }
    /// <summary>
    /// Will try the first claim name and then all the way to the last
    /// </summary>
    public static string? GetValueByType(this IEnumerable<Claim> claims, params string[] types)
    {

        foreach (var type in types)
        {
            var claimOfType = claims.FirstOrDefault(c => c.Type == type);
            if (claimOfType != null)
            {
                return claimOfType.Value;
            }
        }
        return null;
    }
}