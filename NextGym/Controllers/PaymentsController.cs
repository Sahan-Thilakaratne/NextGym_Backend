using Application.Billing;
using Contracts.Billing;
using Microsoft.AspNetCore.Mvc;


namespace NextGym.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _service;
    public PaymentsController(IPaymentService service) => _service = service;

    // POST /api/payments (record payment)
    [HttpPost]
    public async Task<ActionResult<PaymentDto>> Record([FromBody] RecordPaymentRequest req, CancellationToken ct)
        => Ok(await _service.RecordAsync(req, ct));

    // GET /api/payments/subscription/{subscriptionId}
    [HttpGet("subscription/{subscriptionId:long}")]
    public async Task<ActionResult<IReadOnlyList<PaymentDto>>> GetBySubscription(long subscriptionId, CancellationToken ct)
        => Ok(await _service.GetBySubscriptionAsync(subscriptionId, ct));

    // GET /api/payments/{paymentId}/receipt
    [HttpGet("{paymentId:long}/receipt")]
    public async Task<ActionResult<ReceiptDto>> Receipt(long paymentId, CancellationToken ct)
    {
        var receipt = await _service.GetReceiptAsync(paymentId, ct);
        return receipt is null ? NotFound() : Ok(receipt);
    }
}
