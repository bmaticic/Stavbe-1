import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { MojElektroMerilnoMesto } from '../_models/MojElektroMerilnoMesto';
import { Egraf } from '../_models/egraf';
import { IRange } from '../_models/i-range';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MojElektroService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  headers = new HttpHeaders().set('Content-Type', 'application/json');


  mojElektroSignal = signal<MojElektroMerilnoMesto | null>(null);
  selectedRangeSignal = signal<IRange | null>(null);
  activeTabSignal = signal<number>(0); // Track which tab is active (0-based index)

  // moj-elektro merilna mesta za stavbo
  getMojElektroMerilnaMesta(naziv: string) {
    return this.http.get<MojElektroMerilnoMesto[]>(this.baseUrl + 'mojElektro/moj-elektro-merilna-mesta/' + naziv);
  }

  // podatki za Egraf za moj-elektro merilno mesto
  getPodatkeZaMojElektroMerilnoMesto(enotniIdentifikator: string) {
    return this.http.get<Egraf>(this.baseUrl + 'mojElektro/moj-elektro-merilno-mesto/' + enotniIdentifikator);
  }

  // seznam vseh moj-elektro merilnih mest v bazi
  getMojElektroMerilnaMestaVsa() {
    return this.http.get<MojElektroMerilnoMesto[]>(this.baseUrl + 'mojElektro/moj-elektro-merilna-mesta-vsa');
  }


  getMojElektroMerilnoMesto(enotniIdentifikator: string) {
    return this.http.get<MojElektroMerilnoMesto>(this.baseUrl + 'mojElektro/' + enotniIdentifikator);
  }


  // različno agregirani podatki da pridobim Egraf za moj-elektro za eno merilno mesto
  get_agregirane_PodatkeZaMojElektroMerilnoMesto(idJavnegaObjekta: number, enotniIdentifikator: string, aggregation: string, sifraEnergijaMoc: string,
                letoOD: number, letoDO: number, mesecOD: number, mesecDO: number): Observable<any> {

    const params = new HttpParams()
    .set('idJavnegaObjekta', `${idJavnegaObjekta}`)
    .set('enotniIdentifikator', `${enotniIdentifikator}`)
    .set('aggregation', `${aggregation}`)
    .set('sifraEnergijaMoc', `${sifraEnergijaMoc}`)
    .set('letoOD', `${letoOD}`)
    .set('letoDO', `${letoDO}`)
    .set('mesecOD', `${mesecOD}`)
    .set('mesecDO', `${mesecDO}`);

    return this.http.get<Egraf>(this.baseUrl + 'mojElektroAgregirani/moj-elektro-agregirani-za-egraf' , { params });
  }


}
