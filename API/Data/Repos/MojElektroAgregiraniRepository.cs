using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using API.Entities;
using API.Entities.MojElektro;
using API.Extensions;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;



namespace API.Data.Repos;

public class TimeSeriesLabelConfiguration
{
    public IList<string> MonthLabels { get; set; } = new ReadOnlyCollection<string>
        (new List<string> { "jan", "feb", "mar", "apr", "maj", "junij", "julij", "avg", "sep", "okt", "nov", "dec" });

    public IList<string> DayLabels { get; set; } = new ReadOnlyCollection<string>
        (new List<string> { "nedelja", "ponedeljek", "torek", "sreda", "četrtek", "petek", "sobota" });

    public IList<string> HourLabels { get; set; } = new ReadOnlyCollection<string>
        (new List<string> { "00h", "01h", "02h", "03h", "04h", "05h", "06h", "07h", "08h", "09h",
                                "10h", "11h", "12h", "13h", "14h", "15h", "16h", "17h", "18h", "19h",
                                "20h", "21h", "22h", "23h" });
}


public class MojElektroAgregiraniRepository(DataContext context) : IMojElektroAgregiraniRepository
{

    private readonly CultureInfo? _currentCulture;
    private readonly TimeSeriesLabelConfiguration _labelConfig = new TimeSeriesLabelConfiguration();

    public int[] HighSeasonMonths { get; set; } = [1, 2, 11, 12];



