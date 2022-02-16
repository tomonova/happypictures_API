import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { Subscription } from 'rxjs';
import { StoryViewerComponent } from 'src/app/components/story-viewer/story-viewer.component';
import { Story } from 'src/app/models/storyModel';
import { DbConnectorService } from 'src/app/services/db-connector.service';

@Component({
  selector: 'app-home-screen',
  templateUrl: './home-screen.component.html',
  styleUrls: ['./home-screen.component.css']
})
export class HomeScreenComponent implements OnInit {
  discoveredStories:Story[]=[]
  currentDiscoveredStories:Story[]=[]
  currDiscIndex = 0
  sub1:Subscription

  favouriteStories:Story[]=[]
  currentFavouriteStories:Story[]=[]
  currFavIndex = 0
  sub2:Subscription

  myStories:Story[]=[]
  currentMyStories:Story[]=[]
  currMyIndex = 0
  sub3:Subscription
  constructor(private dbConnector:DbConnectorService,private dialog: MatDialog) { }

  ngOnInit(): void {
    this.sub1 = this.dbConnector.getDiscoverStories().subscribe(val=>{
      console.log("d_stories", val)
      this.discoveredStories=val
      this.currentDiscoveredStories=this.discoveredStories.slice(0,8)
    })
    this.sub2 = this.dbConnector.getFavouriteStories().subscribe(val=>{
      console.log("f_stories", val)
      this.favouriteStories=val
      this.currentFavouriteStories=this.favouriteStories.slice(0,8)
    })
    this.sub3 = this.dbConnector.getUserStories().subscribe(val=>{
      console.log("m_stories", val)
      this.myStories=val
      this.currentMyStories=this.myStories.slice(0,8)
    })
  }
  next(str){
    switch(str){
      case "disc":
        if(this.currDiscIndex+8<this.discoveredStories.length){
          this.currDiscIndex+=8
          this.currentDiscoveredStories = this.discoveredStories.slice(this.currDiscIndex,this.currDiscIndex+8)
        }
        break;
      case "fav":
        if(this.currFavIndex+8<this.favouriteStories.length){
          this.currFavIndex+=8
          this.currentFavouriteStories = this.favouriteStories.slice(this.currFavIndex,this.currFavIndex+8)
        }
        break;
      case "my":
        if(this.currMyIndex+8<this.myStories.length){
          this.currMyIndex+=8
          this.currentMyStories = this.myStories.slice(this.currMyIndex,this.currMyIndex+8)
        }
        break;
    }
  }
  back(str){
    switch(str){
      case "disc":
        if(this.currDiscIndex-8>=0){
          this.currDiscIndex-=8
          this.currentDiscoveredStories = this.discoveredStories.slice(this.currDiscIndex,this.currDiscIndex+8)
        }
        break;
      case "fav":
        if(this.currFavIndex-8>=0){
          this.currFavIndex-=8
          this.currentFavouriteStories = this.favouriteStories.slice(this.currFavIndex,this.currFavIndex+8)
        }
        break;
      case "my":
        if(this.currMyIndex-8>=0){
          this.currMyIndex-=8
          this.currentMyStories = this.myStories.slice(this.currMyIndex,this.currMyIndex+8)
        }
        break;
    }
  }
  ngOnDestroy():void{
    this.sub1.unsubscribe()
  }
  openStory(str){
   
    const dialogConfig = new MatDialogConfig();
  
    dialogConfig.disableClose = false;
    dialogConfig.autoFocus = false;
    dialogConfig.data = {story:str};
    this.dbConnector.getStoryCards(str).subscribe(val=>{
      console.log("kartice storija su", val)
      str.CARDS=val
      const dr = this.dialog.open(StoryViewerComponent, dialogConfig);
      dr.afterClosed().subscribe(
      data => {
        console.log("Dialog output:", data)
        if(data){

        }
      }
    );
    })
    
  }
}
