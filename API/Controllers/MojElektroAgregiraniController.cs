using System;
using API.Data.Repos;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MojElektroAgregiraniController(IMojElektroAgregiraniRepository mojElektroAgregiraniRepository) : BaseApiController
{

    
    [HttpGet("moj-elektro-agregirani-za-egraf")]
    public async Task<ActionResult<Egraf>> GetMojElektroForChart([FromQuery] HttpParametri aggrInDatumi)
    {
        var dataForGraphic = await mojElektroAgregiraniRepository.Get_15MinMeritvePoLetih(aggrInDatumi);

        // dodamo še naslove grafov
        switch (aggrInDatumi.sifraEnergijaMoc)
        {
            case "EnergijaAPlus":
                dataForGraphic.ChartLabel = "Energija A+ (kWh)";
                dataForGraphic.EnotaMere = "(kWh)";
                break;
            case "PrejetaDelovnaMoc":
                dataForGraphic.ChartLabel = "Prejeta delovna moč (kW)";
                dataForGraphic.EnotaMere = "(kW)";
                break;
            default:
                dataForGraphic.ChartLabel = "empty";
                break;
        }

        switch (aggrInDatumi.aggregation)
        {
            case "PoLetihPoMesecih":
                dataForGraphic.ChartLabel = dataForGraphic.ChartLabel + " po letih / mesecih";
                break;
            case "PoLetihPoMesecihPoBlokih":
                dataForGraphic.ChartLabel = dataForGraphic.ChartLabel + " po mesecih / po blokih";
                break;

            case "PoLetihPoTednihPoDnevih":
                dataForGraphic.ChartLabel = dataForGraphic.ChartLabel + " po tednih / dnevih";
                break;
            case "PoLetihPoDnevihPoUrah":
                dataForGraphic.ChartLabel = dataForGraphic.ChartLabel + " po dnevih / urah";
                break;

            default:
                dataForGraphic.ChartLabel = "empty";
                break;
        }

        return Ok(dataForGraphic);
    }


}
