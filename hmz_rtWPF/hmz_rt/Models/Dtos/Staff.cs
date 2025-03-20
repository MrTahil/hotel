using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hmz_rt.Models.Dtos
{
    public class Staff
    {
        public int StaffId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Position { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? DateHired { get; set; }
        public string Status { get; set; }
        public string Department { get; set; }

        public override string ToString()
        {
            return $"{LastName} {FirstName} - {Position}, {Department}";
        }
    }

    public class NewStaffDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Position { get; set; }
        public decimal? Salary { get; set; }
        public string Status { get; set; }
        public string Departmen { get; set; }
    }

    public class UpdateStaffDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Position { get; set; }
        public decimal? Salary { get; set; }
        public string Department { get; set; }
        public string Status { get; set; }
    }
}
