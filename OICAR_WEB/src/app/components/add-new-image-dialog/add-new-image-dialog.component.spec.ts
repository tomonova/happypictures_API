import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddNewImageDialogComponent } from './add-new-image-dialog.component';

describe('AddNewImageDialogComponent', () => {
  let component: AddNewImageDialogComponent;
  let fixture: ComponentFixture<AddNewImageDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddNewImageDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddNewImageDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
