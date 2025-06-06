using System;
using API.DTOs;
using API.Entities;
using API.Entities.MojElektro;
using API.Helpers;

namespace API.Interfaces;

public interface IStavbeRepository
{
    Task<bool> SaveAllAsync();
    Task<PagedList<StavbaDto>> GetStavbeAsync(StavbaParams stavbaParams);
    Task<Stavba?> GetStavbaByIdAsync(int id);
    Task<StavbaDto?> GetStavbaDtoByIdAsync(int id);
    Task<Stavba?> GetStavbaByNazivAsync(string nazivStavbe);
    Task<StavbaDto?> GetStavbaDtoByNazivAsync(string nazivStavbe);
    Task<MerilnoMestoDto[]> GetMerilnaMesta(string nazivStavbe);
    Task<Egraf> GetPodatkeZaMerilnoMesto(string stMerilnegaMesta);
    Task<Poligon> GetGeoTocke(string nazivStavbe);
  //  Task<Stavba> UpdateAsync(Stavba stavba);
}
