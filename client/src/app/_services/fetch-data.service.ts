import { Injectable, inject } from '@angular/core';
import { MojElektroService } from './moj-elektro.service';
import { IEchartData } from '../_models/echart-data';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class FetchDataService {
  private mojElektroService = inject(MojElektroService);

  /**
   * Fetches aggregated data for a moj-elektro metering point and returns it as IEchartData
   */
  fetchEchartData(
    idJavnegaObjekta: number,
    enotniIdentifikator: string,
    aggregation: string,
    sifraEnergijaMoc: string,
    letoOD: number,
    letoDO: number,
    mesecOD: number,
    mesecDO: number
  ): Observable<IEchartData> {
    return this.mojElektroService.get_agregirane_PodatkeZaMojElektroMerilnoMesto(
      idJavnegaObjekta,
      enotniIdentifikator,
      aggregation,
      sifraEnergijaMoc,
      letoOD,
      letoDO,
      mesecOD,
      mesecDO
    ).pipe(
      map(data => ({
        chartLabel: data.chartLabel,
        enotaMere: data.enotaMere,
        axisXLabels: data.axisXLabels,
        linesData: data.lines.map((item: any[]) => item.values),
        legend: data.lines.map((item: { type: any }) => item.type),
        legendaOriginal: data.lines.map((item: { typeOriginal: any }) => item.typeOriginal),
        legendaOriginalDistinct: data.legendaOriginal,
        legendaLeto: data.lines.map((item: { type: any }) => item.type.substr(0, 4))
      }))
    );
  }
}
