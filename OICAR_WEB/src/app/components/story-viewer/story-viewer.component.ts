import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Card } from 'src/app/models/cardModel';
import { Story } from 'src/app/models/storyModel';

@Component({
  selector: 'app-story-viewer',
  templateUrl: './story-viewer.component.html',
  styleUrls: ['./story-viewer.component.css']
})
export class StoryViewerComponent implements OnInit {
  story:Story
  currentCard:Card
  currIndex = 0
  canNext = false
  canBefore = false
  constructor(@Inject(MAT_DIALOG_DATA) data,private dialogRef: MatDialogRef<StoryViewerComponent>) {
    this.story = data.story
    console.log("story kojeg gledamo", this.story)
    if(this.story.CARDS){
      console.log("currentCard", this.story.CARDS[0])
      this.currentCard = this.story.CARDS[0]
      if(this.story.CARDS.length>1){
        this.canNext=true
      }
    }
    
  }

  ngOnInit(): void {
  }
  next(){
    console.log("next click",this.currIndex,this.story.CARDS.length)
    if(this.currIndex<this.story.CARDS.length-1){
      this.currentCard = this.story.CARDS[this.currIndex+1]
      this.currIndex +=1
      this.canBefore=true
      if(this.currIndex==this.story.CARDS.length-1){
        this.canNext=false
      }
    }else{

    }
  }
  prev(){
    console.log("before click")
    if(this.currIndex>0){
      this.currentCard = this.story.CARDS[this.currIndex-1]
      this.currIndex -=1
      this.canNext= true
      if(this.currIndex==0){
        this.canBefore=false
      }
    }else{

    }
  }
  confirm() {
    this.dialogRef.close(true);
  }
  close() {
    this.dialogRef.close(false);
  }
}
