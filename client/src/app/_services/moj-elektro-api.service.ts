import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MojElektroApiService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  getMeterReadings(usagePoint: string, startTime: string, endTime: string, option: string, apiToken: string) {
    const params = new HttpParams()
      .set('usagePoint', usagePoint)
      .set('startTime', startTime)
      .set('endTime', endTime)
      .set('option', option);
    return this.http.get<any>(
      `${this.baseUrl}mojelektroexternaldata/meter-readings`,
      {
        params,
        headers: { 'X-API-TOKEN': apiToken }
      }
    );
  }
}