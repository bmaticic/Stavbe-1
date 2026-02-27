import { Component, effect, inject, Input, OnInit } from '@angular/core';
import { EchartGrafComponent } from "../../merilna-mesta/echart-graf/echart-graf.component";                         // ../merilna-mesta/echart-graf/echart-graf.component
import { Egraf } from '../../_models/egraf';
import { MojElektroService } from '../../_services/moj-elektro.service';
import { MojElektroMerilnoMesto } from '../../_models/MojElektroMerilnoMesto';

import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MojElektroGrafComponent } from "../moj-elektro-graf/moj-elektro-graf.component";
import { Observable } from 'rxjs';
import { IEchartData } from '../../_models/echart-data';


@Component({
  selector: 'app-egraf-vsi-podatki',
  imports: [CommonModule, FormsModule, EchartGrafComponent],
  templateUrl: './egraf-vsi-podatki.component.html',
  styleUrl: './egraf-vsi-podatki.component.css'
})
export class EgrafVsiPodatkiComponent {
  mojElektroService = inject(MojElektroService);

  // Variable to hold the selected merilno mesto
  selectedMerilnoMestoGraf: Egraf | null = null;


  constructor() {
    effect(() => {
      const merilnoMesto = this.mojElektroService.mojElektroSignal();
      if (merilnoMesto) {
        this.fetchData();
      }
    });
  }

  fetchData() {
    const merilnoMesto = this.mojElektroService.mojElektroSignal();
    if (!merilnoMesto) {
      console.warn("No selected merilno mesto");
      return;
    }
    this.mojElektroService.getPodatkeZaMojElektroMerilnoMesto(merilnoMesto.enotniIdentifikator).subscribe({
      next: (data) => {
        this.selectedMerilnoMestoGraf = data; // Store the data for the selected merilno mesto
        console.log(data);

      }
    });
  }


}
