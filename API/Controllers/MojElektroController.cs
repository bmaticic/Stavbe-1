using System;
using API.Entities.MojElektro;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MojElektroController(IMojElektroRepository mojElektroRepository) : BaseApiController
{
    [HttpGet("moj-elektro-merilna-mesta/{nazivStavbe}")]
    public async Task<ActionResult<MojElektroMerilnoMesto[]>> GetMojElektroMerilnaMesta(string nazivStavbe)
    {
        var mojEleMerMesta = await mojElektroRepository.GetMojElektroMerilnaMesta(nazivStavbe);
        if (mojEleMerMesta == null) return NotFound();
        return Ok(mojEleMerMesta);
    }

    [HttpGet("moj-elektro-merilno-mesto/{enotniIdentifikator}")]
    public async Task<ActionResult<Egraf>> GetPodatkeZaMojElektroMerilnoMesto(string enotniIdentifikator)
    {
        var egraf = await mojElektroRepository.GetPodatkeZaMojElektroMerilnoMesto(enotniIdentifikator);
        if (egraf == null) return NotFound();
        return Ok(egraf);
    }



}
