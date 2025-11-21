import { Component, HostListener, inject, OnInit, ViewChild } from '@angular/core';
import { StavbeService } from '../../_services/stavbe.service';
import { Stavba } from '../../_models/stavba';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { PhotoEditorStavbeComponent } from "../photo-editor-stavbe/photo-editor-stavbe.component";

@Component({
  selector: 'app-stavba-edit',
  imports: [TabsModule, FormsModule, PhotoEditorStavbeComponent],
  templateUrl: './stavba-edit.component.html',
  styleUrl: './stavba-edit.component.css'
})
export class StavbaEditComponent implements OnInit {
  @ViewChild('editForm') editForm?: NgForm;
  @HostListener('window:beforeunload', ['$event']) notify($event:any) {
    if (this.editForm?.dirty) {
      $event.returnValue = true;
    }
  }

  stavba?: Stavba
  private stavbeService = inject(StavbeService);
  private toastr = inject(ToastrService);

  ngOnInit(): void {
    this.loadStavba();
  }

  loadStavba() {
    var stavbaNaziv = this.stavbeService.stavbaNaziv();

    this.stavbeService.getStavbaPoNazivu(stavbaNaziv).subscribe(stavba => {
      this.stavba = stavba;
    });
  }

  updateStavba() {
    if (!this.stavba) return;
    this.stavbeService.updateStavba(this.stavba).subscribe({
      next: _ => {
        this.toastr.success('Stavba updated successfully');
        this.editForm?.reset(this.stavba);
      }
    })
  }

  onStavbaChange(event: Stavba) {
    this.stavba = event;
  }

}
