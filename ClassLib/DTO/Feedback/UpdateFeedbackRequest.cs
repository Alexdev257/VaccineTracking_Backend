namespace ClassLib.DTO.Feedback
{
    public class UpdateFeedbackRequest
    {
        public int RatingScore { get; set; }

        public string? Description { get; set; }
    }
}