    public async Task<BaseChartDTO<decimal>> Get_15MinMeritvePoLetih(HttpParametri aggrInDatumi)
    {
        Calendar calendar = (_currentCulture ?? CultureInfo.CurrentCulture).Calendar;




        DateTime datumOD = new DateTime(aggrInDatumi.letoOD, aggrInDatumi.mesecOD, 1);
        DateTime datumDO = new DateTime(aggrInDatumi.letoDO, aggrInDatumi.mesecDO, 1);

        int steviloMesecev = ((datumDO.Year - datumOD.Year) * 12) + datumDO.Month - datumOD.Month;

        int steviloDnevov = (datumDO - datumOD).Days;

        Dictionary<int, List<AggregatedData>>? aggregatedData;

        // pridobimo številke MojElektro merilnih mest za ta javni objekt
        var mojEleMermesta = PridobiMojElektroMerilnaMesta(aggrInDatumi.idJavnegaObjekta);
        int steviloMojEleMerilnihMest = mojEleMermesta.Count;
        var statuses = new List<int>();
        foreach (var kljuc in mojEleMermesta.Keys)
            statuses.Add(kljuc);

        switch (aggrInDatumi.aggregation)
        {


            // ----------------------------------------------------------------------------------  PoLetihPoMesecihPoBlokih 

            case "PoLetihPoMesecihPoBlokih":
                {
                    if (aggrInDatumi.sifraEnergijaMoc == "EnergijaAPlus")
                        // napolnimo variablo aggregatedData - agregiramo po mesecih in po blokih

                        aggregatedData = await GetAgregatedChartDataForPeriod(statuses,
                            datumOD, datumDO,
                            obj => new AggregatedData { Group = Convert.ToInt32(obj.LetoMesecBlok), Sum = obj.Energija_A_plus });
                    else if (aggrInDatumi.sifraEnergijaMoc == "PrejetaDelovnaMoc")
                        aggregatedData = await GetAgregatedChartDataForPeriod(statuses,
                            datumOD, datumDO,
                            obj => new AggregatedData { Group = Convert.ToInt32(obj.LetoMesecBlok), Sum = obj.PrejetaDelovnaMoc });
                    else throw new ArgumentException($"Invalid sifraEnergijaMoc value: {aggrInDatumi.sifraEnergijaMoc} in PoLetihPoMesecihPoBlokih aggregation");


                    DateTime start = datumOD;
                    var intervali = Enumerable.Range(0, steviloMesecev).Select(offset => start.AddMonths(offset)).ToArray();
                    var intIntervali = new List<int>();
                    foreach (var inter in intervali)
                    {
                        var a = Convert.ToInt32(inter.Year.ToString("0000") + inter.Month.ToString("00"));
                        intIntervali.Add(a);
                    }

                    var keys = new List<int>(aggregatedData.Keys);
                    foreach (var key in keys)
                    {
                        aggregatedData[key] = aggregatedData[key].ExtendWithEmptyData(intIntervali).ToList();
                    }



                    var stats = new BaseChartDTO<decimal>
                    {
                        AxisXLabels = new List<string>(),
                        LegendaOriginal = new List<string>(),
                        Lines = new List<ChartDataDTO<decimal>>()
                    };

                    if (aggregatedData != null)
                    {
                        foreach (var status in aggregatedData)
                        {
                            stats.AxisXLabels = status.Value.Select(x => FormatAxisXLabels(x.Group, aggrInDatumi.aggregation)).Distinct();
                            stats.ChartLabel = null;

                            for (int blok = 1; blok <= 5; blok++)
                            {

                                var blokIntervali = new List<int>();
                                foreach (var inter in intervali)
                                {
                                    var a = Convert.ToInt32(inter.Year.ToString("0000") + inter.Month.ToString("00") + blok.ToString("00"));
                                    blokIntervali.Add(a);
                                }

                                var jeBlok = status.Value.Where(x =>
                                    x.Group.ToString().Length >= 8 &&
                                    x.Group.ToString().Substring(6, 2) == blok.ToString("00")
                                );
                                if (jeBlok != null)
                                {
                                    jeBlok = jeBlok.ExtendWithEmptyData(blokIntervali).ToList();


                                    stats.Lines.Add(new ChartDataDTO<decimal>
                                    {
                                        Type = mojEleMermesta[status.Key].ToString() + blok.ToString("00"),
                                        Values = jeBlok.Select(x => (decimal)(x.Sum ?? 0)).ToArray(),
                                    });


                                }
                                else continue;

                            }

                            var _legendaOriginal = new List<string>();
                            foreach (var line in stats.Lines)
                            {
                                if (!string.IsNullOrEmpty(line.TypeOriginal) && !_legendaOriginal.Contains(line.TypeOriginal))
                                    _legendaOriginal.Add(line.TypeOriginal);
                            }
                            stats.LegendaOriginal = _legendaOriginal;

                        }
                    }

                    return stats;

                }


            // ---------------------------------------------------------------------------------- // če je PoLetihPoMesecih razrežemo po letih 

            case "PoLetihPoMesecih":
                {

                    if (aggrInDatumi.sifraEnergijaMoc == "EnergijaAPlus")
                        // napolnimo variablo aggregatedData agregiramo po mesecih
                        aggregatedData = await GetAgregatedChartDataForPeriod(statuses,
                            datumOD, datumDO,
                            obj => new AggregatedData { Group = Convert.ToInt32(obj.LetoMesec), Sum = obj.Energija_A_plus });
                    else if (aggrInDatumi.sifraEnergijaMoc == "PrejetaDelovnaMoc")
                        aggregatedData = await GetAgregatedChartDataForPeriod(statuses,
                            datumOD, datumDO,
                            obj => new AggregatedData { Group = Convert.ToInt32(obj.LetoMesec), Sum = obj.PrejetaDelovnaMoc });
                    else throw new ArgumentException($"Invalid sifraEnergijaMoc value: {aggrInDatumi.sifraEnergijaMoc} in PoLetihPoMesecih aggregation");



                    DateTime start = datumOD;

                    // vzamemo tudi mesece  od januarja do DatumOd.meseci, da je graf leta v redu

                    DateTime start1 = new DateTime(start.Year, 1, 1);
                    int steviloMesecev1 = steviloMesecev + start.Month - 1;

                    // vsi DateTime prvega v mesecu  
                    var intervali = Enumerable.Range(0, steviloMesecev1).Select(offset => start1.AddMonths(offset)).ToArray();       // !!!!

                    var intIntervali = new List<int>();
                    foreach (var inter in intervali)
                    {
                        var a = Convert.ToInt32(inter.Year.ToString("0000") + inter.Month.ToString("00"));
                        intIntervali.Add(a);
                    }

                    if (aggregatedData != null)
                    {
                        var keys = new List<int>(aggregatedData.Keys);
                        foreach (var key in keys)
                        {
                            aggregatedData[key] = aggregatedData[key].ExtendWithEmptyData(intIntervali).ToList();
                        }
                    }

                    // razrežemo po letih
                    var stats = new BaseChartDTO<decimal>
                    {
                        AxisXLabels = new List<string>(),
                        LegendaOriginal = new List<string>(),
                        Lines = new List<ChartDataDTO<decimal>>()
                    };

                    if (aggregatedData != null)
                    {
                        foreach (var vsaLetaLine in aggregatedData)
                        {
                            int index = 0;          // !!!

                            for (var i = datumOD.Year; i < datumDO.Year; i++)
                            {
                                var apom = vsaLetaLine.Value[index].Group.ToString().Substring(0, 4);
                                if (apom == i.ToString())
                                {
                                    var enoLeto = vsaLetaLine.Value.Where(x => x.Group.ToString().Substring(0, 4) == apom);

                                    stats.Lines.Add(new ChartDataDTO<decimal>
                                    {
                                        Type = i.ToString() + "-" + mojEleMermesta[vsaLetaLine.Key].ToString(),
                                        TypeOriginal = mojEleMermesta[vsaLetaLine.Key].ToString(),
                                        Values = enoLeto.Select(x => (decimal)(x.Sum ?? 0)).ToArray()
                                    });
                                }
                                // ne vzamemo prvi record (index=0), ker ima še podatke iz prejšnjega timestamp
                                index = index + 12;
                                if (index >= vsaLetaLine.Value.Count)
                                    index = index - 1;

                            }
                        }
                    }
                    stats.AxisXLabels = _labelConfig?.MonthLabels ?? new List<string>();
                    stats.ChartLabel = null;

                    var _legendaOriginal = new List<string>();
                    foreach (var line in stats.Lines)
                    {
                        if (!string.IsNullOrEmpty(line.TypeOriginal) && !_legendaOriginal.Contains(line.TypeOriginal))
                            _legendaOriginal.Add(line.TypeOriginal);
                    }
                    stats.LegendaOriginal = _legendaOriginal;

                    return stats;
                }

            // -----------------------------------------agregiramo po dnevih nato pripravimo za tedenski graf
            case "PoLetihPoTednihPoDnevih":
                {
                    if (aggrInDatumi.sifraEnergijaMoc == "EnergijaAPlus")
                        // napolnimo variablo aggregatedData agregiramo po mesecih
                        aggregatedData = await GetAgregatedChartDataForPeriod(statuses,
                            datumOD, datumDO,
                            obj => new AggregatedData { Group = Convert.ToInt32(obj.LetoTedenDan), Sum = obj.Energija_A_plus });
                    else if (aggrInDatumi.sifraEnergijaMoc == "PrejetaDelovnaMoc")
                        aggregatedData = await GetAgregatedChartDataForPeriod(statuses,
                            datumOD, datumDO,
                            obj => new AggregatedData { Group = Convert.ToInt32(obj.LetoTedenDan), Sum = obj.PrejetaDelovnaMoc });
                    else throw new ArgumentException($"Invalid sifraEnergijaMoc value: {aggrInDatumi.sifraEnergijaMoc} in PoLetihPoTednihPoDnevih aggregation");



                    // struktura za grafe
                    // za PoLetihPoTednihPoDnevih razrežemo po tednih
                    var stats = new BaseChartDTO<decimal>
                    {
                        AxisXLabels = new List<string>(),
                        LegendaOriginal = new List<string>(),
                        Lines = new List<ChartDataDTO<decimal>>()
                    };

                    // po posameznih merilnih mestih
                    foreach (var vsiTedniLine in aggregatedData)
                    {
                        if (vsiTedniLine.Value.Count > 0)
                        {
                            // najprej po letih
                            int indexDnivLetu = 0;        // za letoOD = 0 ..

                            for (int indexLeta = datumOD.Year; indexLeta < datumDO.Year; indexLeta++)
                            {
                                // int leto_pomInt = Convert.ToInt32(indexLeta);
                                int steviloDniVLetu = calendar.GetDaysInYear(indexLeta);

                                // ugotovimo stevilko tedna za zadnji dan v tem letu, kar je tudi steviloTednov
                                int steviloTednov = calendar.GetWeekOfYear(new DateTime(indexLeta, 12, 31), CalendarWeekRule.FirstDay, DayOfWeek.Monday);

                                for (int iTeden = 1; iTeden <= steviloTednov; iTeden++)
                                {
                                    string leto_teden = indexLeta + iTeden.ToString("00");
                                    var enTedenLine = vsiTedniLine.Value.Where(x => x.Group.ToString().Substring(0, 6) == leto_teden);
                                    stats.Lines.Add(new ChartDataDTO<decimal>
                                    {
                                        Type = indexLeta + "-" + iTeden.ToString("00"),
                                        TypeOriginal = mojEleMermesta[vsiTedniLine.Key].ToString(),
                                        Values = enTedenLine.Select(x => (decimal)x.Sum.GetValueOrDefault())
                                    });
                                }
                                indexDnivLetu = indexDnivLetu + steviloDniVLetu;
                                if (indexDnivLetu >= steviloDnevov - 1) break;
                            }
                        }
                    }

                    stats.AxisXLabels = _labelConfig?.DayLabels ?? new List<string>();
                    stats.ChartLabel = null;

                    var _legendaOriginal = new List<string>();
                    foreach (var line in stats.Lines)
                    {
                        if (line != null
                            && line.TypeOriginal != null
                            && !_legendaOriginal.Contains(line.TypeOriginal))
                            _legendaOriginal.Add(line.TypeOriginal);
                    }
                    stats.LegendaOriginal = _legendaOriginal;

                    return stats;

                }

            // -----------------------------------------agregiramo po urah nato pripravimo za dnevni graf
            case "PoLetihPoDnevihPoUrah":
                {
                    if (aggrInDatumi.sifraEnergijaMoc == "EnergijaAPlus")
                        // napolnimo variablo aggregatedData agregiramo po mesecih
                        aggregatedData = await GetAgregatedChartDataForPeriod(statuses,
                            datumOD, datumDO,
                            obj => new AggregatedData { Group = Convert.ToInt32(obj.LetoDanUra), Sum = obj.Energija_A_plus });
                    else if (aggrInDatumi.sifraEnergijaMoc == "PrejetaDelovnaMoc")
                        aggregatedData = await GetAgregatedChartDataForPeriod(statuses,
                            datumOD, datumDO,
                            obj => new AggregatedData { Group = Convert.ToInt32(obj.LetoDanUra), Sum = obj.PrejetaDelovnaMoc });
                    else throw new ArgumentException($"Invalid sifraEnergijaMoc value: {aggrInDatumi.sifraEnergijaMoc} in PoLetihPoDnevihPoUrah aggregation");



                    // struktura za grafe
                    // za PoLetihPoDnevihPoUrah razrežemo po dnevih
                    var stats = new BaseChartDTO<decimal>
                    {
                        AxisXLabels = new List<string>(),
                        LegendaOriginal = new List<string>(),
                        Lines = new List<ChartDataDTO<decimal>>()
                    };

                    // po posameznih merilnih mestih
                    foreach (var vsiDneviLine in aggregatedData)
                    {
                        if (vsiDneviLine.Value.Count > 0)
                        {
                            // najprej po letih
                            int indexDnivLetu = 0;        // za letoOD = 0 ..

                            for (int indexLeta = datumOD.Year; indexLeta < datumDO.Year; indexLeta++)
                            {
                                // ekstrahiramo leto
                                // string leto_pom = vsiDneviLine.Value[indexDnivLetu].Group.ToString().Substring(0, 4);
                                // int leto_pomInt = Convert.ToInt32(leto_pom);
                                string leto_pom = indexLeta.ToString();
                                int steviloDniVLetu = calendar.GetDaysInYear(indexLeta);

                                // ugotovimo stevilko tedna za zadnji dan v tem letu, kar je tudi steviloTednov
                                int steviloTednov = calendar.GetWeekOfYear(new DateTime(indexLeta, 12, 31), CalendarWeekRule.FirstDay, DayOfWeek.Monday);



                                for (int iDan = 1; iDan <= steviloDniVLetu; iDan++)
                                {
                                    string leto_dan = leto_pom + iDan.ToString("0000");
                                    var enDanLine = vsiDneviLine.Value.Where(x => x.Group.ToString().Substring(0, 8) == leto_dan);
                                    stats.Lines.Add(new ChartDataDTO<decimal>
                                    {
                                        Type = leto_pom + "-" + iDan.ToString("0000"),
                                        TypeOriginal = mojEleMermesta[vsiDneviLine.Key].ToString(),
                                        Values = enDanLine.Select(x => (decimal)x.Sum.GetValueOrDefault())
                                    });
                                }
                                indexDnivLetu = indexDnivLetu + steviloDniVLetu;
                                if (indexDnivLetu >= steviloDnevov - 1) break;
                            }
                        }
                    }

                    stats.AxisXLabels = _labelConfig.HourLabels;
                    stats.ChartLabel = null;

                    var _legendaOriginal = new List<string>();
                    foreach (var line in stats.Lines)
                    {
                        if (line != null
                            && line.TypeOriginal != null
                            && !_legendaOriginal.Contains(line.TypeOriginal))
                            _legendaOriginal.Add(line.TypeOriginal);

                    }
                    stats.LegendaOriginal = _legendaOriginal;

                    return stats;

                }


            default: return new BaseChartDTO<decimal> { Lines = new List<ChartDataDTO<decimal>>() };
        }
    }

