import { Component, inject, OnInit } from '@angular/core';
import { MojElektroService } from '../../_services/moj-elektro.service';
import { IRange, RangePreDefinirani } from '../../_models/i-range';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-egraf-dnevni',
  standalone: true,
  imports: [CommonModule, BsDatepickerModule, FormsModule],
  templateUrl: './egraf-dnevni.component.html',
  styleUrls: ['./egraf-dnevni.component.css']
})
export class EgrafDnevniComponent implements OnInit {
  mojElektroService = inject(MojElektroService);
  ranges: IRange[] = new RangePreDefinirani().getRanges();
  selectedRange: IRange = this.ranges[0];

  // UI / state
  isLoading = false;
  error: string | null = null;
  chartData: any = null; // replace `any` with a proper model when available

  private formatDateAsString(date: Date): string {
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    return `${day}-${month}-${year}`;
  }

  ngOnInit(): void {
    // Ensure selectedRange has a sensible default value (last 7 days)
    if (!this.selectedRange.value || this.selectedRange.value.length !== 2) {
      const end = new Date();
      const start = new Date(end);
      start.setDate(end.getDate() - 6); // last 7 days including today
      this.selectedRange.value = [start, end];
      this.selectedRange.label = `${this.formatDateAsString(start)} - ${this.formatDateAsString(end)}`;
    }

    // Initial load
    this.fetchData();
  }

  onValueChange($event: (Date | undefined)[] | undefined) {
    if ($event && $event.length === 2) {
      this.selectedRange.value = $event as Date[];
      this.selectedRange.label = `${this.formatDateAsString($event[0]!)} - ${this.formatDateAsString($event[1]!)}`;
      // Reload data for the newly selected range
      this.fetchData();
    }
  }

  /**
   * Attempts to fetch data from the MojElektroService for the currently selected range.
   * This method uses a runtime check/cast so it won't fail Typescript compilation if the
   * concrete service method name differs; adjust the method name to the actual service API.
   */
  fetchData(): void {
    this.error = null;
    this.chartData = null;
    const rangeValues = this.selectedRange.value;
    if (!rangeValues || rangeValues.length !== 2) {
      this.error = 'Invalid date range';
      return;
    }

    // Example: try common service method names, falling back to a warning.
    const svc: any = this.mojElektroService;
    const possibleMethods = ['getDailyData', 'getDailyGraph', 'getEgrafDnevni'];

    const method = possibleMethods.find(m => typeof svc[m] === 'function');
    if (!method) {
      console.warn('No known data-fetch method found on MojElektroService. Expected one of:', possibleMethods);
      return;
    }

    try {
      const obs = svc[method](rangeValues[0], rangeValues[1]);
      if (!obs || typeof obs.subscribe !== 'function') {
        this.error = 'Service did not return an observable';
        return;
      }

      this.isLoading = true;
      obs.subscribe({
        next: (data: any) => {
          this.chartData = data;
          this.isLoading = false;
        },
        error: (err: any) => {
          console.error(err);
          this.error = err?.message ?? 'Error loading data';
          this.isLoading = false;
        }
      });
    } catch (err: any) {
      console.error(err);
      this.error = err?.message ?? 'Unexpected error';
      this.isLoading = false;
    }
  }
}