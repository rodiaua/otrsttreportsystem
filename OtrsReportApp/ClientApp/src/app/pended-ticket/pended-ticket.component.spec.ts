import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PendedTicketComponent } from './pended-ticket.component';

describe('PendedTicketComponent', () => {
  let component: PendedTicketComponent;
  let fixture: ComponentFixture<PendedTicketComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PendedTicketComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PendedTicketComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
