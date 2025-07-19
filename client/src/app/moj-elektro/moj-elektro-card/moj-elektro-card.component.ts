import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryModule, GalleryItem, ImageItem } from 'ng-gallery';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';

import { MojElektroService } from '../../_services/moj-elektro.service';
import { MojElektroMerilnoMesto } from '../../_models/MojElektroMerilnoMesto';
import { FormsModule } from '@angular/forms';

interface IRange {
  value: Date[];
  label: string;
}

@Component({
  selector: 'app-moj-elektro-card',
  imports: [TabsModule, GalleryModule, CommonModule, BsDatepickerModule, FormsModule],  // RouterLink
  templateUrl: './moj-elektro-card.component.html',
  styleUrl: './moj-elektro-card.component.css'
})
export class MojElektroCardComponent {
  private mojElektroService = inject(MojElektroService);
  private route = inject(ActivatedRoute);

  mojElektroMerilnoMesto?: MojElektroMerilnoMesto;

  images: GalleryItem[] = [];

  ranges: IRange[] = [{
    value: [new Date(new Date().setDate(new Date().getDate() - 7)), new Date()],
    label: 'Zadnjih 7 dni',
  }, {
    value: [new Date(), new Date(new Date().setDate(new Date().getDate() + 30))],
    label: 'Zadnjih 30 dni'
  }, {
    value: [new Date(new Date().setFullYear(new Date().getFullYear() - 1)), new Date()],
    label: 'Zadnjih 12 mesecev'
  }, {
    value: [new Date(new Date().setFullYear(new Date().getFullYear() - 5)), new Date()],
    label: 'Zadnjih 5 let'
  }

];
  selectedRange: IRange = this.ranges[0];

  ngOnInit(): void {
    // Select the first range by default
    if (this.ranges.length > 0) {
      this.selectedRange = this.ranges[0];
    }
    this.loadMojElektroMerilnoMesto();
  }

  loadMojElektroMerilnoMesto() {
    const enotniIdentifikator = this.route.snapshot.paramMap.get('enotniIdentifikator');
    if (!enotniIdentifikator) return;

    this.mojElektroService.getMojElektroMerilnoMesto(enotniIdentifikator).subscribe({
      next: merilnoMesto => {
        this.mojElektroMerilnoMesto = merilnoMesto;
        this.mojElektroService.mojElektroSignal.set(merilnoMesto);

      }
    });
  }
}