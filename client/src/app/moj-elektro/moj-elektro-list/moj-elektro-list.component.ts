import { Component, inject, OnInit } from '@angular/core';
import { StavbeService } from '../../_services/stavbe.service';
import { MojElektroService } from '../../_services/moj-elektro.service';
import { MojElektroMerilnoMesto } from '../../_models/MojElektroMerilnoMesto';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-moj-elektro-list',
  imports: [RouterLink],
  templateUrl: './moj-elektro-list.component.html',
  styleUrl: './moj-elektro-list.component.css'
})
export class MojElektroListComponent implements OnInit {
  stavbeService = inject(StavbeService);
  mojElektroService = inject(MojElektroService);
  mojElektroMerilnaMesta: MojElektroMerilnoMesto[] = [];


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

}


