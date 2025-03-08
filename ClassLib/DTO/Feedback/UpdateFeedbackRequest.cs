using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.DTO.Feedback
{
    public class UpdateFeedbackRequest
    {
        public int RatingScore { get; set; }

        public string? Description { get; set; }
    }
}
