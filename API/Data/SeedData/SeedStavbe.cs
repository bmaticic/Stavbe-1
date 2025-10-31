using API.Data.Enums;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace API.Data.SeedData;

public class SeedStavbe
{
    // --------------------------------------------------------- Vnos javnih objektov iz matrike Kamnik  

    public static async Task Seed(DataContext context)
    {
        // if (await context.Stavbe.AnyAsync()) return;

        var path = "Data/Source/JObjektiInMerilnaMesta.xlsx";

        using var stream = System.IO.File.OpenRead(path);
        using var excelPackage = new ExcelPackage(stream);

        // get the first worksheet 
        var worksheet = excelPackage.Workbook.Worksheets[0];

        // define how many rows we want to process 
        var nEndRow = worksheet.Dimension.End.Row;


        // initialize the record counters 
        var steviloDodanihJobjektov = 0;

        // kreiram dictionary prve vrste excla - imena polj

        var koloneDict = new Dictionary<string, int>();

        // var prvaVrsta = worksheet.Row(1);

        var prvaVrsta = worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column];

        for (var i = 1; i < worksheet.Dimension.End.Column; i++)
        {
            koloneDict.Add(prvaVrsta[1, i].GetValue<string>(), i);
        }


        // create a lookup dictionary 
        // containing all the javni objekti already existing 
        // into the Database (it will be empty on first run).
        var jObjektiPoSifri = context.Stavbe
            .AsNoTracking()
            .ToDictionary(x => x.SifraJavnegaObjekta, StringComparer.OrdinalIgnoreCase);

        // iterates through all rows, skipping the first one 
        for (int nRow = 2; nRow <= nEndRow; nRow++)
        {
            var row = worksheet.Cells[
                nRow, 1, nRow, worksheet.Dimension.End.Column];

            var sifraJavnegaObjekta = row[nRow, koloneDict["OZN_obj"]].GetValue<string>();
            var naziv = row[nRow, koloneDict["Naziv"]].GetValue<string>();
            var nazivEnote = row[nRow, koloneDict["Naziv"]].GetValue<string>();
            var naslov = row[nRow, koloneDict["Naziv"]].GetValue<string>();
            var ulicaHs = row[nRow, 2].GetValue<string>();
            var katastrskaObcinaSifra = row[nRow, koloneDict["SIFKO"]].GetValue<string>();
            var vrstaObjekta = row[nRow, koloneDict["Vrsta_objekta"]].GetValue<string>();

            // skip this -javni objekt if it already exists in the database
            if (jObjektiPoSifri.ContainsKey(sifraJavnegaObjekta))
                continue;

            // dodamo dve fotki za testiranje
            var foto1 = new PhotoStavbe(){
                Url = "C:\\BOZO\\ASources\\Stavbe\\API\\Data\\slikeStavb\\2_DomKulture.png",
                IsMain = true
            };
            var foto2 = new PhotoStavbe(){
                Url = "https://randomuser.me/api/portraits/women/56.jpg",
                IsMain = false
            };
            var fotke = new List<PhotoStavbe>();
            fotke.Add(foto1);
            fotke.Add(foto2);                    



            var stavba = new Stavba
            {
                SifraJavnegaObjekta = sifraJavnegaObjekta,
                Naziv = naziv,
                NazivEnote = nazivEnote,
                Naslov = naslov,
                UlicaHs = ulicaHs,
                KatastrskaObcinaSifra = katastrskaObcinaSifra,
                KatastrskaObcinaIme = row[nRow, koloneDict["SIFKO"]].GetValue<string>(),

                // Kamnik

                ST_OBJ_Gurs = row[nRow, koloneDict["ST_OBJ"]].GetValue<string>(),
                NTP_NetoTloris = row[nRow, koloneDict["NTP"]].GetValue<decimal>(),
                UporabnaPovrsina = row[nRow, koloneDict["Uporabna"]].GetValue<decimal>(),
                ProjekcijaTloris = row[nRow, koloneDict["NTP"]].GetValue<decimal>(),


                VrstaObjekta = vrstaObjekta,
                VrstaObjektaId = (int)Enum.Parse<VrstaStavbeEnum>(vrstaObjekta),

                Ogrevanje = row[nRow, koloneDict["Ogrevanje"]].GetValue<string>(),
                OgrevanjeOznaka = row[nRow, koloneDict["Ogr_kraj1"]].GetValue<string>(),
                OgrevanjeDrugi = row[nRow, koloneDict["Ogrevanje_drugi"]].GetValue<string>(),
                Opomba = row[nRow, koloneDict["OPOMBA"]].GetValue<string>(),

                Parcele = row[nRow, koloneDict["Parcele"]].GetValue<string>(),
                ParcelePovrsina = row[nRow, koloneDict["Površina parcel"]].GetValue<decimal>(),
                StavbaDaNe = row[nRow, koloneDict["Stavbe"]].GetValue<bool>(),
                StavbaStevilka = row[nRow, koloneDict["Št. Stavbe"]].GetValue<string>(),
                StavbaDel = row[nRow, koloneDict["Del stavbe"]].GetValue<string>(),

                KlasifikacijaCcSi = row[nRow, koloneDict["Klasifikacija CC-SI"]].GetValue<string>(),
                KlasifikacijaNaziv = row[nRow, koloneDict["Klasifikacija naziv"]].GetValue<string>(),

                LetoIzgradnje = row[nRow, koloneDict["Leto izgradnje"]].GetValue<string>(),
                LetoObnove = row[nRow, koloneDict["Leto obnove"]].GetValue<string>(),
                KlimatskiKraj = row[nRow, koloneDict["Klimatski kraj"]].GetValue<string>(),
                //    NadmorskaVisina = row[nRow, koloneDict["Nadmorska višina"]].GetValue<decimal>(),
                NadmorskaVisina = 300,

                KondicioniranaPovrsina = row[nRow, koloneDict["Uporabna"]].GetValue<decimal>(),
                PovrsinaAplikacija = row[nRow, koloneDict["Uporabna"]].GetValue<decimal>(),

                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
            };

            // if(nRow == 2)
            // {
            //     stavba.PhotosStavbe = fotke;
            // }
            
            // add the new to the DB context 
            await context.Stavbe.AddAsync(stavba);

            // store in our lookup to retrieve its Id later on
            jObjektiPoSifri.Add(sifraJavnegaObjekta, stavba);

            // increment the counter 
            steviloDodanihJobjektov++;
        }

        // save all into the Database 
        if (steviloDodanihJobjektov > 0)
            await context.SaveChangesAsync();

        return;

    }



}
