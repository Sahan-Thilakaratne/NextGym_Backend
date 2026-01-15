using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Members
{
    public sealed record GetMembersQuery(string? Search, int Page, int PageSize);
}
