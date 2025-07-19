import { Component, inject, OnInit } from '@angular/core';
import { StavbeService } from '../../_services/stavbe.service';
import { Stavba } from '../../_models/stavba';
import { StavbaCardComponent } from '../stavba-card/stavba-card.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { StavbaParams } from '../../_models/stavbaParams';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-stavbe-list',
  imports: [StavbaCardComponent, PaginationModule, FormsModule],
  templateUrl: './stavbe-list.component.html',
  styleUrl: './stavbe-list.component.css'
})
export class StavbeListComponent implements OnInit {
  stavbeService = inject(StavbeService);
  stavbaParams = new StavbaParams(this.stavbeService.stavbaSignal());
  vrstaStavbeParams = [{ value: 'vrtec', display: 'Vrtec' },
  { value: 'izobraževalni', display: 'Izobraževalni' },
  { value: 'športni', display: 'Športni' },
  { value: 'kulturni', display: 'Kulturni' },
  { value: 'ostali', display: 'Ostali' },
  { value: 'vsi', display: 'Vse stavbe' },
  ];

  ogrevanjeOznakaParams = [{ value: 'DO', display: 'Daljinsko' },
  { value: 'ELKO', display: 'Kurilno olje' },
  { value: 'LB', display: 'Lesna biomasa' },
  { value: 'TC', display: 'Toplotna črpalka' },
  { value: 'UNP', display: 'Utekočinjen naftni plin' },
  { value: 'ZP', display: 'Zemeljski plin' },
  { value: 'ELE', display: 'Elektrika' },
  { value: 'vsi', display: 'Vse vrste ogrevanja' },]


  ngOnInit(): void {
    if (!this.stavbeService.paginatedResult()) this.loadStavbe();
  }

  loadStavbe() {
    this.stavbeService.getStavbe(this.stavbaParams)
  }

  resetFilters() {
    this.stavbaParams = new StavbaParams(this.stavbeService.stavbaSignal());
    this.loadStavbe();
  }


  pageChanged(event: any) {
    if (this.stavbaParams.pageNumber !== event.page) {
      this.stavbaParams.pageNumber = event.page;
      this.loadStavbe();
    }
  }



}
