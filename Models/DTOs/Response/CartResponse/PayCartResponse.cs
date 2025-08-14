using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Response.CartResponse
{
    public class PayCartResponse
    {
        public string Message { get; set; } = "";
        public string? StripeSessionUrl { get; set; }

    }
}
