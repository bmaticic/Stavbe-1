using System;
using API.DTOs;
using API.Entities;
using API.Entities.MojElektro;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repos;

public class StavbeRepository(DataContext context, IMapper mapper) : IStavbeRepository
{
    public async Task<PagedList<StavbaDto>> GetStavbeAsync(StavbaParams stavbaParams)
    {
        var query = context.Stavbe
            // .Include(x => x.PhotosStavbe)
            // .Include(x => x.MerilnaMesta)
            .AsQueryable();

        if (stavbaParams.VrstaObjekta != "vsi")
        {
            query = query.Where(x => x.VrstaObjekta == stavbaParams.VrstaObjekta);
        }

        if (stavbaParams.TipOgrevanja != "vsi")
        {
            query = query.Where(x => x.OgrevanjeOznaka == stavbaParams.TipOgrevanja);
        }

        return await PagedList<StavbaDto>.CreateAsync(query.ProjectTo<StavbaDto>(mapper.ConfigurationProvider), stavbaParams.PageNumber, stavbaParams.PageSize);
    }

    public async Task<Stavba?> GetStavbaByNazivAsync(string nazivStavbe)
    {
        return await context.Stavbe
        .Where(x => x.Naziv == nazivStavbe)
        .Include(x => x.PhotosStavbe)
        //     .ProjectTo<StavbaDto>(mapper.ConfigurationProvider)
        .SingleOrDefaultAsync();
    }

    public async Task<MerilnoMestoDto[]> GetMerilnaMesta(string nazivStavbe)
    {
        var stavba = await context.Stavbe
            .Where(x => x.Naziv == nazivStavbe)
            .Include(x => x.MerilnaMesta)
            .SingleOrDefaultAsync();

        if (stavba == null) return Array.Empty<MerilnoMestoDto>();

        return mapper.Map<MerilnoMestoDto[]>(stavba.MerilnaMesta);
    }

    public async Task<Egraf> GetPodatkeZaMerilnoMesto(string stMerilnegaMesta)
    {
        var merilnoMesto = await context.MerilnaMesta
            .Where(x => x.StMerilnegaMesta == stMerilnegaMesta)
            .Include(x => x.Odcitki)
            .SingleOrDefaultAsync();

        if (merilnoMesto == null || merilnoMesto.Odcitki?.Count == 0) return new Egraf();

        var vrednosti = merilnoMesto.Odcitki?.Select(x => x.Znesek).ToList();
        var axisXLabele = merilnoMesto.Odcitki?.Select(x => x.LetoMesec).ToList();

        return new Egraf
        {
            Vrednosti = vrednosti,
            AxisXLabeleStr = axisXLabele,
        };
    }


    public async Task<Poligon> GetGeoTocke(string nazivStavbe)
    {
        var stavba = await context.Stavbe
            .Where(x => x.Naziv == nazivStavbe)
            .Include(x => x.GeoTocke)
            .SingleOrDefaultAsync();

        if (stavba?.GeoTocke == null) return null!;
        Poligon _poligon = new Poligon();
        _poligon.IdJavnegaObjekta = stavba.Id;
        _poligon.Naziv = stavba.Naziv;

        // za izris poligonov v GoogleMaps
        var _vsiObodiObjekta = new List<List<Tocka>>();
        var _obodObjekta = new List<Tocka>();

        var _geoSortirane = stavba.GeoTocke.OrderBy(zap => zap.Zaporedje).ToList();
        foreach (var geoTocka in _geoSortirane)
        {
            if (geoTocka.Ozn_tock == "nov poligon")     // ----------------------------------------------nov poligon
            {
                if (_obodObjekta.Count != 0)
                {
                    var pom = _obodObjekta.ToList();
                    _vsiObodiObjekta.Add(pom);
                    _obodObjekta.Clear();
                }
            }
            _obodObjekta.Add(new Tocka { Lat = geoTocka.Lat, Lng = geoTocka.Lng });
        }
        var pom1 = _obodObjekta.ToList();
        _vsiObodiObjekta.Add(pom1);
        _obodObjekta.Clear();
        _poligon.NoviObodiObjekta = _vsiObodiObjekta;

        return _poligon;



        // return mapper.Map<GeoTockaDto[]>(stavba.GeoTocke);
    }


    public async Task<StavbaDto?> GetStavbaDtoByNazivAsync(string nazivStavbe)
    {
        return await context.Stavbe
        .Where(x => x.Naziv == nazivStavbe)
        .Include(x => x.PhotosStavbe)
        .ProjectTo<StavbaDto>(mapper.ConfigurationProvider)
        .SingleOrDefaultAsync();
    }


    public async Task<Stavba?> GetStavbaByIdAsync(int id)
    {
        return await context.Stavbe
        .Include(x => x.PhotosStavbe)
        .Where(x => x.Id == id)
        //   .ProjectTo<StavbaDto>(mapper.ConfigurationProvider)
        .SingleOrDefaultAsync();
    }

    public async Task<StavbaDto?> GetStavbaDtoByIdAsync(int id)
    {
        return await context.Stavbe
        .Include(x => x.PhotosStavbe)
        .Where(x => x.Id == id)
        .ProjectTo<StavbaDto>(mapper.ConfigurationProvider)
        .SingleOrDefaultAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }


    // public void UpdateAsync(Stavba stavba)
    // {
    //     context.Entry(stavba).State = EntityState.Modified;
    // }
}



