import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TitleSearchFormComponentComponent } from './title-search-form-component.component';

describe('TitleSearchFormComponentComponent', () => {
  let component: TitleSearchFormComponentComponent;
  let fixture: ComponentFixture<TitleSearchFormComponentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TitleSearchFormComponentComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TitleSearchFormComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
