import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { MojElektroMerilnoMesto } from '../_models/MojElektroMerilnoMesto';
import { Egraf } from '../_models/egraf';

@Injectable({
  providedIn: 'root'
})
export class MojElektroService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;


  getMojElektroMerilnaMesta(naziv: string) {
    return this.http.get<MojElektroMerilnoMesto[]>(this.baseUrl + 'mojElektro/moj-elektro-merilna-mesta/' + naziv);
  }

  getPodatkeZaMojElektroMerilnoMesto(enotniIdentifikator: string) {
    return this.http.get<Egraf>(this.baseUrl + 'mojElektro/moj-elektro-merilno-mesto/' + enotniIdentifikator);
  } 
}
