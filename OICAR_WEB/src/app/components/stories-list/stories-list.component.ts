import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-stories-list',
  templateUrl: './stories-list.component.html',
  styleUrls: ['./stories-list.component.css']
})
export class StoriesListComponent implements OnInit {
  @Input() rows = 1
  @Input() startIndex = 0
  @Input() fullSize = 0
  @Input() stories = []
  @Input() addable = false
  height = "10em"
  canBack=false
  canFront=true
  @Output() backClicked = new EventEmitter<any>();
  @Output() nextClicked = new EventEmitter<any>();
  @Output() storySelected = new EventEmitter<any>();
  @Output() newStoryClicked = new EventEmitter<any>();
  clickedStoryID= -1
  constructor() {
    this.height = (this.rows * 10)+"em"
   }

  ngOnInit(): void {
    if(this.startIndex==0){
      
      this.canBack=false
    }
  }
  ngOnChanges(){
    if(this.startIndex==0){
      this.canBack=false
    }else{
      this.canBack=true
    }
    //console.log(this.startIndex, "= pocindex, fullSize=",this.fullSize, "(this.startIndex+7 > this.fullSize-1) -->",(this.startIndex+7 > this.fullSize-1))
    if(this.addable){
      if((this.startIndex+7 > this.fullSize-1) && this.rows==1){
        this.canFront=false
      }else{
        this.canFront=true
      }
    }else{
      if((this.startIndex+8 > this.fullSize-1) && this.rows==1){
        this.canFront=false
      }else{
        this.canFront=true
      }
    }
   
  }
  storyClicked(story){
    this.clickedStoryID=story.IDSTORY
    this.storySelected.emit(story)
  }
  backClick(){
    this.backClicked.emit()
  }
  nextClick(){
    this.nextClicked.emit()
  }
  newStory(){
    this.clickedStoryID=-1
    this.newStoryClicked.emit()
  }
}
