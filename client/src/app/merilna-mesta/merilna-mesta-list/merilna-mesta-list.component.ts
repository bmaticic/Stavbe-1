import { Component, inject, OnInit } from '@angular/core';
import { StavbeService } from '../../_services/stavbe.service';
import { MerilnoMesto } from '../../_models/merilno-mesto';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { FormsModule } from '@angular/forms';
import { Egraf } from '../../_models/egraf';
import { CollapseDirective } from 'ngx-bootstrap/collapse';
import { EchartGrafComponent } from '../echart-graf/echart-graf.component';

@Component({
  selector: 'app-merilna-mesta-list',
  imports: [ButtonsModule, FormsModule, CollapseDirective, EchartGrafComponent],
  templateUrl: './merilna-mesta-list.component.html',
  styleUrl: './merilna-mesta-list.component.css'
})
export class MerilnaMestaListComponent implements OnInit {
  stavbeService = inject(StavbeService);
  merilnaMesta: MerilnoMesto[] = [];

  isCollapsed = false;
  selectedMerilnoMestoGraf: Egraf | null = null;

  ngOnInit(): void {
    this.loadMerilnaMesta();
    
  }

  loadMerilnaMesta() {
    this.stavbeService.getMerilnaMestaStavbe(this.stavbeService.stavbaNaziv()).subscribe({
      next: (merilnaMesta) => {
        this.merilnaMesta = merilnaMesta;
      },
      error: (error) => {
        console.log(error);
      }
    });
  }

    onMerilnoMestoSelected(merilnoMesto: MerilnoMesto) {
      this.stavbeService.getPodatkeZaMerilnoMesto(merilnoMesto.stMerilnegaMesta).subscribe({
        next: (data) => {
          this.selectedMerilnoMestoGraf = data; // Store the data for the selected merilno mesto
         // console.log(data);
  
          this.isCollapsed = !this.isCollapsed; // Toggle collapse state
        }
      });
    }
  

}
