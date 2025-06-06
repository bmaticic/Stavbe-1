using System;
using API.Entities.MojElektro;
using API.Helpers;

namespace API.Interfaces;

public interface IMojElektroRepository
{
    Task<MojElektroMerilnoMesto[]> GetMojElektroMerilnaMesta(string nazivStavbe);
    Task<Egraf> GetPodatkeZaMojElektroMerilnoMesto(string enotniIdentifikator);

}
