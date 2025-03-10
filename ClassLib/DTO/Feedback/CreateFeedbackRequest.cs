namespace ClassLib.DTO.Feedback
{
    public class CreateFeedbackRequest
    {

        public int UserId { get; set; }

        public int RatingScore { get; set; }

        public string? Description { get; set; }


    }
}
