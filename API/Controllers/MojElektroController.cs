using System;
using API.Data.Repos;
using API.DTOs;
using API.Entities;
using API.Entities.MojElektro;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MojElektroController : BaseApiController
{
    private readonly IMojElektroRepository _mojElektroRepository;

    public MojElektroController(IMojElektroRepository mojElektroRepository)
    {
        _mojElektroRepository = mojElektroRepository;
    }

    public class GroupedEgrafRequest
    {
        public string? EnotniIdentifikator { get; set; }
        public string? GroupedBy { get; set; }
    }
//------------------------------------------------------------------------------------------------------------------------------
[HttpGet("moj-elektro-merilno-mesto-grouped")]
public async Task<ActionResult<Egraf>> GetGroupedEgraf(
    [FromQuery] string enotniIdentifikator,
    [FromQuery] string groupedBy)
{
    if (string.IsNullOrEmpty(enotniIdentifikator) || string.IsNullOrEmpty(groupedBy))
        return BadRequest("Missing parameters.");

    var egraf = await _mojElektroRepository
        .GetGrupiranePodatkeZaMojElektroMerilnoMesto(enotniIdentifikator, groupedBy);

    if (egraf == null) return NotFound();
    return Ok(egraf);
}

    // Egraf grouped by month, week, or day
    [HttpGet("moj-elektro-merilno-mesto-grupirano/{enotniIdentifikator}/{groupBy}")]
    public async Task<ActionResult<Egraf>> GetGrupiranePodatkeZaMojElektroMerilnoMesto(string enotniIdentifikator, string groupBy)
    {
        if (string.IsNullOrEmpty(enotniIdentifikator)) return BadRequest("Enotni identifikator ni določen.");
        if (string.IsNullOrEmpty(groupBy)) return BadRequest("Parameter groupBy ni določen.");

        var egraf = await _mojElektroRepository.GetGrupiranePodatkeZaMojElektroMerilnoMesto(enotniIdentifikator, groupBy);
        if (egraf == null) return NotFound();
        return Ok(egraf);
    }


    // vrne seznam vseh moj elektro merilnih mest za določeno stavbo (v glavnem je eden)
    [HttpGet("moj-elektro-merilna-mesta/{nazivStavbe}")]
    public async Task<ActionResult<MojElektroMerilnoMesto[]>> GetMojElektroMerilnaMesta(string nazivStavbe)
    {
        var mojEleMerMesta = await _mojElektroRepository.GetMojElektroMerilnaMesta(nazivStavbe);
        if (mojEleMerMesta == null) return NotFound();
        return Ok(mojEleMerMesta);
    }


    [HttpGet("moj-elektro-merilna-mesta-vsa")]
    public async Task<ActionResult<MojElektroMerilnoMestoDto[]>> GetVsaMojElektroMerilnaMesta()
    {
        var mojEleMerMesta = await _mojElektroRepository.GetVsaMojElektroMerilnaMesta();
        if (mojEleMerMesta == null) return NotFound();
        return Ok(mojEleMerMesta);
    }

    // moj elektro merilno mesto po enotnem identifikatorju
    [HttpGet("{enotniIdentifikator}")]
    public async Task<ActionResult<MojElektroMerilnoMestoDto>> GetMojElektroMerilnoMesto(string enotniIdentifikator)
    {
        if (string.IsNullOrEmpty(enotniIdentifikator)) return BadRequest("Enotni identifikator ni določen.");

        var mojEleMerMesto = await _mojElektroRepository.GetMojElektroMerilnoMesto(enotniIdentifikator);
        if (mojEleMerMesto == null) return NotFound();

        return Ok(mojEleMerMesto);
    }


    // struktura za Egraf za moj elektro merilno mesto po enotnem identifikatorju
    [HttpGet("moj-elektro-merilno-mesto/{enotniIdentifikator}")]
    public async Task<ActionResult<Egraf>> GetPodatkeZaMojElektroMerilnoMesto(string enotniIdentifikator)
    {
        var egraf = await _mojElektroRepository.GetPodatkeZaMojElektroMerilnoMesto(enotniIdentifikator);
        if (egraf == null) return NotFound();
        return Ok(egraf);
    }

}
