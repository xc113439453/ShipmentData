using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentData.Interfaces
{
    public interface IProduct: ISummary
    {
        public int No { get; set; } 
    }
}
