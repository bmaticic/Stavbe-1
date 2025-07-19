using System;
using API.DTOs;
using API.Entities.MojElektro;
using API.Helpers;

namespace API.Interfaces;

public interface IMojElektroRepository
{
    Task<MojElektroMerilnoMesto[]> GetMojElektroMerilnaMesta(string nazivStavbe);
    Task<Egraf> GetPodatkeZaMojElektroMerilnoMesto(string enotniIdentifikator);

    Task<MojElektroMerilnoMestoDto[]> GetVsaMojElektroMerilnaMesta();

    Task<MojElektroMerilnoMestoDto> GetMojElektroMerilnoMesto(string enotniIdentifikator);

}
