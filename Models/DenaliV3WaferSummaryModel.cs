using System.ComponentModel;
using CsvHelper.Configuration.Attributes;
using ShipmentData.Interfaces;

namespace ShipmentData.Models;

public class DenaliV3WaferSummaryModel : ISummaryModel
{
    [Name("PIC S/N")]
    public string SN { get; set; }

    [Name("Channel")]
    public string Channel { get; set; }

    [Name("PIV Heater Resistance(ohm)")]
    public decimal? Heater_Resistance { get; set; }

    [Name("DK MPD-b(nA)@1.0")]
    public decimal? MPD_b_1V { get; set; }

    [Name("DK MPD-t(nA)@1.0")]
    public decimal? MPD_t_1V { get; set; }

    [Name("DK MPD-LD(nA)@1.0")]
    public decimal? MPD_Ld_1V { get; set; }

    [Name("PIV Ppi Comsuption(mW)")]
    public decimal? Ppi { get; set; }

    [Name("PIV ER(dB)")]
    public decimal? ER { get; set; }

    [Name("Bias-IL Full Device On-chip IL-ave(dB)")]
    public decimal? IL_by_Power { get; set; }
}
