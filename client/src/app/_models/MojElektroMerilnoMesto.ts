import { Photo } from "./photo"
import { Stavba } from "./stavba"

export interface MojElektroMerilnoMesto {
  enotniIdentifikator: string
  sifraJavnegaObjekta: string
  nazivJavnegaObjekta: string
  photoUrl: string     // slika stavbe

  gsrnMM: string
  naziv: string
  naslov: string
  rtp: string
  sNizvod: string
  tp: string
  nNizvod: string
  dobavitelj: string
  idJavnegaObjekta: number
  stavba: Stavba
  photoStavbe: Photo
  meritve15min: any[]
}