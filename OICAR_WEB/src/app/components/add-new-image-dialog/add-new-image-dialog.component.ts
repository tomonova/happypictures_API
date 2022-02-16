import { Component, Inject, OnInit } from '@angular/core';
import { MatDialog, MatDialogConfig, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Image } from 'src/app/models/imageModel';
import { DbConnectorService } from 'src/app/services/db-connector.service';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-add-new-image-dialog',
  templateUrl: './add-new-image-dialog.component.html',
  styleUrls: ['./add-new-image-dialog.component.css']
})
export class AddNewImageDialogComponent implements OnInit {
  selectedImg = ''
  allImages:Image[]=[]
  loading = false
  selectedImgID=-1
  constructor( @Inject(MAT_DIALOG_DATA) data,private dialogRef: MatDialogRef<AddNewImageDialogComponent>, private dbConnector:DbConnectorService,private dialog: MatDialog,) { }

  ngOnInit(): void {
    this.getImages()
  }
  close() {
    this.dialogRef.close(this.selectedImg);
  }
  getImages(){
    this.loading=true
    this.dbConnector.getAllUserImages().subscribe(val=>{
      console.log("user images",val)
      this.allImages=val
      this.loading=false
    })
  }
  clickedImage(image){
    console.log("clicked image")
    this.selectedImgID=image.IDIMAGE
    this.dialogRef.close(image);
    
  }
  importNew(){

  }
  deleteImage(){
    this.loading=true
    this.loading = true
    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = false;
    dialogConfig.autoFocus = false;
    dialogConfig.data = {title:"Are you sure you want to delete this image?"};

    const dr = this.dialog.open(ConfirmDialogComponent, dialogConfig);
    dr.afterClosed().subscribe(
      data => {
        console.log("Dialog output:", data)
        if(data){
          this.dbConnector.deleteImage(this.selectedImgID).subscribe(val=>{
            console.log("deletali smo sliku")
            this.allImages = this.allImages.filter(img=>img.IDIMAGE!=this.selectedImgID)
            this.loading=false
            this.selectedImgID=-1
          })

        }
      }
    );
    
  }
  fileSelected(event){
    this.loading=true
    console.log(event.target.files)
    this.dbConnector.uploadNewImage(event.target.files[0]).subscribe(val=>{
      console.log("uspilo", val)
      this.loading  = false
      var imag:Image = JSON.parse(localStorage.getItem("added_img"))
      imag.IDIMAGE=val.idImage
      this.allImages.unshift(imag)
    },error=>{
      console.log("error", error)
      this.loading=false
    })
  }
}
