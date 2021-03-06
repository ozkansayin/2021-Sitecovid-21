using System;

namespace Feature.SmartNavigation.Models
{
    public class EntryModel
    {
        public Guid FromId { get; set; }
        public Guid ToId { get; set; }
        public int HitPoint_Level1 { get; set; }
        public int HitPoint_Level2 { get; set; }
        public int HitPoint_Level3 { get; set; }
        public decimal CalculatedHitPoint { get; set; }
    }
}
