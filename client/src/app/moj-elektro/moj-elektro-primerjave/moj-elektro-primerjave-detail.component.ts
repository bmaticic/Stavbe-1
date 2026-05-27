import { Component, Input, OnChanges, SimpleChanges, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { MojElektroService } from '../../_services/moj-elektro.service';
import { MojElektroMerilnoMesto } from '../../_models/MojElektroMerilnoMesto';
import { EgrafDnevniComponent } from '../egraf-dnevni/egraf-dnevni.component';
import { EgrafPoTednihComponent } from '../egraf-po-tednih/egraf-po-tednih.component';
import { EgrafPoUrahComponent } from '../egraf-po-urah/egraf-po-urah.component';
import { EgrafPoBlokihComponent } from '../egraf-po-blokih/egraf-po-blokih.component';

@Component({
  selector: 'app-moj-elektro-primerjave-detail',
  standalone: true,
  imports: [TabsModule, CommonModule, EgrafDnevniComponent, EgrafPoTednihComponent, EgrafPoUrahComponent, EgrafPoBlokihComponent],
  templateUrl: './moj-elektro-primerjave-detail.component.html',
  styleUrls: ['./moj-elektro-primerjave-detail.component.css']
})
export class MojElektroPrimerjaveDetailComponent implements OnChanges {
  @Input() selectedMesta: string[] = [];

  mojElektroService = inject(MojElektroService);
  selectedDetails: MojElektroMerilnoMesto[] = [];
  activeMesto?: MojElektroMerilnoMesto;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['selectedMesta']) {
      this.loadSelectedMesta();
    }
  }

  loadSelectedMesta(): void {
    this.selectedDetails = [];
    this.activeMesto = undefined;

    if (!this.selectedMesta || this.selectedMesta.length === 0) {
      return;
    }

    const loaded: Record<string, MojElektroMerilnoMesto> = {};
    let loadedCount = 0;

    this.selectedMesta.forEach(enotniIdentifikator => {
      this.mojElektroService.getMojElektroMerilnoMesto(enotniIdentifikator).subscribe({
        next: (merilnoMesto) => {
          loaded[enotniIdentifikator] = merilnoMesto;
          loadedCount++;
          if (!this.activeMesto) {
            this.setActiveMesto(merilnoMesto);
          }
          if (loadedCount === this.selectedMesta.length) {
            this.selectedDetails = this.selectedMesta
              .filter(id => loaded[id])
              .map(id => loaded[id]);
          }
        },
        error: () => {
          loadedCount++;
          if (loadedCount === this.selectedMesta.length) {
            this.selectedDetails = this.selectedMesta
              .filter(id => loaded[id])
              .map(id => loaded[id]);
          }
        }
      });
    });
  }

  setActiveMesto(merilnoMesto: MojElektroMerilnoMesto): void {
    this.activeMesto = merilnoMesto;
    this.mojElektroService.mojElektroSignal.set(merilnoMesto);
    this.mojElektroService.activeTabSignal.set(0);
  }

  isActive(enotniIdentifikator: string): boolean {
    return this.activeMesto?.enotniIdentifikator === enotniIdentifikator;
  }

  onTabSelect(index: number): void {
    this.mojElektroService.activeTabSignal.set(index);
  }
}
