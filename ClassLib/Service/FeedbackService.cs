using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<List<GetFeedbackResponse>> GetFeedbackById(int id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ArgumentNullException("Id can not be blank");
            }
            var feedback = await _feedbackRepository.getFeedBackById(id);
            if(feedback.Count == 0)
            {
                throw new ArgumentException("Feedback is not exist");
            }
            List<GetFeedbackResponse> result = new List<GetFeedbackResponse>();
            foreach (var feedbackItem in feedback)
            {
                var rs = _mapper.Map<GetFeedbackResponse>(feedback);
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
            //rs.IsDeleted = false;
            return await _feedbackRepository.addFeedback(rs);
        }
    }
}
