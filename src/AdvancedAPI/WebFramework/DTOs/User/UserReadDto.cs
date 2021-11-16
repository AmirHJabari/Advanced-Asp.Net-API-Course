using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace WebFramework.DTOs
{
    public class UserReadDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public DateTimeOffset LoginDate { get; set; }
        public DateTimeOffset LastActivityDate { get; set; }
    }
}
