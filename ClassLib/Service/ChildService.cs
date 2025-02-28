using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ClassLib.DTO.Child;
using ClassLib.Models;
using ClassLib.Repositories;
using Microsoft.IdentityModel.Tokens;

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
                var child = await _childRepository.GetChildById(id);
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

        public async Task<bool> CreateChildAsync(CreateChildRequest request)
        {
            //    public int ParentId { get; set; }

            //public string Name { get; set; } = null!;

            //public DateTime DateOfBirth { get; set; }

            //public int Gender { get; set; }
            bool check = false;
            if (string.IsNullOrEmpty(request.ParentId.ToString()))
            {
                throw new ArgumentNullException("Parent ID can not be blank");
            }
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new ArgumentNullException("Name can not be blank");
            }
            if (string.IsNullOrEmpty(request.DateOfBirth.ToString()))
            {
                throw new ArgumentNullException("Date of birth can not be blank");
            }
            if (string.IsNullOrEmpty(request.Gender.ToString()))
            {
                throw new ArgumentNullException("Gendere can not be blank");
            }

            try
            {
                var child = _mapper.Map<Child>(request);
                child.Status = "Active";
                child.CreatedAt = DateTime.Now;
                return await _childRepository.CreateChild(child);
            }
            catch (Exception ex)
            {
                throw new Exception("Can not create child");
                return check;
            }

        }

        public async Task<bool> UpdateChildAsync(int id, UpdateChildRequest request)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ArgumentNullException("ID can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.ParentId.ToString()))
            {
                throw new ArgumentNullException("Parent ID can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentNullException("Name can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.DateOfBirth.ToString()))
            {
                throw new ArgumentNullException("Date of birth can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.Gender.ToString()))
            {
                throw new ArgumentNullException("Gender can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.Status))
            {
                throw new ArgumentNullException("Status can not be blank");
            }
            var child = await _childRepository.GetChildById(id);
            if (child == null)
            {
                throw new Exception("No child in the system");
            }
            //var updateChild = _mapper.Map<Child>(request);
            //updateChild.Id = id;
            //updateChild.Status = "Active";
            //updateChild.CreatedAt = DateTime.Now;
            child.ParentId = request.ParentId;
            child.Name = request.Name;
            child.DateOfBirth = request.DateOfBirth;
            child.Gender = request.Gender;
            child.Status = request.Status;

            return await _childRepository.UpdateChild(child);
        }

        public async Task<bool> HardDeleteChildAsync(int id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ArgumentNullException("ID can not be blank");
            }
            var child = await _childRepository.GetChildById(id);
            if (child == null)
            {
                throw new Exception("No child in the system");
            }
            return await _childRepository.HardDeleteChild(id);
        }

        public async Task<bool> SoftDeleteChildAsync(int id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ArgumentNullException("ID can not be blank");
            }
            var child = await _childRepository.GetChildById(id);
            if (child == null)
            {
                throw new Exception("No child in the system");
            }
            //child.IsDeleeted = true;
            return await _childRepository.UpdateChild(child);
        }
    }
}
