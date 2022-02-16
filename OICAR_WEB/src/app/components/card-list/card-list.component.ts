import { Component, Input, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { Card } from 'src/app/models/cardModel';
import { DbConnectorService } from 'src/app/services/db-connector.service';
import { Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-card-list',
  templateUrl: './card-list.component.html',
  styleUrls: ['./card-list.component.css']
})
export class CardListComponent implements OnInit {
  allCards:Card[]
  doneCards = 0
  currentImageUrl
  @Output() cardClicked = new EventEmitter<Card>();
  @Input() changedCard:Card
  @Input() deletedCard:Card
  @Input() addable = true
  constructor(private dbDonnector:DbConnectorService, private sanitizer: DomSanitizer) { }

  ngOnInit(): void {
      this.getCards()
  }
  ngOnChanges():void{
    console.log("promjena se dogodila")
    if(this.allCards){
      if(this.changedCard!=null){
        if(this.allCards.find(val=>val.IDCARD==this.changedCard.IDCARD)){
          var curCard = this.allCards.findIndex(val=>val.IDCARD==this.changedCard.IDCARD)
          this.allCards[curCard]=this.changedCard
        }else{
          this.allCards.unshift(this.changedCard)
        }
      }
      if(this.deletedCard!=null){
        this.allCards = this.allCards.filter(crd => crd.IDCARD != this.deletedCard.IDCARD)
      }
     
    }
    
  }
  getCards(){
    this.dbDonnector.getCards().subscribe(val =>{
      console.log(val)
      console.log("cards",val)
      this.allCards=val
      //var cards:Card[] = []
      //val.forEach(e => {
      //  cards.push(e)
      //});
      console.log("cardice",this.allCards)
    },error=>{
      console.log("erroric") 
    })
  }
  cardChosen(card){
    if(card=='0'){
      //nova kartica
      this.cardClicked.emit({IDCARD:null,TEXT:"",PROFILEID:null,NAME:"",TAGS:[],CardOrder:0,SHARED:0,FORMAT:{IMAGE1:null,IMAGE2:null,IMAGE3:null,COLOR:'#fff',FONT_SIZE:2,FONT_FORMAT:1,LAYOUT:0}})
    }else{
      this.cardClicked.emit(card);
    }
    
  }
  getImage(cardUrl){
    var imageUrl
    if(this.doneCards>this.allCards.length){
      return
    }
    console.log("cardurl je", cardUrl)
    this.doneCards+=1
    this.dbDonnector.getImage(cardUrl).then(res=>{
      console.log("AAAAAAA", res.toString()+"")
      imageUrl = res
      console.log("imageurl ",imageUrl)
      return imageUrl+""
    })
    
  }

}
