using System;
using System.Text;
using API.Data.Enums;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace API.Data.SeedData;
    //----------------------------------------------------------------------------------------------------  ImportElektro
    //----------------------------------------------------------------------------------------------------  ImportSmeti
    //----------------------------------------------------------------------------------------------------  ImportKpk Kamnik



public class SeedOdcitki
{
    public static async Task ImportElektro(DataContext context)
    {
        if (await context.Odcitki.AnyAsync()) return;

        var path = "Data/Source/En_Kamnik_Elektrika_2020_2022_v3.xlsx";

        using var stream = System.IO.File.OpenRead(path);
        using var excelPackage = new ExcelPackage(stream);

        // get the first worksheet 
        var worksheet = excelPackage.Workbook.Worksheets[0];

        // define how many rows we want to process 
        var nEndRow = worksheet.Dimension.End.Row;

        // initialize the record counters 
        var steviloDodanihOdcitkov = 0;
        int steviloNeuspelih = 0;

        // kreiram dictionary prve vrste excla - imena polj
        var koloneDict = new Dictionary<string, int>();
        var prvaVrsta = worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column];
        for (var i = 1; i <= worksheet.Dimension.End.Column; i++)
        {
            koloneDict.Add(prvaVrsta[1, i].GetValue<string>(), i);
        }

        var meseciDict = new Dictionary<string, int>();
        int indeksKolone = 1;
        meseciDict.Add("Januar", indeksKolone++);
        meseciDict.Add("Februar", indeksKolone++);
        meseciDict.Add("Marec", indeksKolone++);
        meseciDict.Add("April", indeksKolone++);
        meseciDict.Add("Maj", indeksKolone++);
        meseciDict.Add("Junij", indeksKolone++);
        meseciDict.Add("Julij", indeksKolone++);
        meseciDict.Add("Avgust", indeksKolone++);
        meseciDict.Add("September", indeksKolone++);
        meseciDict.Add("Oktober", indeksKolone++);
        meseciDict.Add("November", indeksKolone++);
        meseciDict.Add("December", indeksKolone++);

        Dictionary<string, int> seznamNeuvrscenihMerilnihMest = new Dictionary<string, int>();
        int steviloNeuspelihMerilnihMest = 0;

        Dictionary<string, int> seznamMerMest = new Dictionary<string, int>();
        Dictionary<string, string> seznamDobaviteljiMerMesta = new Dictionary<string, string>();

        foreach (MerilnoMesto mermesto in context.MerilnaMesta)
        {
            try
            {
                seznamMerMest.Add(mermesto.StMerilnegaMesta, mermesto.Id);
                seznamDobaviteljiMerMesta.Add(mermesto.Dobavitelj, mermesto.StMerilnegaMesta);
            }
            catch
            {
                Console.WriteLine("napaka dict");
            }
        }

