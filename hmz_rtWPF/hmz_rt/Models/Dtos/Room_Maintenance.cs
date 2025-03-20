using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hmz_rt.Models.Dtos
{
    public class RoomMaintenanceViewModel
    {
        public int MaintenanceId { get; set; }
        public DateTime? MaintenanceDate { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? DateReported { get; set; }
        public DateTime? ResolutionDate { get; set; }
        public decimal? Cost { get; set; }
        public string Notes { get; set; }
        public int? RoomId { get; set; }
        public string RoomNumber { get; set; }
        public int? StaffId { get; set; }  // Ezt a tulajdonságot kell hozzáadni
    }


    // Karbantartási kérelem létrehozási DTO
    public class MaintenanceCreateDto
    {
        public DateTime? MaintenanceDate { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public int? RoomId { get; set; }
    }
    public class MaintanceUpdate
    {
        public DateTime? ResolutionDate { get; set; }
        public string Status { get; set; }
        public decimal? Cost { get; set; }
        public string Notes { get; set; }
        public int? StaffId { get; set; }
    }

    public class Roommaintenance
    {
        public int MaintenanceId { get; set; }
        public DateTime? MaintenanceDate { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? DateReported { get; set; }
        public DateTime? ResolutionDate { get; set; }
        public decimal? Cost { get; set; }
        public string Notes { get; set; }
        public int? RoomId { get; set; }
        public int? StaffId { get; set; }  // Új tulajdonság
    }
}
