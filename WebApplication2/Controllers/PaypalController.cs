using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Text;
using Daishi.PaySharp;

namespace WebApplication2.Controllers
{
    public class PaypalController : Controller
    {
        string user = ConfigurationManager.AppSettings["User"];
        string password = ConfigurationManager.AppSettings["Password"];
        string signature = ConfigurationManager.AppSettings["Signature"];
        string subject = ConfigurationManager.AppSettings["Subject"];
        string accessToken;
        PayPalError payPalError;

        PayPalAdapter payPalAdapter = new PayPalAdapter();

        public ActionResult Index()
        { 
           var setExpresscheckout =
                payPalAdapter.SetExpressCheckout(
                    new SetExpressCheckoutPayload
                    {
                        User = user,
                        Password = password,
                        Signature = signature,
                        Version = "108.0",
                        Amount = "9.01",
                        Subject = subject,
                        LocaleCode = "en-IE",
                        CurrencyCode = "CAD",
                        CancelUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/Paypal/Failure",
                        ReturnUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/Paypal/Success",
                        PaymentRequestName = "TEST",
                        PaymentRequestDescription = "TEST BOOKING"
                    },
                    Encoding.UTF8,
                    ConfigurationManager.AppSettings["ExpressCheckoutURI"]);


            var ok = PayPalUtility.TryParseAccessToken(setExpresscheckout,
                out accessToken, out payPalError);

            if (ok)
            {
                return Redirect("https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_express-checkout&token="+ accessToken);
            }
            else
            {
                //var newModel = new Models.PaypalResult { id = 123, name = payPalError.LongMessage };
                return View();
            }
            
            
        }

        public ActionResult Success(string token, string PayerID = "") {
            var getExpressCheckoutDetails = payPalAdapter.GetExpressCheckoutDetails(
            new GetExpressCheckoutDetailsPayload
             {
             User = user,
             Password = password,
             Signature = signature,
             Version = "108.0",
             AccessToken = token,
             Subject = subject,
             PayerID = PayerID
             },
            ConfigurationManager.AppSettings["ExpressCheckoutURI"]);

            CustomerDetails customerDetails;

            var ok = PayPalUtility.TryParseCustomerDetails(
                getExpressCheckoutDetails, out customerDetails,
                out payPalError);

   
            if (!ok)
            {
                var newModel = new Models.PaypalResult { PayerID = payPalError.LongMessage, token = payPalError.LongMessage };
                return View("Index", newModel);
            }

            var doExpressCheckoutPayment = payPalAdapter
                    .DoExpressCheckoutPayment(
                        new DoExpressCheckoutPaymentPayload
                        {
                            User = user,
                            Password = password,
                            Signature = signature,
                            Version = "108.0",
                            AccessToken = token,
                            Subject = subject,
                            PayerID = PayerID,
                            PaymentRequestAmt = customerDetails.Amt,
                            PaymentRequestCurrencyCode = "CAD"
                        },
                        ConfigurationManager.AppSettings["ExpressCheckoutURI"]);

            TransactionResults transactionResults;

            ok = PayPalUtility.TryParseTransactionResults(
                doExpressCheckoutPayment, out transactionResults,
                out payPalError);

            if (ok)
            {
                var newModel = new Models.PaypalResult { PayerID = transactionResults.PaymentInfoPaymentStatus, token = customerDetails.Amt };
                // Completed
                return View("Index", newModel);

            }
            else
            {
                var newModel = new Models.PaypalResult { PayerID = payPalError.LongMessage, token = customerDetails.Amt };
                return View("Index", newModel);
            }
        }
    }
}