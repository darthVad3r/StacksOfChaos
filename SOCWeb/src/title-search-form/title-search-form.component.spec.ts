import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TitleSearchFormComponent } from './title-search-form.component';

describe('TitleSearchFormComponent', () => {
  let component: TitleSearchFormComponent;
  let fixture: ComponentFixture<TitleSearchFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TitleSearchFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TitleSearchFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
