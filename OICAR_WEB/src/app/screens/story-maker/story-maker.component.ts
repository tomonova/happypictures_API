import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { Subscription } from 'rxjs';
import { AddNewImageDialogComponent } from 'src/app/components/add-new-image-dialog/add-new-image-dialog.component';
import { ConfirmDialogComponent } from 'src/app/components/confirm-dialog/confirm-dialog.component';
import { StoryViewerComponent } from 'src/app/components/story-viewer/story-viewer.component';
import { Story } from 'src/app/models/storyModel';
import { DbConnectorService } from 'src/app/services/db-connector.service';

@Component({
  selector: 'app-story-maker',
  templateUrl: './story-maker.component.html',
  styleUrls: ['./story-maker.component.css']
})
export class StoryMakerComponent implements OnInit {
  myStories:Story[]=[]
  currentMyStories:Story[]=[]
  currMyIndex = 0
  currentStory:Story={IDSTORY:-1,CARDS:[],DESCRIPTION:"",FAVOURITE:0,IMAGE:null,NAME:"",NumberOfLikes:0,PROFILEID:parseInt(localStorage.getItem("user_id")),SHARED:0,TAGS:[]}
  sub3:Subscription
  creatingNew = true
  loading = false

  constructor(private dialog: MatDialog,private dbConnector:DbConnectorService) { }

  ngOnInit(): void {
    this.loading = true
    this.sub3 = this.dbConnector.getUserStories().subscribe(val=>{
      console.log("m_stories", val)
      this.myStories=val
      this.loading = false
      this.currentMyStories=this.myStories.slice(0,7)
    })
  }
  ngOnDestroy():void{
    this.sub3.unsubscribe()
  }
  next(str){
    //console.log("nextClick", str, this.myStories.slice(this.currMyIndex+7,))
    switch(str){
      case "my":
        if(this.currMyIndex+7<this.myStories.length){
          this.currMyIndex+=7
          this.currentMyStories = this.myStories.slice(this.currMyIndex,this.currMyIndex+7)
        }
        break;
    }
  }
  back(str){
    switch(str){
      case "my":
        console.log(this.currMyIndex, this.myStories.length)
        if(this.currMyIndex-7>=0){
          this.currMyIndex-=7
          this.currentMyStories = this.myStories.slice(this.currMyIndex,this.currMyIndex+7)
        }
        break;
    }
  }
  cardSelected(card){
    this.currentStory.CARDS.push(card)
    console.log("trenutne kartice", this.currentStory.CARDS)
  }
  storySelected(story){
    this.loading = true
    this.currentStory=story
    this.creatingNew=false
    console.log("odabran story", story)
    this.dbConnector.getStoryCards(story).subscribe(val=>{
      this.loading = false
      console.log("cardsi storija su", val)
      this.currentStory.CARDS=val
    })
  }
  sharedChange(event){
    console.log(event)
    if(event){
      this.currentStory.SHARED=1
    }else{
      this.currentStory.SHARED=0
    }
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
  tagsChanged(event){
    console.log(event.target.value)
    var currValues = event.target.value.split(",")
    this.currentStory.TAGS=[]
    currValues.forEach(element => {
      this.currentStory.TAGS.push({"VALUE":element})
    });
    //this.changesMade=true
    console.log("current tags are", this.currentStory.TAGS)
  }
  nameChanged(event){
    this.currentStory.NAME=event.target.value
  }
  descChanged(event){
    this.currentStory.DESCRIPTION=event.target.value
  }
  chooseImage(){
    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = false;
    dialogConfig.autoFocus = false;
    dialogConfig.data = {};

    const dr = this.dialog.open(AddNewImageDialogComponent, dialogConfig);
    dr.afterClosed().subscribe(
      data => {
        console.log("Dialog output:", data)
        this.currentStory.IMAGE=data
      }
    );
  }
  makeNewStory(){
    this.creatingNew = true
    this.currentStory={IDSTORY:-1,CARDS:[],DESCRIPTION:"",FAVOURITE:0,IMAGE:null,NAME:"",NumberOfLikes:0,PROFILEID:parseInt(localStorage.getItem("user_id")),SHARED:0,TAGS:[]}
  }
  removeCard(ind){
    this.currentStory.CARDS.splice(ind,1)
  }
  saveClick(){
    this.loading = true
    if(this.currentStory.IMAGE==null || this.currentStory.NAME==""){
      this.loading = false
      alert("Select thumbnail and name for your story")
    }else{
      if(this.creatingNew){
        console.log("u kreaciju nove kartice saljemo", this.currentStory)
        this.dbConnector.insertNewStory(this.currentStory).subscribe(val=>{
          console.log("Kreirali smo novu karticu manageCards vrati", val, "al mi postavljamo story id na", parseInt(localStorage.getItem("new_story_id")))
          this.loading = false
          this.currentStory.IDSTORY=parseInt(localStorage.getItem("new_story_id"))
          console.log("ukupno dobiveni story iz operacije insertNew je", this.currentStory)
          this.currentMyStories.unshift(this.currentStory)
          this.currentMyStories.pop()
        })
      }else{
        this.dbConnector.updateStory(this.currentStory).subscribe(val=>{
          console.log("story updejtan", val)
          this.loading = false
          this.currentMyStories = this.currentMyStories.filter(cur => cur.IDSTORY!=this.currentStory.IDSTORY)
          this.currentMyStories.unshift(this.currentStory)
        })
      }
    }
   
  }
  previewClick(){
      const dialogConfig = new MatDialogConfig();
  
      dialogConfig.disableClose = false;
      dialogConfig.autoFocus = false;
      dialogConfig.data = {story:this.currentStory};
  
      const dr = this.dialog.open(StoryViewerComponent, dialogConfig);
      dr.afterClosed().subscribe(
        data => {
          console.log("Dialog output:", data)
          if(data){
  
          }
        }
      );
  }
  deleteClick(){
    this.loading = true
    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = false;
    dialogConfig.autoFocus = false;
    dialogConfig.data = {title:"Are you sure you want to delete this story?"};

    const dr = this.dialog.open(ConfirmDialogComponent, dialogConfig);
    dr.afterClosed().subscribe(
      data => {
        console.log("Dialog output:", data)
        if(data){
          this.dbConnector.deleteStory(this.currentStory).subscribe(val=>{
            console.log("story deletan")
            this.myStories = this.myStories.filter(st => st.IDSTORY!=this.currentStory.IDSTORY)
            this.currMyIndex = 0
            this.currentMyStories=this.myStories.slice(0,8)
            this.loading=false
            this.makeNewStory()
          })

        }
      }
    );
  }
}
