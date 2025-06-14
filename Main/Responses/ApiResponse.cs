using Main.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Responses
{
    public class ApiResponse<T>  where T : class
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Location { get; set; }
        public Dictionary<string, List<string>>? Errors { get; set; }
        public NotificationType NotificationType { get; set; }
        public int? TotalCount { get; set; }
    }
}
