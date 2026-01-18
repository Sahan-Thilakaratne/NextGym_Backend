using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Billing
{
    public enum SubscriptionStatus
    {
        ACTIVE = 0,
        EXPIRED = 1,
        PAUSED = 2
    }

    public enum PaymentMethod
    {
        CASH = 0,
        CARD = 1,
        ONLINE = 2
    }

    public enum PaymentStatus
    {
        PAID = 0,
        FAILED = 1,
        REFUNDED = 2
    }
}
