import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule } from '@angular/forms';
import { MojElektroApiService } from '../../_services/moj-elektro-api.service';
import { IntervalBlok } from '../../_models/interval-blok';
import { ReactiveFormsModule } from '@angular/forms';
import { JsonPipe } from '@angular/common';
import { MojElektroService } from '../../_services/moj-elektro.service';

@Component({
  selector: 'app-moj-elektro-read15',
  templateUrl: './moj-elektro-read15.component.html',
  styleUrl: './moj-elektro-read15.component.css',
  imports: [ReactiveFormsModule, FormsModule, JsonPipe]

})
export class MojElektroRead15Component implements OnInit {
  form: FormGroup;
  result: IntervalBlok | null = null;
  loading = false;
  error: string | null = null;
  mojElektroService = inject(MojElektroService);


  constructor(private api: MojElektroApiService, private fb: FormBuilder) {
    this.form = this.fb.group({
      usagePoint: [''],
      startTime: [''],
      endTime: [''],
      option: [''],
      apiToken: ['']
    });
  }
  ngOnInit(): void {
    this.form.patchValue({
      usagePoint: this.mojElektroService.mojElektroSignal()?.enotniIdentifikator,
      apiToken: '735c3aff196b4b409f1b56e10da3edd6',
      startTime: '2025-05-01',
      endTime: '2025-05-07',
      option: 'ReadingType=8.0.2.4.1.2.12.0.0.0.0.0.0.0.0.3.72.0'
    });
  }

  submit() {
    this.loading = true;
    this.error = null;
    const { usagePoint, startTime, endTime, option, apiToken } = this.form.value;
    this.api.getMeterReadings(usagePoint, startTime, endTime, option, apiToken).subscribe({
      next: (data: IntervalBlok) => {
        this.result = data;
        this.loading = false;
      },
      error: err => {
        this.error = err.message || 'Napaka pri pridobivanju podatkov.';
        this.loading = false;
      }
    });
  }
}