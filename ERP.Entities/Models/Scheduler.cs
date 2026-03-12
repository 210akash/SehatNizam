namespace ERP.Entities.Models
{
    public class Scheduler : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
    }
}
