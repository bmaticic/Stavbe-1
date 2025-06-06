import { Component, inject, Input, OnInit } from '@angular/core';
import { StavbeService } from '../../_services/stavbe.service';
import { CollapseDirective } from 'ngx-bootstrap/collapse';
import { EchartGrafComponent } from "../echart-graf/echart-graf.component";
import { Egraf } from '../../_models/egraf';
import { MojElektroService } from '../../_services/moj-elektro.service';
import { MojElektroMerilnoMesto } from '../../_models/MojElektroMerilnoMesto';

@Component({
  selector: 'app-moj-elektro-m-mesta',
  imports: [CollapseDirective, EchartGrafComponent],
  templateUrl: './moj-elektro-m-mesta.component.html',
  styleUrl: './moj-elektro-m-mesta.component.css'
})
export class MojElektroMMestaComponent implements OnInit {
  stavbeService = inject(StavbeService);
  mojElektroService = inject(MojElektroService);
  // Array to hold the merilna mesta data
  mojElektroMerilnaMesta: MojElektroMerilnoMesto[] = [];
  // Variable to control the collapse state of the merilno mesto details
  isCollapsed = false;
  // Variable to hold the selected merilno mesto
  selectedMerilnoMestoGraf: Egraf | null = null;

  ngOnInit(): void {
    this.loadMojElektroMerilnaMesta();
  }

  loadMojElektroMerilnaMesta() {
    this.mojElektroService.getMojElektroMerilnaMesta(this.stavbeService.stavbaNaziv()).subscribe({
      next: (mojElektroMerilnaMesta) => {
        this.mojElektroMerilnaMesta = mojElektroMerilnaMesta;
      },
      error: (error) => {
        console.log(error);
      }
    });
  }

  onMerilnoMestoSelected(merilnoMesto: MojElektroMerilnoMesto) {
    this.mojElektroService.getPodatkeZaMojElektroMerilnoMesto(merilnoMesto.enotniIdentifikator).subscribe({
      next: (data) => {
        this.selectedMerilnoMestoGraf = data; // Store the data for the selected merilno mesto
        console.log(data);

        this.isCollapsed = !this.isCollapsed; // Toggle collapse state
      }
    });
  }



}
