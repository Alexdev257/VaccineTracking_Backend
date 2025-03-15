using AutoMapper;
using ClassLib.DTO.Feedback;
using ClassLib.Models;
using ClassLib.Repositories;

namespace ClassLib.Service
{
    public class FeedbackService
    {
        private readonly FeedbackRepository _feedbackRepository;
        private IMapper _mapper;

        public FeedbackService(FeedbackRepository feedbackRepository, IMapper mapper)
        {
            _feedbackRepository = feedbackRepository;
            _mapper = mapper;
        }

        public async Task<List<GetFeedbackResponse>> GetAllFeedback()
        {
            var feedback = await _feedbackRepository.getAllFeedBackRepo();
            if(feedback.Count == 0)
            {
                throw new ArgumentException("Do not exist any feedbacks");
            }
            List<GetFeedbackResponse> result = new List<GetFeedbackResponse>();
            foreach(var feedbackItem in feedback)
            {
                var rs = _mapper.Map<GetFeedbackResponse>(feedbackItem);
                result.Add(rs);
            }
            return result;
        }

        public async Task<GetFeedbackResponse?> GetFeedbackById(int id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ArgumentNullException("Id can not be blank");
            }
            var feedback = await _feedbackRepository.getFeedBackById(id);
            if(feedback == null)
            {
                throw new ArgumentException("Feedback is not exist");
            }
            var result = _mapper.Map<GetFeedbackResponse>(feedback);
            return result;
        }

        public async Task<List<GetFeedbackResponse>> GetFeedbackByUserId(int userId)
        {
            if(string.IsNullOrWhiteSpace(userId.ToString()))
            {
                throw new ArgumentNullException("User id can not be blank");
            }
            var feedbacks = await _feedbackRepository.getFeedBackByUserId(userId);
            if(feedbacks.Count == 0)
            {
                throw new ArgumentException("This user do not have any feedbacks");
            }
            List<GetFeedbackResponse> result = new List<GetFeedbackResponse>();
            foreach (var feedbackItem in feedbacks)
            {
                var rs = _mapper.Map<GetFeedbackResponse>(feedbackItem);
                result.Add(rs);
            }
            return result;
        }

        public async Task<List<GetFeedbackAdminResponse>> GetAllFeedbackAdmin()
        {
            var feedback = await _feedbackRepository.getAllFeedBackRepoAdmin();
            if (feedback.Count == 0)
            {
                throw new ArgumentException("Do not exist any feedbacks");
            }
            List<GetFeedbackAdminResponse> result = new List<GetFeedbackAdminResponse>();
            foreach (var feedbackItem in feedback)
            {
                var rs = _mapper.Map<GetFeedbackAdminResponse>(feedbackItem);
                result.Add(rs);
            }
            return result;
        }

        public async Task<GetFeedbackResponse?> GetFeedbackByIdAdmin(int id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ArgumentNullException("Id can not be blank");
            }
            var feedback = await _feedbackRepository.getFeedBackByIdAdmin(id);
            if (feedback == null)
            {
                throw new ArgumentException("Feedback is not exist");
            }
            var result = _mapper.Map<GetFeedbackResponse>(feedback);
            return result;
        }

        public async Task<List<GetFeedbackResponse>> GetFeedbackByUserIdAdmin(int userId)
        {
            if (string.IsNullOrWhiteSpace(userId.ToString()))
            {
                throw new ArgumentNullException("User id can not be blank");
            }
            var feedbacks = await _feedbackRepository.getFeedBackByUserIdAdmin(userId);
            if (feedbacks.Count == 0)
            {
                throw new ArgumentException("This user do not have any feedbacks");
            }
            List<GetFeedbackResponse> result = new List<GetFeedbackResponse>();
            foreach (var feedbackItem in feedbacks)
            {
                var rs = _mapper.Map<GetFeedbackResponse>(feedbackItem);
                result.Add(rs);
            }
            return result;
        }

        public async Task<bool> CreateFeedback(CreateFeedbackRequest request)
        {
            if(string.IsNullOrWhiteSpace(request.UserId.ToString()))
            {
                throw new ArgumentNullException("User ID can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.RatingScore.ToString()))
            {
                throw new ArgumentNullException("Rating score can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.Description))
            {
                throw new ArgumentNullException("Description can not be blank");
            }

            var rs = _mapper.Map<Feedback>(request);
            rs.IsDeleted = false;
            return await _feedbackRepository.addFeedback(rs);
        }

        public async Task<bool> UpdateFeedback(int id, UpdateFeedbackRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RatingScore.ToString()))
            {
                throw new ArgumentNullException("Rating score can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.Description))
            {
                throw new ArgumentNullException("Description can not be blank");
            }

            var feedback = await _feedbackRepository.getFeedBackById(id);
            if(feedback == null)
            {
                throw new ArgumentException("Feedback is not exist");
            }
            //feedback = _mapper.Map<Feedback>(request);
            var rs = _mapper.Map(request, feedback);
            var result = await _feedbackRepository.updateFeedBack(rs);
            return result;
        }

        public async Task<bool> HardDeleteFeedBack(int id)
        {
            if(string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ArgumentNullException("Id can not be blank");
            }
            var feedback = await _feedbackRepository.getFeedBackById(id);
            if(feedback == null)
            {
                throw new ArgumentException("Feedback is not exist");
            }
            
            var result = await _feedbackRepository.hardDeleteFeedback(feedback);
            return result;
        }

        public async Task<bool> SoftDeleteFeedback(int id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ArgumentNullException("ID can not be blank");
            }
            var feedback = await _feedbackRepository.getFeedBackById(id);
            if (feedback == null)
            {
                throw new ArgumentException("Feedback is not exist");
            }
            feedback.IsDeleted = true;
            var result = await _feedbackRepository.updateFeedBack(feedback);
            return result;
        }
    }
}
