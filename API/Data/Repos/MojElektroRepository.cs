using System;
using System.Linq;
using API.DTOs;
using API.Entities.MojElektro;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repos;

public class MojElektroRepository(DataContext context, IMapper mapper) : IMojElektroRepository
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

    // vrne Egraf za eno merilno mesto po enotnem identifikatorju iz MojElektro
    public async Task<Egraf> GetPodatkeZaMojElektroMerilnoMesto(string enotniIdentifikator)
    {
        var mmesto = await context.MojElektroMerilnaMesta
            .Where(x => x.EnotniIdentifikator == enotniIdentifikator)
            .Include(x => x.Meritve15min)
            .SingleOrDefaultAsync();
        if (mmesto == null || mmesto.Meritve15min?.Count == 0) return new Egraf();


        var vrednosti = mmesto.Meritve15min?.Select(x => x.Energija_A_plus).ToList();
        var axisXLabele = mmesto.Meritve15min?.Select(x => x.TimeStamp).ToList();
        var bloki = mmesto.Meritve15min?.Select(x => x.Blok).ToList();

        // Fill gaps in AxisXLabele with missing 15-min timestamps and 0.0 values in Vrednosti and bloki
        if (axisXLabele != null && vrednosti != null && bloki != null && axisXLabele.Count > 1)
        {
            var filledAxisXLabele = new List<DateTime>();
            var filledVrednosti = new List<decimal>();
            var filledBloki = new List<int>();
            filledAxisXLabele.Add(axisXLabele[0]);
            filledVrednosti.Add(vrednosti[0]);
            filledBloki.Add(bloki[0]);
            for (int i = 1; i < axisXLabele.Count; i++)
            {
                var prev = axisXLabele[i - 1];
                var curr = axisXLabele[i];
                var prevVal = vrednosti[i - 1];
                var currVal = vrednosti[i];
                var prevBlok = bloki[i - 1];
                var currBlok = bloki[i];
                var diff = curr - prev;
                while (diff > TimeSpan.FromMinutes(15))
                {
                    prev = prev.AddMinutes(15);
                    filledAxisXLabele.Add(prev);
                    filledVrednosti.Add(0);
                    filledBloki.Add(0);
                    diff = curr - prev;
                }
                filledAxisXLabele.Add(curr);
                filledVrednosti.Add(currVal);
                filledBloki.Add(currBlok);
            }
            axisXLabele = filledAxisXLabele;
            vrednosti = filledVrednosti;
            bloki = filledBloki;
        }
        return new Egraf
        {
            Vrednosti = vrednosti?.ToList(),
            AxisXLabele = axisXLabele?.ToList(),
            Bloki = bloki?.ToList()
        };
    }




    public async Task<MojElektroMerilnoMestoDto[]> GetVsaMojElektroMerilnaMesta()
    {
        var mmesta = await context.MojElektroMerilnaMesta
        .Include(x => x.Stavba)
        .Include(x => x.Stavba!.PhotosStavbe)
        .ToArrayAsync();
        if (mmesta == null || mmesta.Length == 0) return Array.Empty<MojElektroMerilnoMestoDto>();

        return mapper.Map<MojElektroMerilnoMestoDto[]>(mmesta);
    }

    public async Task<MojElektroMerilnoMestoDto> GetMojElektroMerilnoMesto(string enotniIdentifikator)
    {
        var mmesto = await context.MojElektroMerilnaMesta
        .Where(x => x.EnotniIdentifikator == enotniIdentifikator)
        .Include(x => x.Stavba)
        .Include(x => x.Stavba!.PhotosStavbe)
        .SingleOrDefaultAsync();
        if (mmesto == null ) return new MojElektroMerilnoMestoDto();
        return mapper.Map<MojElektroMerilnoMestoDto>(mmesto);
    }

}
