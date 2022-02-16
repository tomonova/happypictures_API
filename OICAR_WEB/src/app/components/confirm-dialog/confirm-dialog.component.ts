import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AddNewImageDialogComponent } from '../add-new-image-dialog/add-new-image-dialog.component';

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.css']
})
export class ConfirmDialogComponent implements OnInit {
  title
  constructor(@Inject(MAT_DIALOG_DATA) data,private dialogRef: MatDialogRef<AddNewImageDialogComponent>) { 
    this.title = data.title
  }

  ngOnInit(): void {
  }
  confirm() {
    this.dialogRef.close(true);
  }
  close() {
    this.dialogRef.close(false);
  }
}
