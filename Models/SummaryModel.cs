using System.ComponentModel;
using ShipmentData.Interfaces;

namespace ShipmentData.Models
{
    public class WeserSummaryModel : ISummary
    {
        [DisplayName("PIC_SN")]
        public string SN { get; set; }

        [DisplayName("Channel")]
        public string Channel { get; set; }

        [DisplayName("IL_by_pd_peak(dB)")]
        public decimal? IL_by_PD { get; set; }

        [DisplayName("PIV_Heater_Resistance(ohm)")]
        public decimal? Heater_Resistance { get; set; }

        [DisplayName("DK_MPD-b(nA)@1.0")]
        public decimal? MPD_b_1V { get; set; }

        [DisplayName("DK_MPD-t(nA)@1.0")]
        public decimal? MPD_t_1V { get; set; }

        [DisplayName("DK_MPD-LD(nA)@1.0")]
        public decimal? MPD_Ld_1V { get; set; }

        [DisplayName("PIV_Ppi_Comsuption(mW)")]
        public decimal? Ppi { get; set; }

        [DisplayName("PIV_ER(dB)")]
        public decimal? ER { get; set; }
    }

    public class DenaliSummaryModel : ISummary
    {
        [DisplayName("芯片编号")]
        public string SN { get; set; }

        [DisplayName("通道")]
        public string Channel { get; set; }

        [DisplayName("IL_by_pd_peak(dB)")]
        public decimal? IL_by_PD { get; set; }

        [DisplayName("Loop1(dB)")]
        public decimal? Loop1 { get; set; }

        [DisplayName("Loop2(dB)")]
        public decimal? Loop2 { get; set; }

        [DisplayName("HT_R(ohm)")]
        public decimal? Heater_Resistance { get; set; }

        [DisplayName("MPDb/1V(nA)")]
        public decimal? MPD_b_1V { get; set; }

        [DisplayName("MPDt/1V(nA)")]
        public decimal? MPD_t_1V { get; set; }

        [DisplayName("MPDLd/1V(nA)")]
        public decimal? MPD_Ld_1V { get; set; }

        [DisplayName("Ppi1(mW)")]
        public decimal? Ppi { get; set; }

        [DisplayName("ER(dB)")]
        public decimal? ER { get; set; }
        
    }
}
