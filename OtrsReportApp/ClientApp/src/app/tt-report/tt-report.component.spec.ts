import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { TtReportComponent } from './tt-report.component';

describe('TtReportComponent', () => {
  let component: TtReportComponent;
  let fixture: ComponentFixture<TtReportComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ TtReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TtReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
