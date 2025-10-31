using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Entities;

[Table("GeoTocke")]
[Index(nameof(SifraObjekta), IsUnique = false, Name = "IX_SifraObjekta")]
public class GeoTocka
{
        [Key]
        [Required]
        public int Id { get; set; }

        public string SifraObjekta { get; set; } = null!;
        public string? SifraMerilnegaMesta { get; set; } = null!;
        public int? Zaporedje { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal? Lat { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal? Lng { get; set; }
        public string? JavniObjektSifraJavnegaObjekta { get; set; } = null!;


        //  Kamnik
        public int? FID { get; set; }
        public string? OZN_obj { get; set; } = null!;
        public string? Naziv { get; set; } = null!;
        public string? SID { get; set; } = null!;
        public string? SIFKO { get; set; } = null!;
        public string? ST_stevilka { get; set; } = null!;
        public string? Ozn_tock { get; set; } = null!;

        [Column(TypeName = "decimal(9,6)")]
        public decimal? DDLat { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal? DDLon { get; set; }

        // -- Kamnik

        #region
        /// <summary>
        /// Id Javnega objekta na katerega je povezana ta geotočka
        /// </summary>
        [ForeignKey(nameof(Stavba))]
        public int IdJavnegaObjekta { get; set; }
        #endregion


        #region Navigation Properties
        /// <summary>
        /// 
        /// </summary>
        public Stavba? Stavba { get; set; } = null!;
        #endregion



}
