using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities.MojElektro;
using Microsoft.EntityFrameworkCore;

[Table("MojElektro15MinMeritve")]
[Index(nameof(StMerilnegaMesta), nameof(TimeStamp), IsUnique = false, Name = "IX_StMerilnegaMesta_TimeStamp")]
public class MojElektro15MinMeritev
{
    [Key]
    [Required]
    public int Id { get; set; }
    [Required]
    public string? StMerilnegaMesta { get; set; }
    [Required]
    public int IdMerilnegaMesta { get; set; }
    [Required]
    public DateTime TimeStamp { get; set; }
    public int Leto { get; set; }
    public int Mesec { get; set; }
    public string? LetoMesec { get; set; }
    public string? LetoMesecBlok { get; set; }
    public string? LetoTedenDan { get; set; }
    public string? LetoTedenDanUra { get; set; }
    public string? LetoDanUra { get; set; }

    [Column(TypeName = "decimal(7,4)")]
    public decimal Energija_A_plus { get; set; }
    [Column(TypeName = "decimal(7,4)")]
    public decimal Energija_A_minus { get; set; }
    [Column(TypeName = "decimal(7,4)")]
    public decimal Energija_R_plus { get; set; }
    [Column(TypeName = "decimal(7,4)")]
    public decimal Energija_R_minus { get; set; }
    [Column(TypeName = "decimal(7,4)")]
    public decimal PrejetaDelovnaMoc { get; set; }  // P+
    [Column(TypeName = "decimal(7,4)")]
    public decimal OddanaDelovnaMoc { get; set; }   // P-
    [Column(TypeName = "decimal(7,4)")]
    public decimal PrejetaJalovaMoc { get; set; }   // Q+
    [Column(TypeName = "decimal(7,4)")]
    public decimal OddanaJalovaMoc { get; set; }   // Q-
    [Required]
    public int Blok { get; set; }

    #region
    /// <summary>
    /// Id merilnega mesta MojElektro na katerega je povezana ta meritev
    /// </summary>
    [ForeignKey(nameof(MerilnoMestoMojElektro))]
    public int IdMerilnegaMestaMojElektro { get; set; }
    #endregion


    #region Navigation Properties
    public MojElektroMerilnoMesto? MerilnoMestoMojElektro { get; set; } = null!;
    #endregion




}
