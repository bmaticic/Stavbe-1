using System;
using API.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace API.Data.SeedData;

public class SeedGeoTocke
{

    public static async Task ImportGeo(DataContext context)
    {
       // if (await context.GeoTocke.AnyAsync()) return;

        Dictionary<string, int> seznamStavb = new Dictionary<string, int>();
        foreach (Stavba stavba in context.Stavbe)
        {
            seznamStavb.Add(stavba.SifraJavnegaObjekta, stavba.Id);
        }

        var path = "Data/Source/En_Kamnik_tocke_objektov_Kamnik_v6.xlsx";

        using var stream = System.IO.File.OpenRead(path);
        using var excelPackage = new ExcelPackage(stream);

        var worksheet = excelPackage.Workbook.Worksheets[0];

        // define how many rows we want to process 
        var nEndRow = worksheet.Dimension.End.Row;


        // initialize the record counters 
        int steviloDodanihGeoTock = 0;
        int steviloNeuspelih = 0;

        var koloneDict = new Dictionary<string, int>();

        var prvaVrsta = worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column];

        for (var i = 1; i <= worksheet.Dimension.End.Column; i++)
        {
            koloneDict.Add(prvaVrsta[1, i].GetValue<string>(), i);
        }

        for (int nRow = 2; nRow <= nEndRow; nRow++)
        {
            var row = worksheet.Cells[
                    nRow, 1, nRow, worksheet.Dimension.End.Column];
            
            var index = row[nRow, koloneDict["Ozn_obj"]].GetValue<string>();
            if (await context.GeoTocke.AnyAsync(x => x.SifraObjekta == index))
            {
                continue;
            }
            var _geoTocka = new GeoTocka();

            _geoTocka.FID = row[nRow, koloneDict["FID"]].GetValue<int>();
            _geoTocka.OZN_obj = row[nRow, koloneDict["Ozn_obj"]].GetValue<string>();
            _geoTocka.Naziv = row[nRow, koloneDict["Naziv"]].GetValue<string>();
            _geoTocka.SID = row[nRow, koloneDict["FID"]].GetValue<string>();
            _geoTocka.SIFKO = row[nRow, koloneDict["SIFKO"]].GetValue<string>();
            _geoTocka.ST_stevilka = row[nRow, koloneDict["ST_stevilka"]].GetValue<string>();
            _geoTocka.Ozn_tock = row[nRow, koloneDict["Ozn_toc"]].GetValue<string>();
            _geoTocka.DDLat = row[nRow, koloneDict["Lat_vmes"]].GetValue<decimal>();
            _geoTocka.DDLon = row[nRow, koloneDict["Lon_vmes"]].GetValue<decimal>();

            // Športni park Domžale
            _geoTocka.SifraObjekta = _geoTocka.OZN_obj;

            _geoTocka.Zaporedje = row[nRow, koloneDict["Vrstni_red"]].GetValue<int>();

            _geoTocka.Lat = _geoTocka.DDLat;
            _geoTocka.Lng = _geoTocka.DDLon;
            // --- Športni park Domžale

            var pp = 0;
            if (seznamStavb.ContainsKey(_geoTocka.SifraObjekta))
            {
                pp = seznamStavb[_geoTocka.SifraObjekta];
                Stavba? stavba = await context.Stavbe.FindAsync(pp);

                if (stavba != null)
                {
                    _geoTocka.IdJavnegaObjekta = stavba.Id;
                    steviloDodanihGeoTock++;
                }
                else
                {
                    steviloNeuspelih++;
                    continue;
                }
            }
            else
            {
                steviloNeuspelih++;
                continue;
            }

            await context.GeoTocke.AddAsync(_geoTocka);
        }

        if (steviloDodanihGeoTock > 0)
            await context.SaveChangesAsync();

        Console.WriteLine($"Dodano {steviloDodanihGeoTock} geo točk stavb.");
        Console.WriteLine($"Neuspešno dodanih {steviloNeuspelih} geo točk stavb.");

        return;

    }

}

