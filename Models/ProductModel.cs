using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using ShipmentData.Attributes;
using ShipmentData.Interfaces;

namespace ShipmentData.Models
{
    public class BaseProductModel
    {
        [ExcludeFromFilter]
        [DisplayName("No.")]
        public int No { get; set; }

        [DisplayName("SN")]
        [ExcludeFromFilter]
        public string SN { get; set; }

        [DisplayName("Temp.")]
        [ExcludeFromFilter]
        public string Temp { get; set; } = "RT";

        [DisplayName("Test Result")]
        [ExcludeFromFilter]
        public string Result { get; set; } = "Pass";

        [DisplayName("Heater_Resistance")]
        public decimal? Heater_Resistance { get; set; }

        [DisplayName("MPD_b (1V)")]
        public decimal? MPD_b_1V { get; set; }

        [DisplayName("MPD_t (1V)")]
        public decimal? MPD_t_1V { get; set; }

        [DisplayName("MPD_LD (1V)")]
        public decimal? MPD_Ld_1V { get; set; }

        [DisplayName("Ppi")]
        public decimal? Ppi { get; set; }

        [DisplayName("ER")]
        public decimal? ER { get; set; }
    }



    public class DenaliV3PICModel : BaseProductModel, IProductModel
    {
        public DenaliV3PICModel()
        {

        }
        [DisplayName("Unit\nCH")]
        [ExcludeFromFilter]
        public string Channel { get; set; } // 手动设置 CH1-CH4 

        [DisplayName("IL by PD")]
        public decimal? IL_by_PD { get; set; }

        [DisplayName("Loop")]
        public decimal? Loop { get; set; }
    }

    public class DenaliV3WaferModel : BaseProductModel, IProductModel
    {
        public DenaliV3WaferModel()
        {

        }
        [DisplayName("Unit\nCH")]
        [ExcludeFromFilter]
        public string Channel { get; set; } // 手动设置 CH1-CH4 

        [DisplayName("IL by Power")]
        public decimal? IL_by_Power { get; set; }

    }
}
