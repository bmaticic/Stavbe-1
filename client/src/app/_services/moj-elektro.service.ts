import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { MojElektroMerilnoMesto } from '../_models/MojElektroMerilnoMesto';
import { Egraf } from '../_models/egraf';

@Injectable({
  providedIn: 'root'
})
export class MojElektroService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  mojElektroSignal = signal<MojElektroMerilnoMesto | null>(null);

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


}
