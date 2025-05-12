using System.ComponentModel;
using ShipmentData.Interfaces;

namespace ShipmentData.Models
{
    public class BaseProductModel
    {
        [DisplayName("No.")]
        public int No { get; set; }

        [DisplayName("SN")]
        public string SN { get; set; }

        [DisplayName("Temp.")]
        public string Temp { get; set; }

        [DisplayName("Test Result")]
        public string Result { get; set; }

        [DisplayName("Heater_ Resistance")]
        public decimal? Heater_Resistance { get; set; }

        [DisplayName("MPD_b (1V)")]
        public decimal? MPD_b_1V { get; set; }

        [DisplayName("MPD_t (1V)")]
        public decimal? MPD_t_1V { get; set; }

        [DisplayName("MPD_ Ld (1V)")]
        public decimal? MPD_Ld_1V { get; set; }

        [DisplayName("Ppi")]
        public decimal? Ppi { get; set; }

        [DisplayName("ER")]
        public decimal? ER { get; set; }
    }

    public class DenaliModel : BaseProductModel, IProductModel
    {
        [DisplayName("IL by PD")]
        public decimal? IL_by_PD { get; set; }

        [DisplayName("Loop1")]
        public decimal? Loop1 { get; set; }

        [DisplayName("Loop2")]
        public decimal? Loop2 { get; set; }

        [DisplayName("Unit\n  \nCH")]
        public string Channel { get; set; } // 手动设置 CH1-CH4  
    }

    public class WeserModel : BaseProductModel, IProductModel
    {
        [DisplayName("IL by Power")]
        public decimal? IL_by_PD { get; set; }

        [DisplayName("Unit\nCH")]
        public string Channel { get; set; } // 手动设置 CH1-CH4 
    }

    public class DenaliV3Model : BaseProductModel, IProductModel
    {
        [DisplayName("Unit\nCH")]
        public string Channel { get; set; } // 手动设置 CH1-CH4 

        [DisplayName("IL by Power")]
        public decimal? IL_by_PD { get; set; }

        [DisplayName("Loop 1")]
        public decimal? Loop1 { get; set; }

        [DisplayName("Loop 2")]
        public decimal? Loop2 { get; set; }
    }
}
