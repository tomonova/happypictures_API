import { CdkTextareaAutosize } from '@angular/cdk/text-field';
import { Component, OnInit,Input, ViewChild, EventEmitter, Output} from '@angular/core';
import { Card } from 'src/app/models/cardModel';
import {MatDialog, MatDialogConfig} from "@angular/material/dialog";
import { AddNewImageDialogComponent } from '../add-new-image-dialog/add-new-image-dialog.component';
import { DbConnectorService } from 'src/app/services/db-connector.service';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-card-viewer',
  templateUrl: './card-viewer.component.html',
  styleUrls: ['./card-viewer.component.css']
})
export class CardViewerComponent implements OnInit {
  @Input() card:Card
  @Input() currCreating:false
  @ViewChild('autosize') autosize: CdkTextareaAutosize;
  changesMade =false
  user
  @Output() cardEdited = new EventEmitter<any>();
  @Output() cardDeleted = new EventEmitter<any>();
  loading=false
  constructor(private dialog: MatDialog, private dbConnector:DbConnectorService) { }

  ngOnInit(): void {
    this.user=JSON.parse(localStorage.getItem("user"))
  }
  ngOnChanges() {
    this.changesMade=false
  }
  textChanged(event){
    console.log(event)
    this.card.TEXT=event
    this.changesMade=true
  }
  nameChanged(event){
    console.log(event)
    this.card.NAME=event
    this.changesMade=true
  }
  tagsChanged(event){
    console.log(event.target.value)
    var currValues = event.target.value.split(",")
    this.card.TAGS=[]
    currValues.forEach(element => {
      this.card.TAGS.push({"VALUE":element})
    });
    this.changesMade=true
    console.log("current tags are", this.card.TAGS)
  }
  toggleBold(){
    if(this.card.FORMAT.FONT_FORMAT==2){
      //boldano je stavi da nije
      this.card.FORMAT.FONT_FORMAT=1
    }else{
      this.card.FORMAT.FONT_FORMAT=2
    }
    this.changesMade=true
  }
  imageAdd(num){
    switch(num){
      case 1:
        break;
      case 2:
        break;
      case 3:
        break;
    }
    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = false;
    dialogConfig.autoFocus = false;
    dialogConfig.data = {};

    const dr = this.dialog.open(AddNewImageDialogComponent, dialogConfig);
    dr.afterClosed().subscribe(
      data => {
        console.log("Dialog output:", data)
        switch(num){
          case 1:
            this.card.FORMAT.IMAGE1 = data
            break;
          case 2:
            this.card.FORMAT.IMAGE2 = data
            break;
          case 3:
            this.card.FORMAT.IMAGE3 = data
            break;
        }
      }
    );
    this.changesMade=true
  }
  removeImage(num){
    switch(num){
      case 1:
        this.card.FORMAT.IMAGE1=null
        break;
      case 2:
        this.card.FORMAT.IMAGE2=null
        break;
      case 3:
        this.card.FORMAT.IMAGE3=null
        break;
    }
    this.changesMade=true
  }
  saveCard(){
    this.loading=true
    if(this.currCreating){
      //kreiramo novu karticu
      this.dbConnector.insertNewCard(this.card).subscribe(val=>{
        console.log("KARTICA KREIRANA",val)
        this.card.IDCARD=val.idCard
        this.cardEdited.emit(this.card)
        this.changesMade=false
        this.loading=false
      })
    }else{
      //Mijenjamo postojeću
      this.dbConnector.updateCard(this.card).subscribe(val=>{
        console.log("KARTICA izmjenjena",val)
        this.cardEdited.emit(this.card)
        this.changesMade=false
        this.loading=false
      })
    }
  }
  previewCard(){
    
  }
  deleteCard(){
    this.loading=true
    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = false;
    dialogConfig.autoFocus = false;
    dialogConfig.data = {title:"Are you sure you want to delete this card?"};

    const dr = this.dialog.open(ConfirmDialogComponent, dialogConfig);
    dr.afterClosed().subscribe(
      data => {
        console.log("Dialog output:", data)
        if(data){
          this.dbConnector.deleteCard(this.card).subscribe(val=>{
            this.cardDeleted.emit(this.card)
            this.loading=false
        })
        }
      }
    );
    
  }
  getTags(tags){
    if(typeof tags != 'undefined'){
      var str = ""
      var first = true
      tags.forEach(element => {
        if(!first){
          str += ","
        }
        first = false
        str = str + element.VALUE
      });
      return str
    }else{
      return ""
    }
  }
}
