using Microsoft.AspNetCore.Identity;

#nullable disable

namespace YarpOidc.Model;
public class DummyUserStore : 
    IUserStore<ApplicationUser>, 
    IUserRoleStore<ApplicationUser> 
{
    public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(user.Id);
    public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(user.UserName);

    public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
    public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
    public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
    public Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken) => Task.FromResult<ApplicationUser>(null);
    public Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) => Task.FromResult<ApplicationUser>(null);
    public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken) => throw new NotImplementedException();
    
    public Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken) => Task.CompletedTask;
    public Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken) => Task.CompletedTask;
    public Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult<IList<string>>(new List<string>());
    public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken) => Task.FromResult(false);
    public Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken) => Task.FromResult<IList<ApplicationUser>>(new List<ApplicationUser>());
    public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) => Task.FromResult<ApplicationUser>(null);

    public void Dispose() { }
}

public class DummyRoleStore : IRoleStore<IdentityRole>
{
    public Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
    public Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
    public Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
    public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken) => Task.FromResult(role.Id);
    public Task<string> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken) => Task.FromResult(role.Name);
    public Task SetRoleNameAsync(IdentityRole role, string roleName, CancellationToken cancellationToken) => Task.CompletedTask;
    public Task<string> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken) => Task.FromResult(role.NormalizedName);
    public Task SetNormalizedRoleNameAsync(IdentityRole role, string normalizedName, CancellationToken cancellationToken) => Task.CompletedTask;
    public Task<IdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken) => Task.FromResult<IdentityRole>(null);
    public Task<IdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken) => Task.FromResult<IdentityRole>(null);
    public void Dispose() { }
}