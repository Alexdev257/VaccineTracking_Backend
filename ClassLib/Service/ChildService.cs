using AutoMapper;
using ClassLib.DTO.Child;
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

        public async Task<List<GetChildResponse>> GetAllChildAsync()
        {
            var child = await _childRepository.GetAll();
            List<GetChildResponse> result = new List<GetChildResponse>();
            foreach (var childItem in child)
            {
                var res = _mapper.Map<GetChildResponse>(childItem);
                result.Add(res);
            }
            if(result.Count == 0)
            {
                throw new ArgumentException("No child in the system");
            }
            return result;
        }

        //public async Task<List<GetChildResponse>> GetAllChildForAdminAsync()
        //{
        //    var child = await _childRepository.GetAllForAdmin();
        //    List<GetChildResponse> result = new List<GetChildResponse>();
        //    foreach (var childItem in child)
        //    {
        //        var res = _mapper.Map<GetChildResponse>(childItem);
        //        result.Add(res);
        //    }
        //    if (result.Count == 0)
        //    {
        //        throw new ArgumentException("No child in the system");
        //    }
        //    return result;
        //}

        public async Task<GetChildResponse?> GetChildByIdAsync(int id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                throw new ArgumentNullException("ID can not be blank");
            }
            var child = await _childRepository.GetChildById(id);
            if(child == null)
            {
                throw new ArgumentException("No child in the system");
            }
            var res = _mapper.Map<GetChildResponse>(child);
            return res;
        }

        public async Task<List<GetChildResponse>> GetAllChildByParentIdAsync(int parentId)
        {
            if (string.IsNullOrEmpty(parentId.ToString()))
            {
                throw new ArgumentNullException("Parent ID can not be blank");
            }
            var child = await _childRepository.getAllChildByParentsId(parentId);
            if(child.Count == 0)
            {
                throw new ArgumentException("No child was added by this parent");
            }
            List<GetChildResponse> result = new List<GetChildResponse>();
            foreach (var childItem in child)
            {
                var res = _mapper.Map<GetChildResponse?>(childItem);
                result.Add(res);
            }
            return result;

                
            
        }

        public async Task<bool> CreateChildAsync(CreateChildRequest request)
        {
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
                var child = _mapper.Map<Child>(request);
                child.Status = "Active";
                child.CreatedAt = Helpers.TimeProvider.GetVietnamNow();
                return await _childRepository.CreateChild(child);
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
                throw new ArgumentException("No child in the system");
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
            var child = await _childRepository.GetChildByIdHardDelete(id);
            if (child == null)
            {
                throw new ArgumentException("No child in the system");
            }
            return await _childRepository.HardDeleteChild(child);
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
                throw new ArgumentException("No child in the system");
            }
            child.IsDeleted = true;
            return await _childRepository.UpdateChild(child);
        }
    }
}
