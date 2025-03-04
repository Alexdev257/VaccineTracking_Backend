using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.DTO.Child
{
    public class GetChildResponse
    {
        public int Id { get; set; }

        public int ParentId { get; set; }

        public string Name { get; set; } = null!;

        public DateTime DateOfBirth { get; set; }

        public int Gender { get; set; }

        public string Status { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
