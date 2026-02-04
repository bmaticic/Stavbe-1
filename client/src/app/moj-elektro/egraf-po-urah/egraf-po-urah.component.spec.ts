import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EgrafPoUrahComponent } from './egraf-po-urah.component';

describe('EgrafPoUrahComponent', () => {
  let component: EgrafPoUrahComponent;
  let fixture: ComponentFixture<EgrafPoUrahComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EgrafPoUrahComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EgrafPoUrahComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
