using System;

namespace API.Entities.MojElektro;

    public class AggregatedData
    {
        public int Group { get; set; }
        public int Count { get; set; }
        public decimal? Sum { get; set; }
    }

    public class AggregatedData<TGroup>
    {
        public TGroup? Group { get; set; }
        public int Count { get; set; }
        public decimal? Sum { get; set; }
    }

    public class ChartDataDTO<T> where T : struct
    {
        public IEnumerable<T>? Values { get; set; }
        public string? Type { get; set; }
        public string? TypeOriginal { get; set; }
    }


    public class BaseChartDTO<T> where T : struct
    {
        public IList<ChartDataDTO<T>>? Lines { get; set; }
        public IEnumerable<string>? AxisXLabels { get; set; }
        public IEnumerable<string>? LegendaOriginal { get; set; }
        public string? ChartLabel { get; set; }
        public string? EnotaMere { get; set; }      // kWh,  kW, 
    }

    
    // public class Egraf
    // {
    //     public List<decimal>? Vrednosti { get; set; }
    //     public List<DateTime>? AxisXLabele { get; set; }
    // }