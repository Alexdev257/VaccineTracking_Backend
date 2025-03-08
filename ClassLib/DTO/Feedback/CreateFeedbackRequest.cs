using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.DTO.Feedback
{
    public class CreateFeedbackRequest
    {

        public int UserId { get; set; }

        public int RatingScore { get; set; }

        public string? Description { get; set; }


    }
}