        for (int nRow = 2; nRow <= nEndRow; nRow++)
        {
            var row = worksheet.Cells[
                nRow, 1, nRow, worksheet.Dimension.End.Column];

            var _odc = new Odcitek();

            _odc.Type = Convert.ToInt16(OdcitekTypeEnum.ELEKTRIKA);
            _odc.EnergentTip = "ELEKTRIKA";



            var pom = row[nRow, koloneDict["Merilno_mesto"]].GetValue<string>();
            if (seznamDobaviteljiMerMesta.ContainsKey(pom))
            {
                var _sifraMerMesta = seznamDobaviteljiMerMesta[pom];
                _odc.StMerilnegaMesta = _sifraMerMesta;
            }
            else
            {
                Console.WriteLine("sifra mermesta ne obstaja:" + pom);                                  // tukaj lovimo nova merilna mesta !!!!!!!!!!!!!!!
                break;
            }


            _odc.NazivMerilnegaMesta = row[nRow, koloneDict["Merilno_mesto"]].GetValue<string>();     // naziv merilnega mesta uporabimo za dobavitelja


            if (seznamMerMest[_odc.StMerilnegaMesta] > 0)
                _odc.IdMerilnegaMesta = seznamMerMest[_odc.StMerilnegaMesta];
            else
            {
                _odc.IdMerilnegaMesta = 0;
                Console.WriteLine("ele odcitek brez mer mesta" + _odc.NazivMerilnegaMesta);
            }



            var leto = row[nRow, koloneDict["Leto"]].GetValue<string>();
            var letoNum = Convert.ToInt16(leto);
            var mesec = row[nRow, koloneDict["Mesec"]].GetValue<string>();
            var mesecNumeric = meseciDict[mesec].ToString("00");
            _odc.LetoMesec = leto + meseciDict[mesec].ToString("00");

            _odc.StevilkaRacuna = row[nRow, koloneDict["Št. Računa"]].GetValue<string>();

            var pomDate = new DateTime(letoNum, meseciDict[mesec], 1);
            _odc.DatumOdcitka = pomDate;

            // ele

            _odc.EleenergijaVt = row[nRow, koloneDict["Energija VT (kWh)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Energija VT (kWh)"]].GetValue<decimal>();
            _odc.EleenergijaMt = row[nRow, koloneDict["Energija MT (kWh)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Energija MT (kWh)"]].GetValue<decimal>();
            _odc.EleenergijaET = row[nRow, koloneDict["Energija ET (kWh)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Energija ET (kWh)"]].GetValue<decimal>();
            _odc.Energija = (_odc.EleenergijaMt ?? 0) + (_odc.EleenergijaVt ?? 0) + (_odc.EleenergijaET ?? 0);


            _odc.EleJalovaEnergija = row[nRow, koloneDict["Jalova energija (kVArh)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Jalova energija (kVArh)"]].GetValue<decimal>();

            _odc.EleobracunskaMoc = row[nRow, koloneDict["Obračunska moč (kW)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Obračunska moč (kW)"]].GetValue<decimal>();
            _odc.ELEEnergijaEUR = row[nRow, koloneDict["Energija (EUR)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Energija (EUR)"]].GetValue<decimal>();
            _odc.ELEOmreznina = row[nRow, koloneDict["Omrežnina (EUR)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Omrežnina (EUR)"]].GetValue<decimal>();
            _odc.ELEPrispevki = row[nRow, koloneDict["Prispevki (EUR)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Prispevki (EUR)"]].GetValue<decimal>();
            _odc.Znesek = row[nRow, koloneDict["Skupaj z DDV"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Skupaj z DDV"]].GetValue<decimal>();

            _odc.CreatedDate = DateTime.Now;
            _odc.UpdatedDate = DateTime.Now;

            if (seznamMerMest.ContainsKey(_odc.StMerilnegaMesta))
            {
                _odc.IdMerilnegaMesta = seznamMerMest[_odc.StMerilnegaMesta];

                var mmesto = await context.MerilnaMesta.FindAsync(_odc.IdMerilnegaMesta);
                if (mmesto == null)
                {
                    Console.WriteLine($"MerilnoMesto with Id {_odc.IdMerilnegaMesta} not found.");
                    continue;
                }

                _odc.MerilnoMesto = mmesto;
                _odc.StMerilnegaMesta = mmesto.StMerilnegaMesta;

                _odc.IdJavnegaObjekta = mmesto.IdJavnegaZavoda;


                // za debug

                if (_odc.IdJavnegaObjekta == 8)
                    Console.WriteLine("ttt");



            }
            else
            {
                if (!seznamNeuvrscenihMerilnihMest.ContainsKey(_odc.StMerilnegaMesta))
                {
                    seznamNeuvrscenihMerilnihMest.Add(_odc.StMerilnegaMesta, steviloNeuspelihMerilnihMest++);
                }
                steviloNeuspelih++;
            }
            await context.Odcitki.AddAsync(_odc);
            steviloDodanihOdcitkov++;
        }
        if (steviloDodanihOdcitkov > 0)
            await context.SaveChangesAsync();

        Console.WriteLine($"Dodano {steviloDodanihOdcitkov} steviloDodanihMeritev");
        Console.WriteLine($"Neuspešno dodanih {steviloNeuspelih} steviloNeuspelih.");
    }




    //----------------------------------------------------------------------------------------------------  ImportSmeti
    public static async Task ImportSmeti(DataContext context)
    {
        Dictionary<string, int> seznamMerMest = new Dictionary<string, int>();
        Dictionary<int, string> seznamDobaviteljev = new Dictionary<int, string>();
        Dictionary<string, string> seznamMerMestaDobavitelji = new Dictionary<string, string>();
        Dictionary<string, string> seznamDobaviteljiMerMesta = new Dictionary<string, string>();
        foreach (MerilnoMesto mermesto in context.MerilnaMesta)
        {
            try
            {
                seznamMerMest.Add(mermesto.StMerilnegaMesta, mermesto.Id);
                seznamDobaviteljev.Add(mermesto.Id, mermesto.Dobavitelj);
                seznamMerMestaDobavitelji.Add(mermesto.StMerilnegaMesta, mermesto.Dobavitelj);
                seznamDobaviteljiMerMesta.Add(mermesto.Dobavitelj, mermesto.StMerilnegaMesta);
            }
            catch
            {
                Console.WriteLine("napaka dict");
            }
        }

        var path = "Data/Source/En_Kamnik_Smeti_2020_2022_v2.xlsx";
        //  "Data/Source/En_Kamnik_Smeti_2020_2021_v6.xlsx");
        using var stream = System.IO.File.OpenRead(path);
        using var excelPackage = new ExcelPackage(stream);

        // get the first worksheet 
        var worksheet = excelPackage.Workbook.Worksheets[0];

        // define how many rows we want to process 
        var nEndRow = worksheet.Dimension.End.Row;

        // initialize the record counters 
        var steviloDodanihOdcitkov = 0;
        int steviloNeuspelih = 0;

        // kreiram dictionary prve vrste excla - imena polj
        var koloneDict = new Dictionary<string, int>();
        var prvaVrsta = worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column];
        for (var i = 1; i <= worksheet.Dimension.End.Column; i++)
        {
            koloneDict.Add(prvaVrsta[1, i].GetValue<string>(), i);
        }

        for (int nRow = 2; nRow <= nEndRow; nRow++)
        {
            var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];
            var _odc = new Odcitek();


            var pom = row[nRow, koloneDict["ODJEMNO MESTO"]].GetValue<string>();
            if (pom == null) { pom = "*"; continue; }

            if (seznamDobaviteljiMerMesta.ContainsKey(pom))
            {
                var _sifraMerMesta = seznamDobaviteljiMerMesta[row[nRow, koloneDict["ODJEMNO MESTO"]].GetValue<string>()];
                _odc.StMerilnegaMesta = _sifraMerMesta;
            }
            else
            {
                Console.WriteLine("ni ključa " + pom);                      // tu lovimo nova mer mesta   !!!!!!!
                continue;
            }

            if (seznamMerMest.ContainsKey(_odc.StMerilnegaMesta))
            {
                _odc.IdMerilnegaMesta = seznamMerMest[_odc.StMerilnegaMesta];
                MerilnoMesto? mmesto = await context.MerilnaMesta.FindAsync(_odc.IdMerilnegaMesta);

                _odc.MerilnoMesto = mmesto;
                _odc.StMerilnegaMesta = mmesto.StMerilnegaMesta;
                _odc.IdJavnegaObjekta = mmesto.IdJavnegaZavoda;
                _odc.NazivMerilnegaMesta = pom;
            }
            else
            {
                steviloNeuspelih++;
                continue;
            }

            _odc.EnergentTip = "SMETI";
            _odc.Type = Convert.ToInt16(OdcitekTypeEnum.SMETI);

            _odc.StevilkaRacuna = row[nRow, koloneDict["ŠT. RAČUNA"]].GetValue<string>();

            _odc.LetoMesec = row[nRow, koloneDict["Obracun_mesec"]].GetValue<string>();

            char[] karakterji = row[nRow, koloneDict["Obracun_mesec"]].GetValue<string>().ToCharArray();
            var leto = new StringBuilder();
            leto.Append(karakterji[0]);
            leto.Append(karakterji[1]);
            leto.Append(karakterji[2]);
            leto.Append(karakterji[3]);
            var leto1 = Convert.ToInt32(leto.ToString());

            var mesec = new StringBuilder();
            mesec.Append(karakterji[4]);
            mesec.Append(karakterji[5]);
            var mesec1 = Convert.ToInt32(mesec.ToString());
            DateTime currDatum = new DateTime(leto1, mesec1, 1);

            _odc.DatumOdcitka = currDatum;
            _odc.ObdobjeStoritveOd = currDatum;
            _odc.ObdobjeStoritveDo = currDatum;

            _odc.SMETIPapirKartonEur = row[nRow, koloneDict["Papir/Karton (EUR)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Papir/Karton (EUR)"]].GetValue<decimal>();

            _odc.SMETIEmbalazaEur = row[nRow, koloneDict["Embalaža (EUR)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Embalaža (EUR)"]].GetValue<decimal>();

            _odc.SMETIZbiranjeBioEur = row[nRow, koloneDict["Zbiranje BIO (EUR)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Zbiranje BIO (EUR)"]].GetValue<decimal>();
            _odc.SMETIObdelavaBioEur = row[nRow, koloneDict["Obdelava BIO - storitev (EUR)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Obdelava BIO - storitev (EUR)"]].GetValue<decimal>();
            _odc.SMETIZbiranjeMKOEur = row[nRow, koloneDict["Zbiranje MKO - storitev (EUR)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Zbiranje MKO - storitev (EUR)"]].GetValue<decimal>();
            _odc.SMETIObdelavaMKOEur = row[nRow, koloneDict["Obdelava MKO - storitev (EUR)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Obdelava MKO - storitev (EUR)"]].GetValue<decimal>();

            _odc.SMETIOdlaganjeMKOEur = 0;
            _odc.SMETIZnesek = row[nRow, koloneDict["Vrednost_bruto"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Vrednost_bruto"]].GetValue<decimal>();

            _odc.Znesek = _odc.SMETIZnesek ?? 0;
            _odc.Energija = 0;
            _odc.SMETIOpomba = row[nRow, koloneDict["Opomba"]].GetValue<string>();

            _odc.CreatedDate = DateTime.Now;
            _odc.UpdatedDate = DateTime.Now;

            await context.Odcitki.AddAsync(_odc);
            steviloDodanihOdcitkov++;

        }
        if (steviloDodanihOdcitkov > 0)
            await context.SaveChangesAsync();

        Console.WriteLine($"Dodano {steviloDodanihOdcitkov} steviloDodanihMeritev");
        Console.WriteLine($"Neuspešno dodanih {steviloNeuspelih} steviloNeuspelih.");
    }





    //--------------------------------------------------------------------------------------- KPK Kamnik
    [HttpGet]
    public static async Task ImportKpk(DataContext context)
    {

        Dictionary<string, int> seznamMerMest = new Dictionary<string, int>();
        Dictionary<int, string> seznamDobaviteljev = new Dictionary<int, string>();
        Dictionary<string, string> seznamMerMestaDobavitelji = new Dictionary<string, string>();
        Dictionary<string, string> seznamDobaviteljiMerMesta = new Dictionary<string, string>();
        foreach (MerilnoMesto mermesto in context.MerilnaMesta)
        {
            try
            {
                seznamMerMest.Add(mermesto.StMerilnegaMesta, mermesto.Id);
                seznamDobaviteljev.Add(mermesto.Id, mermesto.Dobavitelj);
                seznamMerMestaDobavitelji.Add(mermesto.StMerilnegaMesta, mermesto.Dobavitelj);
                seznamDobaviteljiMerMesta.Add(mermesto.Dobavitelj, mermesto.StMerilnegaMesta);
            }
            catch
            {
                Console.WriteLine("napaka dict");
            }
        }


        var path = "Data/Source/En_Kamnik_KPK_2020_2022_v3.xlsx";

        using var stream = System.IO.File.OpenRead(path);
        using var excelPackage = new ExcelPackage(stream);

        // get the first worksheet 
        var worksheet = excelPackage.Workbook.Worksheets[0];

        // define how many rows we want to process 
        var nEndRow = worksheet.Dimension.End.Row;

        // initialize the record counters 
        var steviloDodanihOdcitkov = 0;
        int steviloNeuspelih = 0;

        // kreiram dictionary prve vrste excla - imena polj
        var koloneDict = new Dictionary<string, int>();
        var prvaVrsta = worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column];
        for (var i = 1; i <= worksheet.Dimension.End.Column; i++)
        {
            koloneDict.Add(prvaVrsta[1, i].GetValue<string>(), i);
        }

        for (int nRow = 2; nRow <= nEndRow; nRow++)
        {
            var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];
            var _odc = new Odcitek();

            var pom = row[nRow, koloneDict["oznaka"]].GetValue<string>();
            if (seznamDobaviteljiMerMesta.ContainsKey(pom))
            {
                var _sifraMerMesta = seznamDobaviteljiMerMesta[pom];
                _odc.StMerilnegaMesta = _sifraMerMesta;
                _odc.NazivMerilnegaMesta = pom;
            }
            else
            {
                Console.WriteLine("KPK ni uparjen: " + pom);
            }
            var _idMerilnegaMesta = seznamMerMest[_odc.StMerilnegaMesta];
            _odc.IdMerilnegaMesta = _idMerilnegaMesta;
            _odc.EnergentTip = "VODA";
            _odc.Type = Convert.ToInt16(OdcitekTypeEnum.VODA);

            _odc.StevilkaRacuna = row[nRow, koloneDict["St_racuna"]].GetValue<string>();


            _odc.LetoMesec = row[nRow, koloneDict["mesec_obracuna"]].GetValue<string>();

            char[] karakterji = _odc.LetoMesec.ToCharArray();

            var leto = new StringBuilder();
            leto.Append(karakterji[0]);
            leto.Append(karakterji[1]);
            leto.Append(karakterji[2]);
            leto.Append(karakterji[3]);
            var leto1 = Convert.ToInt32(leto.ToString());

            var mesec = new StringBuilder();
            mesec.Append(karakterji[4]);
            mesec.Append(karakterji[5]);
            var mesec1 = Convert.ToInt32(mesec.ToString());
            DateTime currDatum = new DateTime(leto1, mesec1, 1);

            _odc.DatumOdcitka = currDatum;
            _odc.ObdobjeStoritveOd = currDatum;
            _odc.ObdobjeStoritveDo = currDatum;
            //         _odc.LetoMesec = currDatum.Year.ToString("00") + currDatum.Month.ToString("00");


            _odc.VODAPorabaM3 = row[nRow, koloneDict["voda_podj_k (m3)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["voda_podj_k (m3)"]].GetValue<decimal>();



            _odc.VODAVodarinaEur = row[nRow, koloneDict["Vodarina (EUR)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Vodarina (EUR)"]].GetValue<decimal>();
            //      _odc.VODAPrispevekEur = row[nRow, koloneDict["Prispevek (EUR)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Prispevek (EUR)"]].GetValue<decimal>();
            _odc.VODAOmrezninaEur = row[nRow, koloneDict["V_Omreznina (EUR)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["V_Omreznina (EUR)"]].GetValue<decimal>();


            _odc.VODAZnesek = row[nRow, koloneDict["Vodovod (EUR)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Vodovod (EUR)"]].GetValue<decimal>();
            _odc.KANALKanalscinaEur = row[nRow, koloneDict["Kanalscina (EUR)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Kanalscina (EUR)"]].GetValue<decimal>();
            _odc.KANALOmrezninaEur = row[nRow, koloneDict["K_omreznina (EUR)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["K_omreznina (EUR)"]].GetValue<decimal>();
            _odc.KANALCiscenjeVodeEur = row[nRow, koloneDict["ciscenje vode (EUR)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["ciscenje vode (EUR)"]].GetValue<decimal>();
            _odc.KANALCCNOmrezninaEur = row[nRow, koloneDict["CČN_omreznina (EUR)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["CČN_omreznina (EUR)"]].GetValue<decimal>();
            _odc.KANALZnesek = row[nRow, koloneDict["Kanalizacija (EUR)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Kanalizacija (EUR)"]].GetValue<decimal>();

            _odc.Znesek = row[nRow, koloneDict["SKUPAJ EUR"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["SKUPAJ EUR"]].GetValue<decimal>();
            _odc.Energija = 0;

            _odc.CreatedDate = DateTime.Now;
            _odc.UpdatedDate = DateTime.Now;

            if (seznamMerMest.ContainsKey(_odc.StMerilnegaMesta))
            {
                _odc.IdMerilnegaMesta = seznamMerMest[_odc.StMerilnegaMesta];
                MerilnoMesto? mmesto = await context.MerilnaMesta.FindAsync(_odc.IdMerilnegaMesta);

                _odc.MerilnoMesto = mmesto;
                _odc.StMerilnegaMesta = mmesto.StMerilnegaMesta;

                _odc.IdJavnegaObjekta = mmesto.IdJavnegaZavoda;
            }
            else
            {
                steviloNeuspelih++;
            }
            await context.Odcitki.AddAsync(_odc);
            steviloDodanihOdcitkov++;
        }
        if (steviloDodanihOdcitkov > 0)
            await context.SaveChangesAsync();

        Console.WriteLine($"Dodano {steviloDodanihOdcitkov} steviloDodanihMeritev");
        Console.WriteLine($"Neuspešno dodanih {steviloNeuspelih} steviloNeuspelih.");

    }



}
