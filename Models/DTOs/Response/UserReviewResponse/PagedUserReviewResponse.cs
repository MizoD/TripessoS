using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Response.UserReviewResponse
{
    public class PagedUserReviewResponse
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalReviews { get; set; }
        public int TotalPages { get; set; }
        public List<UserReviewResponse> Reviews { get; set; } = new();
    }
}
