// -------------------------------------------------------

// VNOS PODATKOV ZEMELJSKI PLIN         /SeedToplota/ImportZemPlin

// Import Pogodbena Toplota Petrol      /SeedToplota/ImportPogodbenaToplotaPetrol

// Import Drugi Energenti               /SeedToplota/ImportDrugiEnergenti

// -------------------------------------------------------


using System;
using System.Text;
using API.Data.Enums;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace API.Data.SeedData;

public class SeedToplota
{
        //----------------------------------------------------------------------------------------------------  ImportZemPlin

    public static async Task ImportZemPlin(DataContext context)
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
        Dictionary<string, int> seznamNeuvrscenihMerilnihMest = new Dictionary<string, int>();
        int steviloNeuspelihMerilnihMest = 0;


        var path = "Data/Source/En_Kamnik_Zemeljski plin_2020_2022_v2.xlsx";
        // "Data/Source/En_Kamnik_Zemeljski plin_2020_2021_v6.xlsx");

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
            var row = worksheet.Cells[
                nRow, 1, nRow, worksheet.Dimension.End.Column];

            var _odc = new Odcitek();

            var pom = row[nRow, koloneDict["Odjemno mesto"]].GetValue<string>();
            if (seznamDobaviteljiMerMesta.ContainsKey(pom))
            {
                var _sifraMerMesta = seznamDobaviteljiMerMesta[pom];
                _odc.StMerilnegaMesta = _sifraMerMesta;
                _odc.NazivMerilnegaMesta = pom;
            }
            else
            {
                Console.WriteLine("zem plin ni uparjen: " + pom);
                continue;
            }

            var _idMerilnegaMesta = seznamMerMest[_odc.StMerilnegaMesta];
            _odc.IdMerilnegaMesta = _idMerilnegaMesta;



            _odc.EnergentTip = "OGREVANJE";
            _odc.Type = Convert.ToInt16(OdcitekTypeEnum.OGREVANJE);
            _odc.TipOgrevanja = "ZemeljskiPlin";
            _odc.TipOgrevanjaOznaka = "ZP";

            _odc.StevilkaRacuna = row[nRow, koloneDict["Številka računa"]].GetValue<string>();
            _odc.LetoMesec = row[nRow, koloneDict["Mesec_obrac"]].GetValue<string>();

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

