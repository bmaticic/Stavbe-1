using System;

namespace API.Entities;

public class HttpParametri
{
        public int idJavnegaObjekta { get; set; }
        public string? enotniIdentifikator { get; set; } 
        public string? aggregation { get; set; }        // PoLetihPoMesecih, PoLetihPoTednihPoDnevih, ...
        public string? sifraEnergijaMoc { get; set; }      // EnergijaAPlus, PrejetaDelovnaMoc, ...
        public int letoOD { get; set; }
        public int letoDO { get; set; }
        public int mesecOD { get; set; }
        public int mesecDO { get; set; }
        public string? stroskiAliEnergija { get; set; }

}
