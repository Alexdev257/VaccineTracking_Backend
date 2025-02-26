using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ClassLib.Models;
using ClassLib.Repositories;

namespace ClassLib.Service
{
    public class ChildService
    {
        private readonly ChildRepository _childRepository;
        private readonly IMapper _mapper;
        public ChildService(ChildRepository childRepository, IMapper mapper)
        {
            _childRepository = childRepository;
            _mapper = mapper;
        }

        public async Task<List<Child>> GetAllChildAsync()
        {
            return await _childRepository.GetAll();
        }

        public async Task<Child?> GetChildByIdAsync(int id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                throw new ArgumentNullException("ID can not be blank");
            }
            try
            {
                var child = await _childRepository.GetById(id);
                return child;
            }
            catch (Exception ex)
            {
                throw new Exception("No child in the system");
            }
        }

        public async Task<List<Child>> GetAllChildByParentIdAsync(int parentId)
        {
            if (string.IsNullOrEmpty(parentId.ToString()))
            {
                throw new ArgumentNullException("Parent ID can not be blank");
            }
            try
            {
                var child = await _childRepository.getAllChildByParentsId(parentId);
                return child;
            }
            catch (Exception ex)
            {
                throw new Exception("No child in the system");
            }
        }

    }
}
