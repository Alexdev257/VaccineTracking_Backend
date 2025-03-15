using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.DTO.Feedback
{
    public class GetFeedbackAdminResponse
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int RatingScore { get; set; }

        public string? Description { get; set; }

        public bool IsDeleted { get; set; }
    }
}
