import { Component, inject, OnInit } from '@angular/core';
import { MojElektroService } from '../../_services/moj-elektro.service';
import { IRange, RangePreDefinirani } from '../../_models/i-range';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-egraf-dnevni',
  imports: [BsDatepickerModule, FormsModule],
  templateUrl: './egraf-dnevni.component.html',
  styleUrl: './egraf-dnevni.component.css'
})
export class EgrafDnevniComponent  {
  mojElektroService = inject(MojElektroService);
  ranges: IRange[] = new RangePreDefinirani().getRanges();
  selectedRange: IRange = this.ranges[0];


  onValueChange($event: (Date | undefined)[] | undefined) {
    if ($event && $event.length === 2) {
      this.selectedRange.value = $event as Date[];
      this.selectedRange.label = `${$event[0]?.toLocaleDateString()} - ${$event[1]?.toLocaleDateString()}`;
    }
  }
}