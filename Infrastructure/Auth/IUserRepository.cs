using Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Auth
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username, CancellationToken ct);
        Task<User?> GetByIdAsync(long id, CancellationToken ct);
        Task<bool> UsernameExistsAsync(string username, long? excludeId, CancellationToken ct);
        Task AddAsync(User user, CancellationToken ct);
        Task UpdateAsync(User user, CancellationToken ct);
    }
}
