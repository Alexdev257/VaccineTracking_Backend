namespace ClassLib.DTO.Feedback
{
    public class GetFeedbackResponse
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int RatingScore { get; set; }

        public string? Description { get; set; }

    }
}
