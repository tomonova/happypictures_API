import { Component, OnInit } from '@angular/core';
import { Card } from 'src/app/models/cardModel';

@Component({
  selector: 'app-card-maker',
  templateUrl: './card-maker.component.html',
  styleUrls: ['./card-maker.component.css']
})
export class CardMakerComponent implements OnInit {
  clickedCard=null
  isCardSelected = false
  currCreating = false

  editedCard = null
  deletedCard = null
  constructor() { }

  ngOnInit(): void {
  }
  cardSelected(event){
    console.log(event)
    this.isCardSelected = true
    var kartica = new Card()
    kartica = JSON.parse(JSON.stringify(event))
    if(kartica.IDCARD==null){
      this.currCreating=true
    }
    this.clickedCard=kartica
  }
  cardEdited(ev){
    this.editedCard = ev
    if(this.currCreating){
      //dodana nova kartica
     
    }else{
      //izmjenjena stara kartica
   
    }
  
  }
  cardDeleted(card){
    this.deletedCard=card
    this.clickedCard=null
  }
}
