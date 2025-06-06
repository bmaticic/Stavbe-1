using System;
using API.Entities.MojElektro;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repos;

public class MojElektroRepository(DataContext context) : IMojElektroRepository
{
    public async Task<MojElektroMerilnoMesto[]> GetMojElektroMerilnaMesta(string nazivStavbe)
    {
        var stavba = await context.Stavbe
            .Where(x => x.Naziv == nazivStavbe)
            .Include(x => x.MojElektroMerilnaMesta)
            .SingleOrDefaultAsync();

        if (stavba == null || stavba.MojElektroMerilnaMesta == null) return Array.Empty<MojElektroMerilnoMesto>();

        return stavba.MojElektroMerilnaMesta
            .Select(x => new MojElektroMerilnoMesto
            {
                Id = x.Id,
                EnotniIdentifikator = x.EnotniIdentifikator,
                SifraJavnegaObjekta = x.SifraJavnegaObjekta,
                NazivJavnegaObjekta = x.NazivJavnegaObjekta,
                IdJavnegaObjekta = x.IdJavnegaObjekta,
                GsrnMM = x.GsrnMM,
                Naziv = x.Naziv,
                Naslov = x.Naslov,
                RTP = x.RTP,
                SNizvod = x.SNizvod,
                TP = x.TP,
                NNizvod = x.NNizvod,
                Dobavitelj = x.Dobavitelj
            })
            .ToArray();
    }

    public async Task<Egraf> GetPodatkeZaMojElektroMerilnoMesto(string enotniIdentifikator)
    {
        var mmesto = await context.MojElektroMerilnaMesta
            .Where(x => x.EnotniIdentifikator == enotniIdentifikator)
            .Include(x => x.Meritve15min)
            .SingleOrDefaultAsync();
        if (mmesto == null || mmesto.Meritve15min?.Count == 0) return new Egraf();

        var vrednosti = mmesto.Meritve15min?.Select(x => x.Energija_A_plus).ToList();
        var axisXLabele = mmesto.Meritve15min?.Select(x => x.TimeStamp).ToList();

        return new Egraf
        {
            Vrednosti = vrednosti?.ToList(), // Prvih 96 vrednosti
            AxisXLabele = axisXLabele?.ToList(), // Prvih 96 časovnih oznak
        };
    }
}
