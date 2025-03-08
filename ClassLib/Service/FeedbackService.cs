using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ClassLib.DTO.Feedback;
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

        public async Task<GetFeedbackResponse> getFeedbackById(int id)
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
    }
}
