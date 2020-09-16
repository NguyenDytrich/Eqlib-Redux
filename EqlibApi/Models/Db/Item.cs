using EqlibApi.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EqlibApi.Models
{
    public class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public EAvailability Availability { get; set; }
        public bool OffsiteEligable { get; set; }
        public DateTime DateAcquired { get; set; }
        public DateTime LastInspected { get; set; }
        public ECondition Condition { get; set; }
        public string Notes { get; set; }

        public int ItemGroupId { get; set; }
        public ItemGroup ItemGroup { get; set; }
    }
}
