import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryModule, GalleryItem, ImageItem } from 'ng-gallery';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';

import { MojElektroService } from '../../_services/moj-elektro.service';
import { MojElektroMerilnoMesto } from '../../_models/MojElektroMerilnoMesto';
import { FormsModule } from '@angular/forms';
import { EgrafDnevniComponent } from "../egraf-dnevni/egraf-dnevni.component";
import { IRange } from '../../_models/i-range';
import { MojElektroRead15Component } from '../moj-elektro-read15/moj-elektro-read15.component';
import { EgrafPoUrahComponent } from "../egraf-po-urah/egraf-po-urah.component";
import { EgrafPoTednihComponent } from "../egraf-po-tednih/egraf-po-tednih.component";
import { EgrafPoBlokihComponent } from "../egraf-po-blokih/egraf-po-blokih.component";
import { EgrafVsiPodatkiComponent } from "../egraf-vsi-podatki/egraf-vsi-podatki.component";
import { EgrafVsiPodatkiGrupiraniComponent } from '../egraf-vsi-podatki-grupirani/egraf-vsi-podatki-grupirani.component';

@Component({
  selector: 'app-moj-elektro-card',
  imports: [TabsModule, GalleryModule, CommonModule, BsDatepickerModule, FormsModule, EgrafDnevniComponent, MojElektroRead15Component, EgrafPoUrahComponent, EgrafPoTednihComponent, 
    EgrafPoBlokihComponent, EgrafVsiPodatkiComponent, EgrafVsiPodatkiGrupiraniComponent],  // RouterLink
  templateUrl: './moj-elektro-card.component.html',
  styleUrl: './moj-elektro-card.component.css'
})
export class MojElektroCardComponent {
  mojElektroService = inject(MojElektroService);
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
        this.mojElektroService.selectedRangeSignal.set(this.selectedRange);
      }
    });
  }

  onTabSelect(index: number): void {
    // Update active tab signal when tab selection changes
    this.mojElektroService.activeTabSignal.set(index);
  }

  // onValueChange($event: (Date | undefined)[] | undefined) {
  //   if ($event && $event.length === 2) {
  //     this.selectedRange.value = $event as Date[];
  //     this.selectedRange.label = `${$event[0]?.toLocaleDateString()} - ${$event[1]?.toLocaleDateString()}`;
  //     this.mojElektroService.selectedRangeSignal.set(this.selectedRange);

  //   }
  // }

}