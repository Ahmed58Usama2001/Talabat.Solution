using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Controllers;

public class PaymentsController : BaseApiController
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;
    const string _whSecret = "whsec_d27e4894ac645b1f42abd4d94bbed2cd17321b62d87eca272618184da443a39b";

    public PaymentsController(IPaymentService paymentService,ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [Authorize]
    [ProducesResponseType(typeof(CustomerBasket),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status400BadRequest)]
    [HttpPost("{basketId}")]
    public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
    {
        var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

        if (basket == null) return BadRequest(new ApiResponse(400, "An error with your basket"));

        return Ok(basket);
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        
        
            var stripeEvent = EventUtility.ConstructEvent(json,
                Request.Headers["Stripe-Signature"], _whSecret);

            var paymentIntent =(PaymentIntent) stripeEvent.Data.Object;

            Order order;

            switch (stripeEvent.Type)
            {
                case Events.PaymentIntentSucceeded:
                    order=await _paymentService.UpdatePaymentIntentToSucceedOrFailed(paymentIntent.Id, true);
                    _logger.LogInformation("Payment is succeeded", paymentIntent.Id);
                    break;

                case Events.PaymentIntentPaymentFailed:
                    order=await _paymentService.UpdatePaymentIntentToSucceedOrFailed(paymentIntent.Id, false);
                    _logger.LogInformation("Payment is failed", paymentIntent.Id);
                    break;
            }

            return Ok();       
    }
}
