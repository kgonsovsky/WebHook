using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourOperator.Api.Model
{
    public class KafkaOpts
    {
        public string BootstrapServers { get; set; }
        public string ClientId { get; set; }
        public string GroupId { get; set; }
    }
}
