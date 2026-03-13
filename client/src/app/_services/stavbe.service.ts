import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Stavba } from '../_models/stavba';
import { of, tap } from 'rxjs';
import { Photo } from '../_models/photo';
import { PaginatedResult } from '../_models/pagination';
import { StavbaParams } from '../_models/stavbaParams';
import { MerilnoMesto } from '../_models/merilno-mesto';
import { Ipoligon } from '../_models/poligon';
import { MojElektroMerilnoMesto } from '../_models/MojElektroMerilnoMesto';
import { Egraf } from '../_models/egraf';
@Injectable({
  providedIn: 'root'
})
export class StavbeService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  // stavbe = signal<Stavba[]>([]);
  paginatedResult = signal<PaginatedResult<Stavba[]> | null>(null)
  stavbaNaziv = signal<string>("");
  stavbaIdSignal = signal<number>(0);
  stavbaSignal = signal<Stavba | null>(null);


  getStavbe(stavbaParams: StavbaParams) {
    let params = this.setPaginationHeaders(stavbaParams.pageNumber, stavbaParams.pageSize);
    params = params.append('vrstaObjekta', stavbaParams.vrstaObjekta);
    params = params.append('ogrevanjeOznaka', stavbaParams.ogrevanjeOznaka);

    return this.http.get<Stavba[]>(this.baseUrl + 'stavbe', { observe: 'response', params })
      .subscribe({
        next: response => {
          this.paginatedResult.set({
            items: response.body as Stavba[],
            pagination: JSON.parse(response.headers.get('Pagination')!)
          })
        }
      })
  }

  private setPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    if (pageNumber && pageSize) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    }
    return params;
  }


  getStavbaPoNazivu(naziv: string) {
    // const stavba = this.stavbe().find(x => x.naziv === naziv);
    // if (stavba !== undefined) return of(stavba);
    return this.http.get<Stavba>(this.baseUrl + 'stavbe/' + naziv);
  }

  getMerilnaMestaStavbe(naziv: string) {
    return this.http.get<MerilnoMesto[]>(this.baseUrl + 'stavbe/merilna-mesta/' + naziv);
  }

  getMerilnaMestaStavbeStevilo(naziv: string): number {
    this.http.get<MerilnoMesto[]>(this.baseUrl + 'stavbe/merilna-mesta/' + naziv)
      .pipe(
        tap((x) => {
          return (x.length)
        })
      )
    return 0
  }

  getPodatkeZaMerilnoMesto(stMerilnegaMesta: string) {
    return this.http.get<Egraf>(this.baseUrl + 'stavbe/merilno-mesto/' + stMerilnegaMesta);
  }

  
  // geo točke
  getGeoTockeStavbe(naziv: string) {
    return this.http.get<Ipoligon>(this.baseUrl + 'stavbe/geo-tocke/' + naziv);
  }

  getStavba(id: number) {
    // const stavba = this.stavbe().find(x => x.id === id);
    // if (stavba !== undefined) return of(stavba);
    return this.http.get<Stavba>(this.baseUrl + 'stavbe/' + id);
  }

  updateStavba(stavba: Stavba) {
    return this.http.put(this.baseUrl + 'stavbe', stavba).pipe(
      // tap(() => {
      //   this.stavbe.update(stavbe => stavbe.map(s => s.naziv === stavba.naziv
      //     ? stavba : s))
      // })
    )
  }


  setMainPhoto(photo: Photo) {

    const idStavbeInIdPhoto = this.stavbaIdSignal().toString() + " " + photo.id.toString()

    return this.http.put(this.baseUrl + 'stavbe/set-main-photo/' + idStavbeInIdPhoto, {}).pipe(
      // tap(() => {
      //   this.stavbe.update(stavbe => stavbe.map(m => {
      //     if (m.photosStavbe.includes(photo)) {
      //       m.photoUrl = photo.url
      //     }
      //     return m;
      //   }))
      // })
    )
  }

  deletePhoto(photo: Photo) {

    const idStavbeInIdPhoto = this.stavbaIdSignal().toString() + " " + photo.id.toString()
    return this.http.delete(this.baseUrl + 'stavbe/delete-photo/' + idStavbeInIdPhoto).pipe(
      // tap(() => {
      //   this.stavbe.update(stavbe => stavbe.map(m => {
      //     if (m.photosStavbe.includes(photo)) {
      //       m.photosStavbe = m.photosStavbe.filter(x => x.id !== photo.id)
      //     }
      //     return m
      //   }))
      // })
    )
  }

}



// const parametri = new HttpParams()
// .set('stavbaNaziv', `${this.stavbaNaziv()}`)
// .set('PhotoId', `${photo.id}`)

