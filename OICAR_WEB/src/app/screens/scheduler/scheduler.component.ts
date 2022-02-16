import { Component, OnInit,TemplateRef} from '@angular/core';
import { Card } from 'src/app/models/cardModel';
import { Story } from 'src/app/models/storyModel';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { ChoosePictureDialogComponent } from 'src/app/components/choose-picture-dialog/choose-picture-dialog.component';
import { DbConnectorService } from 'src/app/services/db-connector.service';
import { AddNewImageDialogComponent } from 'src/app/components/add-new-image-dialog/add-new-image-dialog.component';
import { Schedule } from 'src/app/models/scheduleModel';
import { formatDate } from '@angular/common';
import { User } from 'src/app/models/userModel';

@Component({
  selector: 'app-scheduler',
  templateUrl: './scheduler.component.html',
  styleUrls: ['./scheduler.component.css']
})
export class SchedulerComponent implements OnInit {
  days=["Monday","Tuesday","Wednesday","Thursday","Friday","Saturday","Sunday"]
  scheduledImages:Schedule[]=[{},{},{},{},{},{},{}]
  isEditing=false
  currentSch:Schedule={}
  loading = false
  dialogRef!: TemplateRef<any>;
  changedDays:boolean[]=[false,false,false,false,false,false,false]

  constructor(public dialog: MatDialog,private dbConnector:DbConnectorService) { }

  ngOnInit(): void {
    //odradi monday
    this.setSchedule()
    //ode storije razdili po danima
    //var cardExample:Card = {card_id:"1",name:"card prvi",text:"Moje ime je _____", images:["../../../assets/images/examples/Girl-with-teddy.jpg"]}
    //var storyExample:Story = {id:"1",name:"Story prvi",thumbnail:"../../../assets/images/examples/biting_nails.jpg",cards:[cardExample]}
   

    //this.scheduledImages[0]=["../../../assets/images/examples/biting_nails.jpg"]
    //this.scheduledImages[2]=["../../../assets/images/examples/brrr.png"]
  }
  async setSchedule(){
    this.loading=true
    var datumPon = this.setToMonday(new Date())
    await this.dbConnector.getWholeWeekSchedule(datumPon)
    this.loading=false
    console.log(this.dbConnector.schedules)
    this.scheduledImages=this.dbConnector.schedules
  }
  openChoosePicture() {
    const myCompDialog = this.dialog.open(ChoosePictureDialogComponent, { data: {title:"Choose new picture"} });
    myCompDialog.afterClosed().subscribe((res) => {
      // Data back from dialog
      console.log({ res });
    });
  }
  add(day){
    var pon:Date = this.setToMonday(new Date())
    
    var dialogConfig = new MatDialogConfig();
  
    dialogConfig.disableClose = false;
    dialogConfig.autoFocus = false;
    dialogConfig.data = {};
  
    const dr = this.dialog.open(AddNewImageDialogComponent, dialogConfig);
    dr.afterClosed().subscribe(
      data => {
        console.log("Dialog output:", data)
        var user:User = JSON.parse(localStorage.getItem("user"))
        this.currentSch.PROFILEID=user.id
        this.currentSch.SCHEDULE_DATE = formatDate(pon.setDate(pon.getDate()+day),"yyyy-MM-dd", "en")
        this.currentSch.SCHEDULE_IMAGE = [{IMAGEID:data.IDIMAGE,POSITION:0}]
        this.dbConnector.insertSchedule(this.currentSch).subscribe(val =>{
          console.log("ubacili novi schedule", val)
          this.currentSch.IDSCHEDULE=val
        })
      }
    );
  }
  addScheduler(day){
    this.changedDays[day]=true
    var pon:Date = this.setToMonday(new Date())
    
    var dialogConfig = new MatDialogConfig();
  
    dialogConfig.disableClose = false;
    dialogConfig.autoFocus = false;
    dialogConfig.data = {};
    var newSch:Schedule={}
    if(this.scheduledImages[day].SCHEDULE_IMAGE !=undefined){
      const dr = this.dialog.open(AddNewImageDialogComponent, dialogConfig);
      dr.afterClosed().subscribe(
      data => {
        console.log("Dialog output:", data)
        this.scheduledImages[day].SCHEDULE_IMAGE.push({IMAGEID:data.IDIMAGE,POSITION:this.scheduledImages[day].SCHEDULE_IMAGE.length,IMAGE:{URL:data.URL}})
        console.log("za dan ",day, " slike su ", this.scheduledImages[day].SCHEDULE_IMAGE)
      }
    );
    }else{
      const dr = this.dialog.open(AddNewImageDialogComponent, dialogConfig);
      dr.afterClosed().subscribe(
      data => {
        console.log("Dialog output:", data)
        var user:User = JSON.parse(localStorage.getItem("user"))
        newSch.PROFILEID=user.id
        newSch.SCHEDULE_DATE = formatDate(pon.setDate(pon.getDate()+day),"yyyy-MM-dd", "en")
        newSch.SCHEDULE_IMAGE = [{IMAGEID:data.IDIMAGE,POSITION:0,IMAGE:{URL:data.URL}}]
        this.scheduledImages[day]=newSch
      }
    );
    }
  }
  removePhotoFromScheduler(day,imageID){
    this.changedDays[day]=true
    this.scheduledImages[day].SCHEDULE_IMAGE = this.scheduledImages[day].SCHEDULE_IMAGE.filter(imag => imag.IMAGEID != imageID)
    var index = 0
    this.scheduledImages[day].SCHEDULE_IMAGE.forEach(img=>{
      img.POSITION=index
      index+=1
    })
    console.log(this.scheduledImages[day].SCHEDULE_IMAGE)
  }
  async saveNewScheduler(){
    var dayInd=0
    this.changedDays.forEach(chDay=>{
      if(chDay){
         this.dbConnector.updateWholeSchedule(this.scheduledImages[dayInd]).then(val=>{
           console.log("dodali smo schedule", val)
         })
      }
      dayInd+=1
    })
  }
  async saveChanges(){
    this.isEditing=false
    this.loading=true
    var dayInd=0
    this.changedDays.forEach(chDay=>{
      if(chDay){
        console.log("dan koji se minja", this.scheduledImages[dayInd])
        var dayTr = dayInd+0
         this.dbConnector.updateWholeSchedule(JSON.parse(JSON.stringify(this.scheduledImages[dayInd]))).then(val=>{
           console.log("dodali smo schedule", val, "ajmo ga vratit u dan za dan",dayInd, this.scheduledImages[dayTr])
           if(val.idSchedule){
            this.scheduledImages[dayTr].IDSCHEDULE=val.idSchedule
           }else{
            this.scheduledImages[dayTr].IDSCHEDULE=val
           }
           this.loading=false
         })
      }
      dayInd+=1
    })
  }
  setToMonday( date ) {
    var day = date.getDay() || 7;  
    if( day !== 1 ) 
        date.setHours(-24 * (day - 1)); 
    return date;
}
}
