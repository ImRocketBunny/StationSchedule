import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StationLinkMenuComponent } from './station-link-menu.component';

describe('StationLinkMenuComponent', () => {
  let component: StationLinkMenuComponent;
  let fixture: ComponentFixture<StationLinkMenuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StationLinkMenuComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StationLinkMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
