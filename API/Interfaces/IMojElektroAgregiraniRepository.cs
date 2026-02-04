using System;
using API.Entities;
using API.Entities.MojElektro;

namespace API.Interfaces;

public interface IMojElektroAgregiraniRepository
{
        Task<BaseChartDTO<decimal>> Get_15MinMeritvePoLetih(HttpParametri aggrInDatumi);


}
