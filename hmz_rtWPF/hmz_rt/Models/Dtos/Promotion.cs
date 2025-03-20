using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hmz_rt.Models.Dtos
{
    public class Promotion
    {
        public int PromotionId { get; set; }
        public string PromotionName { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? DiscountPercentage { get; set; }
        public string TermsConditions { get; set; }
        public string Status { get; set; }
        public int? RoomId { get; set; }
        public DateTime? DateAdded { get; set; }

        public override string ToString()
        {
            return $"{PromotionName} - {DiscountPercentage}% kedvezmény, {StartDate?.ToString("yyyy.MM.dd")} - {EndDate?.ToString("yyyy.MM.dd")}";
        }
    }

    public class PromotionCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? DiscountPercentage { get; set; }
        public string TermsConditions { get; set; }
        public string Status { get; set; }
        public int? RoomId { get; set; }
    }

    public class PromotionUpdateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? DiscountPercentage { get; set; }
        public string TermsConditions { get; set; }
        public string Status { get; set; }
        public int? RoomId { get; set; }
    }
}
