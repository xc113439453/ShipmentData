using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentData.Interfaces
{
    public interface ISummary
    {
        public string SN { get; set; }
        public string Channel {  get; set; }
        public decimal? Heater_Resistance { get; set; }
        public decimal? MPD_b_1V { get; set; }
        public decimal? MPD_t_1V { get; set; }
        public decimal? MPD_Ld_1V { get; set; }
        public decimal? Ppi { get; set; }
        public decimal? ER { get; set; }
        public decimal? IL_by_PD { get; set; }
    }
}
