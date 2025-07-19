import { Component, Inject, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StavbeService } from '../../_services/stavbe.service';
import { ActivatedRoute, RouterLink, RouterLinkActive } from '@angular/router';
import { Stavba } from '../../_models/stavba';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryModule, GalleryItem, ImageItem } from 'ng-gallery';
import { MerilnaMestaListComponent } from "../../merilna-mesta/merilna-mesta-list/merilna-mesta-list.component";

import { LOCALE_ID } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { PoligonComponent } from "../../geo-tocke/poligon/poligon.component";
import { MojElektroMMestaComponent } from "../../merilna-mesta/moj-elektro-m-mesta/moj-elektro-m-mesta.component";

@Component({
    selector: 'app-stavba-detail',
    imports: [TabsModule, GalleryModule, CommonModule, RouterLink, MerilnaMestaListComponent, PoligonComponent, MojElektroMMestaComponent],
    templateUrl: './stavba-detail.component.html',
    styleUrl: './stavba-detail.component.css'
})
export class StavbaDetailComponent {
    constructor(@Inject(LOCALE_ID) public locale: string) { }
    

    private stavbeService = inject(StavbeService);
    private route = inject(ActivatedRoute);
    stavba?: Stavba;

    images: GalleryItem[] = [];

  
    ngOnInit(): void {
      
      this.loadStavba();
      if(!this.stavba) return;
 //     this.stavbeService.stavbaNaziv.set(this.stavba.naziv);
    }
  
    loadStavba() {
      const naziv = this.route.snapshot.paramMap.get('naziv');
      console.log("test stavba detail loadstavba" + naziv);

      if (!naziv) return;

      this.stavbeService.getStavbaPoNazivu(naziv).subscribe({
        next: stavba => {
          this.stavba = stavba;
          this.stavbeService.stavbaNaziv.set(stavba.naziv);
          this.stavbeService.stavbaSignal.set(stavba);

          stavba.photosStavbe.map(p => {
            this.images.push(new ImageItem({ src: p.url, thumb: p.url }));
          });

        }
      });
    }
  
}

   //       this.stavba.fotoUrl = 'https://randomuser.me/api/portraits/men/93.jpg';

