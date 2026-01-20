using Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth
{
    public interface ITokenService
    {
        (string token, DateTime expiresAtUtc) CreateToken(User user);
    }
}
