import { Component, inject, OnInit } from '@angular/core';
import { StavbeService } from '../../_services/stavbe.service';
import { MojElektroService } from '../../_services/moj-elektro.service';
import { MojElektroMerilnoMesto } from '../../_models/MojElektroMerilnoMesto';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MojElektroPrimerjaveDetailComponent } from './moj-elektro-primerjave-detail.component';

@Component({
  selector: 'app-moj-elektro-primerjave',
  imports: [RouterLink, FormsModule, CommonModule, MojElektroPrimerjaveDetailComponent],
  templateUrl: './moj-elektro-primerjave.component.html',
  styleUrl: './moj-elektro-primerjave.component.css'
})
export class MojElektroPrimerjaveComponent implements OnInit {
  stavbeService = inject(StavbeService);
  mojElektroService = inject(MojElektroService);
  mojElektroMerilnaMesta: MojElektroMerilnoMesto[] = [];
  selectedMesta: Set<string> = new Set();
  selectedMestaArray: string[] = [];
  showComparison = false;

  ngOnInit(): void {
    this.loadMojElektroMerilnaMesta();
  }

  loadMojElektroMerilnaMesta() {
    this.mojElektroService.getMojElektroMerilnaMestaVsa().subscribe({
      next: (mojElektroMerilnaMesta) => {
        this.mojElektroMerilnaMesta = mojElektroMerilnaMesta;
      },
      error: (error) => {
        console.log(error);
      }
    });
  }

  onCheckboxChange(enotniIdentifikator: string, checked: boolean) {
    if (checked) {
      this.selectedMesta.add(enotniIdentifikator);
    } else {
      this.selectedMesta.delete(enotniIdentifikator);
    }
  }

  isSelected(enotniIdentifikator: string): boolean {
    return this.selectedMesta.has(enotniIdentifikator);
  }

  startComparison(): void {
    this.selectedMestaArray = Array.from(this.selectedMesta);
    this.showComparison = true;
  }
}
