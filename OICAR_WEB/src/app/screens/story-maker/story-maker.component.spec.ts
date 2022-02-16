import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StoryMakerComponent } from './story-maker.component';

describe('StoryMakerComponent', () => {
  let component: StoryMakerComponent;
  let fixture: ComponentFixture<StoryMakerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StoryMakerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StoryMakerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
