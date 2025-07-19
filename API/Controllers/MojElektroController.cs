using System;
using API.DTOs;
using API.Entities.MojElektro;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MojElektroController(IMojElektroRepository mojElektroRepository) : BaseApiController
{
    // vrne seznam vseh moj elektro merilnih mest za določeno stavbo (v glavnem je eden)
    [HttpGet("moj-elektro-merilna-mesta/{nazivStavbe}")]
    public async Task<ActionResult<MojElektroMerilnoMesto[]>> GetMojElektroMerilnaMesta(string nazivStavbe)
    {
        var mojEleMerMesta = await mojElektroRepository.GetMojElektroMerilnaMesta(nazivStavbe);
        if (mojEleMerMesta == null) return NotFound();
        return Ok(mojEleMerMesta);
    }

    // struktura za Egraf za moj elektro merilno mesto po enotnem identifikatorju
    [HttpGet("moj-elektro-merilno-mesto/{enotniIdentifikator}")]
    public async Task<ActionResult<Egraf>> GetPodatkeZaMojElektroMerilnoMesto(string enotniIdentifikator)
    {
        var egraf = await mojElektroRepository.GetPodatkeZaMojElektroMerilnoMesto(enotniIdentifikator);
        if (egraf == null) return NotFound();
        return Ok(egraf);
    }

    [HttpGet("moj-elektro-merilna-mesta-vsa")]
    public async Task<ActionResult<MojElektroMerilnoMestoDto[]>> GetVsaMojElektroMerilnaMesta()
    {
        var mojEleMerMesta = await mojElektroRepository.GetVsaMojElektroMerilnaMesta();
        if (mojEleMerMesta == null) return NotFound();
        return Ok(mojEleMerMesta);
    }

    // moj elektro merilno mesto po enotnem identifikatorju
    [HttpGet("{enotniIdentifikator}")]
    public async Task<ActionResult<MojElektroMerilnoMestoDto>> GetMojElektroMerilnoMesto(string enotniIdentifikator)
    {
        if (string.IsNullOrEmpty(enotniIdentifikator)) return BadRequest("Enotni identifikator ni določen.");

        var mojEleMerMesto = await mojElektroRepository.GetMojElektroMerilnoMesto(enotniIdentifikator);
        if (mojEleMerMesto == null) return NotFound();

        return Ok(mojEleMerMesto);
    }



}
