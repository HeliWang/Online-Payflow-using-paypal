using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class PaypalResult
    {
        public string token { get; set; }
        public string PayerID { get; set; }
    }
}