    // Pridobimo vsa MojElektroMerilnaMesta za jObjekt
    public Dictionary<int, string> PridobiMojElektroMerilnaMesta(int idJavnegaObjekta)
    {
        var seznam = new Dictionary<int, string>();
        var jObjekt = context.Stavbe?.Include(m => m.MojElektroMerilnaMesta).FirstOrDefault(x => x.Id == idJavnegaObjekta);

        if (jObjekt?.MojElektroMerilnaMesta != null)
        {
            foreach (var m in jObjekt.MojElektroMerilnaMesta)
            {
                if (!string.IsNullOrEmpty(m.EnotniIdentifikator))
                    seznam.Add(m.Id, m.EnotniIdentifikator);
            }
        }
        return seznam;
    }



    // priprava agregiranih podatkov za grafe !!
    // prvotno - (uporabim za zneske in energijo)

    public Task<Dictionary<int, List<AggregatedData>>> GetAgregatedChartDataForPeriod(IEnumerable<int> statuses, DateTime start, DateTime end,
        Expression<Func<MojElektro15MinMeritev, AggregatedData>> groupDataSelector)
    {
        return GetChartDataForPeriod(statuses, start, end,
            groupDataSelector,
            group => new AggregatedData { Group = group.Key, Sum = group.Sum(x => x.Sum) });
    }


