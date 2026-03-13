import { Component, effect, inject, Input, OnInit } from '@angular/core';
import { EchartGrafComponent } from "../../merilna-mesta/echart-graf/echart-graf.component";
import { Egraf } from '../../_models/egraf';
import { MojElektroService } from '../../_services/moj-elektro.service';
import { MojElektroMerilnoMesto } from '../../_models/MojElektroMerilnoMesto';

import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MojElektroGrafComponent } from "../moj-elektro-graf/moj-elektro-graf.component";
import { Observable } from 'rxjs';
import { IEchartData } from '../../_models/echart-data';


@Component({
  selector: 'app-egraf-vsi-podatki-grupirani',
  imports: [CommonModule, FormsModule, EchartGrafComponent],
  templateUrl: './egraf-vsi-podatki-grupirani.component.html',
  styleUrl: './egraf-vsi-podatki-grupirani.component.css'
})
export class EgrafVsiPodatkiGrupiraniComponent {
  mojElektroService = inject(MojElektroService);

  // Variable to hold the selected merilno mesto
  selectedMerilnoMestoGraf: Egraf | null = null;

  // Grouping options
  groupingOptions = [
    { value: 'hours', label: 'Po urah' },
    { value: 'days', label: 'Po dnevih' },
   // { value: 'weeks', label: 'Po tednih' },
    { value: 'months', label: 'Po mesecih' }
  ];
  selectedGrouping: string = 'months';

  constructor() {
    effect(() => {
      const merilnoMesto = this.mojElektroService.mojElektroSignal();
      if (merilnoMesto) {
        this.fetchData();
      }
    });

    effect(() => {
      const activeTab = this.mojElektroService.activeTabSignal();
      if (activeTab === 5 && this.mojElektroService.mojElektroSignal()) {
        this.fetchData();
      }
    });
  }

  onGroupingChange() {
    this.fetchData();
  }

  fetchData() {
    const merilnoMesto = this.mojElektroService.mojElektroSignal();
    if (!merilnoMesto) {
      console.warn("No selected merilno mesto");
      return;
    }
    this.mojElektroService.getPodatkeZaMojElektroMerilnoMestoGroupedBy(merilnoMesto.enotniIdentifikator, this.selectedGrouping).subscribe({
      next: (data) => {
        this.selectedMerilnoMestoGraf = data; // Store the data for the selected merilno mesto
        console.log(data);
      }
    });
  }
}