            _odc.Znesek = row[nRow, koloneDict["Skupaj-bruto"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Skupaj-bruto"]].GetValue<decimal>();
            _odc.Energija = row[nRow, koloneDict["Poraba [kWh]"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Poraba [kWh]"]].GetValue<decimal>();

            _odc.PLINPorabaKWh = row[nRow, koloneDict["Poraba [kWh]"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Poraba [kWh]"]].GetValue<decimal>();
            _odc.PLINDistribucija = row[nRow, koloneDict["Distribucija"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Distribucija"]].GetValue<decimal>();

            _odc.PLINSkupajBruto = _odc.Znesek;

            _odc.PLINOdjemnaMoc = row[nRow, koloneDict["Odjemna moč [kW]"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Odjemna moč [kW]"]].GetValue<decimal>();

            _odc.PLINZemeljskiPlin = row[nRow, koloneDict["Zemeljski plin"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Zemeljski plin"]].GetValue<decimal>();

            _odc.PLINPrispevki = row[nRow, koloneDict["Prispevki"]].GetValue<decimal>();

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


        //---------------------------------------------------------------------------------------------------- Pogodbena Toplota Petrol

        public static async Task ImportPogodbenaToplotaPetrol(DataContext context)
        {
            // prevents non-development environments from running this method
            int stevec = 1;
            Dictionary<string, int> enumiTipOgrevanjaOznakaDict = new Dictionary<string, int>();
            foreach (string str in Enum.GetNames(typeof(TipOgrevanjaOznakaEnum)))
            {
                enumiTipOgrevanjaOznakaDict.Add(str, stevec++);
            }
            Dictionary<int, string> enumiTipOgrevanjaDict = new Dictionary<int, string>();
            stevec = 1;
            foreach (string str in Enum.GetNames(typeof(TipOgrevanjaEnum)))
            {
                enumiTipOgrevanjaDict.Add(stevec++, str);
            }

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
        var path = "Data/Source/En_Kamnik_Pogodbena_toplota_Petrol_2020_2022_v2.xlsx";
           // "Data/Source/En_Kamnik_Pogodbena_toplota_Petrol_2020_2021_v6.xlsx");

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
                if (seznamDobaviteljiMerMesta.ContainsKey(pom))
                {
                    var _sifraMerMesta = seznamDobaviteljiMerMesta[pom];
                    _odc.StMerilnegaMesta = _sifraMerMesta;
                }
                else
                {
                    Console.WriteLine("ni ključa: " + pom);                                 //  tu lovimo nova mer-mesta
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

                _odc.EnergentTip = "OGREVANJE";
                _odc.Type = Convert.ToInt16(OdcitekTypeEnum.OGREVANJE);



                string _ogrevanjeOznaka = row[nRow, koloneDict["Energent"]].GetValue<string>();


                var zaporedna = enumiTipOgrevanjaOznakaDict[_ogrevanjeOznaka];


                string _ogrevanjeTekst = enumiTipOgrevanjaDict[zaporedna];
                _odc.TipOgrevanjaOznaka = _ogrevanjeOznaka;
                _odc.TipOgrevanja = _ogrevanjeTekst;
                _odc.StevilkaRacuna = row[nRow, koloneDict["ŠT. RAČUNA"]].GetValue<string>();

                _odc.LetoMesec = row[nRow, koloneDict["MESEC/LETO"]].GetValue<string>();
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

                _odc.Znesek = row[nRow, koloneDict["Skupaj Z DDV"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Skupaj Z DDV"]].GetValue<decimal>();
                _odc.Energija = row[nRow, koloneDict["Poraba (kWh)"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Poraba (kWh)"]].GetValue<decimal>();
                _odc.DOEnergijaEuro = row[nRow, koloneDict["Energija [EUR]"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Energija [EUR]"]].GetValue<decimal>();
                _odc.DOObracunskaMocEuro = row[nRow, koloneDict["OBRAČUNSKA MOČ [EUR]"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["OBRAČUNSKA MOČ [EUR]"]].GetValue<decimal>();

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



        //---------------------------------------------------------------------------------------------------------------- Drugi Energenti

        public static async Task ImportDrugiEnergenti(DataContext context)
        {
            int stevec = 1;
            Dictionary<string, int> enumiTipOgrevanjaOznakaDict = new Dictionary<string, int>();
            foreach (string str in Enum.GetNames(typeof(TipOgrevanjaOznakaEnum)))
            {
                enumiTipOgrevanjaOznakaDict.Add(str, stevec++);
            }
            Dictionary<int, string> enumiTipOgrevanjaDict = new Dictionary<int, string>();
            stevec = 1;
            foreach (string str in Enum.GetNames(typeof(TipOgrevanjaEnum)))
            {
                enumiTipOgrevanjaDict.Add(stevec++, str);
            }

            // hardcoded 7  !!!!!!
            var tipOgrevanjaDict = new Dictionary<string, string>();
            for (var i = 1; i <= 7; i++)
            {
                tipOgrevanjaDict.Add(Enum.GetName(typeof(TipOgrevanjaOznakaEnum), i), Enum.GetName(typeof(TipOgrevanjaEnum), i));
            }


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
        var path = "Data/Source/En_Kamnik_Drugi_energenti_2020_2022_v2.xlsx";
          //      "Data/Source/En_Kamnik_Drugi_energenti_2020_2021_v6.xlsx");

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


                var pom = row[nRow, koloneDict["Odjemo_mesto"]].GetValue<string>();
                if (seznamDobaviteljiMerMesta.ContainsKey(pom))
                {
                    var _sifraMerMesta = seznamDobaviteljiMerMesta[row[nRow, koloneDict["Odjemo_mesto"]].GetValue<string>()];
                    _odc.StMerilnegaMesta = _sifraMerMesta;
                }
                else
                {
                    Console.WriteLine("ni ključa " + pom);
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


                _odc.EnergentTip = "OGREVANJE";
                _odc.Type = Convert.ToInt16(OdcitekTypeEnum.OGREVANJE);

                _odc.TipOgrevanjaOznaka = row[nRow, koloneDict["Energent"]].GetValue<string>();
                _odc.TipOgrevanja = tipOgrevanjaDict[_odc.TipOgrevanjaOznaka];

                _odc.StevilkaRacuna = row[nRow, koloneDict["Št_računa"]].GetValue<string>();

                _odc.LetoMesec = row[nRow, koloneDict["Obracun_mesec"]].GetValue<string>();
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

                _odc.Znesek = row[nRow, koloneDict["Znesek_bruto_2"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Znesek_bruto_2"]].GetValue<decimal>();
                _odc.Energija = row[nRow, koloneDict["Energija_kWh"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Energija_kWh"]].GetValue<decimal>();

                _odc.ElkoLbUnpEnotaMere = row[nRow, koloneDict["Enota"]].GetValue<string>();

                _odc.ElkoLbUnpKolicina = row[nRow, koloneDict["Količina"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Količina"]].GetValue<decimal>();
                _odc.ElkoLbUnpEnergijskiEkvivalent = row[nRow, koloneDict["Energijski_ekvivalent"]].GetValue<string>() == "" ? 0 : row[nRow, koloneDict["Energijski_ekvivalent"]].GetValue<decimal>();

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



}
