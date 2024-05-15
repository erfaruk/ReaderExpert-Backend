using ReaderAPI.Models.Database;

namespace ReaderAPI.Models
{
    public class KeyAdd
    {

        public string SecurityId { get; set; } = null!;

        public string? SerialNo { get; set; }

        public string? OrderNo { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndDate { get; set; }

        public int? UserId { get; set; }
    
        }
}
