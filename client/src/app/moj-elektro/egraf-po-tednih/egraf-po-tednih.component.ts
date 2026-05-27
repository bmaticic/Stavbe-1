import { Component, inject, Input, OnChanges, OnInit, SimpleChanges, effect } from '@angular/core';
import { MojElektroService } from '../../_services/moj-elektro.service';
import { FetchDataService } from '../../_services/fetch-data.service';
import { IRange, RangePreDefinirani } from '../../_models/i-range';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MojElektroGrafComponent } from "../moj-elektro-graf/moj-elektro-graf.component";
import { Observable } from 'rxjs';
import { IEchartData } from '../../_models/echart-data';
import { formatDateAsString } from '../../_utils/date-utils';
import { fetchEchartData } from '../../_utils/fetch-data-util';


@Component({
  selector: 'app-egraf-po-tednih',
  standalone: true,
  imports: [CommonModule, BsDatepickerModule, FormsModule, MojElektroGrafComponent],
  templateUrl: './egraf-po-tednih.component.html',
  styleUrl: './egraf-po-tednih.component.css'
})
export class EgrafPoTednihComponent implements OnInit, OnChanges {
  @Input() merilnoMesto?: any;
  mojElektroService = inject(MojElektroService);
  fetchDataService = inject(FetchDataService);
  ranges: IRange[] = new RangePreDefinirani().getRanges();
  selectedRange: IRange = this.ranges[0];

  // UI / state
  isLoading = false;
  error: string | null = null;
  chartData: any = null; // replace `any` with a proper model when available

  public eChartsEnergijaAPlus: Observable<IEchartData> =
    new Observable<IEchartData>();

  // Chart styling configuration
  lineStyle = {
    width: 0.4,
    opacity: 0.7
  };

  legendConfig = {
    top: 40,
    type: 'scroll' as const,
    selectedMode: true
  };


  // --- hard coded
  public sifraEnergijaMoc: string = "EnergijaAPlus";  // EnergijaAPlus, PrejetaDelovnaMoc
  public sifraAgregacija: string = "PoLetihPoTednihPoDnevih";  // PoLetihPoMesecih, PoLetihPoTednihPoDnevih, PoLetihPoDnevihPoUrah, PoLetihPoMesecihPoBlokih

  public letoOD: number = 2022;          // --- hard coded datumOD   datumDO
  public letoDO: number = 2026;
  public mesecOD: number = 1;
  public mesecDO: number = 1;
  
  constructor() {
    // Watch for merilno mesto changes and re-fetch data
    effect(() => {
      const merilnoMesto = this.mojElektroService.mojElektroSignal();
      if (merilnoMesto) {
        this.fetchData();
      }
    });

    effect(() => {
      const activeTab = this.mojElektroService.activeTabSignal();
      if (activeTab === 1 && this.mojElektroService.mojElektroSignal()) {
        this.fetchData();
      }
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['merilnoMesto'] && this.merilnoMesto) {
      this.fetchData();
    }
  }

  ngOnInit(): void {
    // Ensure selectedRange has a sensible default value (last 7 days)
    if (!this.selectedRange.value || this.selectedRange.value.length !== 2) {
      const end = new Date();
      const start = new Date(end);
      start.setDate(end.getDate() - 6); // last 7 days including today
      this.selectedRange.value = [start, end];
      this.selectedRange.label = `${formatDateAsString(start)} - ${formatDateAsString(end)}`;
    }
  }

  onValueChange($event: (Date | undefined)[] | undefined) {
    if ($event && $event.length === 2) {
      this.selectedRange.value = $event as Date[];
      this.selectedRange.label = `${formatDateAsString($event[0]!)} - ${formatDateAsString($event[1]!)}`;
      // Reload data for the newly selected range
      this.fetchData();
    }
  }

  /**
   * Fetches data from the FetchDataService for the currently selected.
   */
  fetchData(): void {
    fetchEchartData(
      this.fetchDataService,
      this.mojElektroService,
      this.selectedRange,
      this.sifraAgregacija,
      this.sifraEnergijaMoc,
      this.letoOD,
      this.letoDO,
      this.mesecOD,
      this.mesecDO,
      (loading) => { this.isLoading = loading; },
      (error) => { this.error = error; },
      (data) => { this.chartData = data; },
      this.merilnoMesto?.enotniIdentifikator,
      this.merilnoMesto?.idJavnegaObjekta
    );
  }


}
