using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributionService
{
    public class MessageStructure
    {
        public Guid MessageId { get; set; }
        public string? Message { get; set; }
        public string? ServiceType { get; set; }
    }
}
