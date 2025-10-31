using System;
using API.Entities.MojElektro;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace API.Data.SeedData;

public class SeedMojElektroMMesta
{
    public static async Task ImportMM(DataContext context)
    {
       // if (await context.MojElektroMerilnaMesta.AnyAsync()) return;

        // var steviloDodanihMerMest = 0;

        var path = "Data/Source/MojElektro/PodatkiMM.xlsx";

        using var stream = System.IO.File.OpenRead(path);
        using var excelPackage = new ExcelPackage(stream);

        // get the first worksheet 
        var worksheet = excelPackage.Workbook.Worksheets[0];

        // define how many rows we want to process 
        var nEndRow = worksheet.Dimension.End.Row;

        // initialize the record counters 
        var steviloDodanihMerilnihMest = 0;
        int steviloNeuspelih = 0;

        // kreiram dictionary prve vrste excla - imena polj
        var koloneDict = new Dictionary<string, int>();
        var prvaVrsta = worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column];
        for (var i = 1; i <= worksheet.Dimension.End.Column; i++)
        {
            koloneDict.Add(prvaVrsta[1, i].GetValue<string>(), i);
        }

        // create a lookup dictionary 
        // containing all the MerilnaMesta already existing 
        // into the Database (it will be empty on first run).
        var obstojecaMojElektroMerilnaMesta = context.MojElektroMerilnaMesta
            .AsNoTracking()
            .ToDictionary(x => x.EnotniIdentifikator ?? string.Empty, StringComparer.OrdinalIgnoreCase);



        for (int nRow = 2; nRow <= nEndRow; nRow++)
        {
            var row = worksheet.Cells[
                nRow, 1, nRow, worksheet.Dimension.End.Column];
            var _merMesto = new MojElektroMerilnoMesto();

            _merMesto.EnotniIdentifikator = row[nRow, koloneDict["Enotni identifikator"]].GetValue<string>();

            // skip this merilno mesto if it already exists in the database
            if (obstojecaMojElektroMerilnaMesta.ContainsKey(_merMesto.EnotniIdentifikator))
                continue;

            _merMesto.SifraJavnegaObjekta = row[nRow, koloneDict["SifraJavnegaObjekta"]].GetValue<string>();

            var jObjekt = context.Stavbe.Where(x => x.SifraJavnegaObjekta == _merMesto.SifraJavnegaObjekta).FirstOrDefault();
            if (jObjekt != null)
            {
                _merMesto.NazivJavnegaObjekta = jObjekt.Naziv;
                _merMesto.IdJavnegaObjekta = jObjekt.Id;
            }
            else
            {
                    steviloNeuspelih++;
                    continue;
            }

            _merMesto.GsrnMM = row[nRow, koloneDict["GSRN MM"]].GetValue<string>();
            _merMesto.Naziv = row[nRow, koloneDict["Naziv"]].GetValue<string>();
            _merMesto.Naslov = row[nRow, koloneDict["Naslov"]].GetValue<string>();

            _merMesto.RTP = row[nRow, koloneDict["RTP"]].GetValue<string>();
            _merMesto.SNizvod = row[nRow, koloneDict["SN izvod"]].GetValue<string>();
            _merMesto.TP = row[nRow, koloneDict["TP"]].GetValue<string>();
            _merMesto.NNizvod = row[nRow, koloneDict["NN izvod"]].GetValue<string>();

            _merMesto.Dobavitelj = row[nRow, koloneDict["Dobavitelj"]].GetValue<string>();

            //          jObjekt.MojElektroMerilnaMesta.Add(_merMesto);

            // add the new to the DB context 
            await context.MojElektroMerilnaMesta.AddAsync(_merMesto);

            // store in our lookup to retrieve its Id later on
            obstojecaMojElektroMerilnaMesta.Add(_merMesto.EnotniIdentifikator, _merMesto);

            // increment the counter 
            steviloDodanihMerilnihMest++;
        }

        // save all into the Database 
        if (steviloDodanihMerilnihMest > 0)
            await context.SaveChangesAsync();

        Console.WriteLine($"Dodano {steviloDodanihMerilnihMest} MojElektro merilnih mest");
        Console.WriteLine($"Neuspešno dodanih {steviloNeuspelih} MojElektro merilnih mest.");

    }
}