    public Task<Dictionary<int, List<AggregatedData>>> GetChartDataForPeriod(
            IEnumerable<int> statuses,
            DateTime start,
            DateTime end,
            Expression<Func<MojElektro15MinMeritev, AggregatedData>> groupDataSelector,
            Expression<Func<IGrouping<int, AggregatedData>, AggregatedData>> groupExpression)
    {
        return GetChartData(statuses,
            obj => obj.TimeStamp >= start && obj.TimeStamp <= end,
            groupDataSelector,
            groupExpression);
    }


    private async Task<Dictionary<int, List<AggregatedData>>> GetChartData(IEnumerable<int> statuses,
        Expression<Func<MojElektro15MinMeritev, bool>> baseFilter,
        Expression<Func<MojElektro15MinMeritev, AggregatedData>> groupDataSelector,
        Expression<Func<IGrouping<int, AggregatedData>, AggregatedData>> groupExpression)
    {
        var result = new Dictionary<int, List<AggregatedData>>();

        foreach (var statusX in statuses)
        {
            var status = statusX;  // Capture the current value
            var data = await context.MojElektro15MinMeritve
                .AsNoTracking()
                .Where(baseFilter)
                .Where(obj => status == (int)obj.IdMerilnegaMesta)
                .Select(groupDataSelector)
                .GroupBy(obj => obj.Group)
                .Select(groupExpression)
                .ToListAsync();

            var data1 = data.OrderBy(d => d.Group).ToList();
            result.Add(statusX, data1);
        }
        return result;
    }


    private string FormatAxisXLabels(int value, string mode)
    {
        switch (mode)
        {
            case "PoLetihPoMesecihPoBlokih":
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(value.ToString().Substring(4, 2));
                    sb.Append("-");
                    sb.Append(value.ToString().Substring(0, 4));
                    return sb.ToString();
                }
        }
        return value.ToString();
    }

}



                                // ekstrahiramo leto
                                // string leto_pom = vsiTedniLine.Value[indexDnivLetu].Group.ToString().Length >= 4
                                //     ? vsiTedniLine.Value[indexDnivLetu].Group.ToString().Substring(0, 4)
                                //     : string.Empty;
