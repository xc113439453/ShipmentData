using System.ComponentModel;
using CsvHelper.Configuration.Attributes;
using ShipmentData.Interfaces;

namespace ShipmentData.Models;

public class DenaliV3PICSummaryModel : ISummaryModel
{
    [DisplayName("芯片编号")]
    public string SN { get; set; }

    [DisplayName("通道")]
    public string Channel { get; set; }

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

    [DisplayName("IL_by_pd_peak(dB)")]
    public decimal? IL_by_PD { get; set; }

    [DisplayName("Loop1(dB)")]
    public decimal? Loop { get; set; }
}
