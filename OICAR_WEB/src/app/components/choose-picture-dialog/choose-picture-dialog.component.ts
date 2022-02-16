import { Component, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
@Component({
  selector: 'app-choose-picture-dialog',
  templateUrl: './choose-picture-dialog.component.html',
  styleUrls: ['./choose-picture-dialog.component.css']
})
export class ChoosePictureDialogComponent implements OnInit {

  sortedImagesByMonth=[["../../../assets/images/examples/nail_cutter.jpg"]]
  constructor(  public dialogRef: MatDialogRef<ChoosePictureDialogComponent>,) { }

  ngOnInit(): void {
  }
  closeDialog() { 
    this.dialogRef.close({ event: 'close', data: {} }); 
  }
}
