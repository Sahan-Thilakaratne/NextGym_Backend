using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Members
{
    public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int Total);
}
