import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams, HttpParamsOptions, HttpRequest, HttpResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, mergeMap, retry } from 'rxjs/operators';
import { Router } from '@angular/router';
import { User } from '../models/userModel';
import { Card } from '../models/cardModel';
import { formatDate } from '@angular/common';
import { DomSanitizer } from '@angular/platform-browser';
import { Story } from '../models/storyModel';
import { environment } from '../../environments/environment';
import { Image } from '../models/imageModel';
import { Schedule } from '../models/scheduleModel';

@Injectable({
  providedIn: 'root'
})
export class DbConnectorService {
  apiroot = environment.apiUrl;
  apirootImg = environment.apiUrlImg;
  user:User
  schedules = [{},{},{},{},{},{},{}]
  constructor(private http: HttpClient,private router:Router,private sanitizer: DomSanitizer) { 
    this.user=JSON.parse(localStorage.getItem("user"))
  }

  getCards(){
    let body = new URLSearchParams();
    var auth_token = localStorage.getItem("access_token")
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${auth_token}`
    })
    const params = new HttpParams()
    .set("userID",this.user.id+"")
    return this.http.get<any>(this.apiroot+"/api/Cards/GetCards",{headers:headers,params:params})
  }
  getDiscoverStories(){
    let body = new URLSearchParams();
    var auth_token = localStorage.getItem("access_token")
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${auth_token}`
    })
    const params = new HttpParams()
    .set("userID",this.user.id+"")
    return this.http.get<any>(this.apiroot+"/api/Stories/DiscoverStories",{headers:headers,params:params})
  }
  getFavouriteStories(){
    var auth_token = localStorage.getItem("access_token")
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${auth_token}`
    })
    const params = new HttpParams()
    .set("userID",this.user.id+"")
    return this.http.get<any>(this.apiroot+"/api/Stories/GetUserFavouriteStories",{headers:headers,params:params})
  }
  getUserStories(){
    var auth_token = localStorage.getItem("access_token")
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${auth_token}`
    })
    const params = new HttpParams()
    .set("userID",this.user.id+"")
    return this.http.get<any>(this.apiroot+"/api/Stories/GetUserStories",{headers:headers,params:params})
  }
  getStoryCards(story){
    var auth_token = localStorage.getItem("access_token")
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${auth_token}`
    })
    const params = new HttpParams()
    .set("storyID",story.IDSTORY+"")
    return this.http.get<any>(this.apiroot+"/api/Cards/GetStoryCards",{headers:headers,params:params})
  }
  insertNewStory(story:Story){
    var auth_token = localStorage.getItem("access_token")
    if(!this.user){
      this.user=JSON.parse(localStorage.getItem("user"))
    }
    var auth_token = localStorage.getItem("access_token")
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${auth_token}`,
      'Content-Type': 'application/json'
    })
    story.PROFILEID = this.user.id
    const params = new HttpParams().set("userID",this.user.id+"")
    
    var storyCardIds =[]
    story.CARDS.forEach(crd => {
      storyCardIds.push({"IDCARD":crd.IDCARD})
    })
    var story2 = JSON.parse(JSON.stringify(story))
    delete story2.IDSTORY
    delete story2.CARDS
    console.log("saljemo",story)
    return this.http.post<any>(this.apiroot+"/api/Story/InsertStory",story2,{headers:headers,params:params}).pipe(mergeMap(
      (val) =>{
        const params2 = new HttpParams().set("storyId",val.IDSTORY+"")
        localStorage.setItem("new_story_id",val.IDSTORY)
        console.log("saljemo u managecards sa", val.IDSTORY, "idem storija i slikama", storyCardIds)
        return this.http.put<any>(this.apiroot+"/api/Cards/ManageStoryCards",storyCardIds,{headers:headers,params:params2})
      }
    ))
  }
  updateStory(story:Story){
    var auth_token = localStorage.getItem("access_token")
    if(!this.user){
      this.user=JSON.parse(localStorage.getItem("user"))
    }
    var auth_token = localStorage.getItem("access_token")
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${auth_token}`,
      'Content-Type': 'application/json'
    })
    story.PROFILEID = this.user.id

    const params = new HttpParams().set("userId",this.user.id+"")
    const params2 = new HttpParams().set("storyId",story.IDSTORY+"")
    var storyCardIds =[]
    story.CARDS.forEach(crd => {
      storyCardIds.push({"IDCARD":crd.IDCARD})
    })
    //delete story.CARDS
    console.log("saljemo",story)
    return this.http.put<any>(this.apiroot+"/api/Story/UpdateStory",story,{headers:headers,params:params}).pipe(mergeMap(
      (val) =>{
        console.log("dobili smo za update", val, "a saljemo za manage cards", storyCardIds, "sa story idem", story.IDSTORY, "moj profil", this.user.id, " story profil", story.PROFILEID, " shareana ", story.SHARED)
        console.log("json", JSON.stringify(storyCardIds))
        return this.http.put<any>(this.apiroot+"/api/Cards/ManageStoryCards",storyCardIds,{headers:headers,params:params2})
      }
    ))
  }
  deleteStory(story:Story){
    var auth_token = localStorage.getItem("access_token")
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${auth_token}`,
    })
    console.log("saljemo", story)
    const params = new HttpParams().set("userID",this.user.id+"").set("storyID",story.IDSTORY+"")
    return this.http.delete<any>(this.apiroot+"/api/Story/DeleteStory",{headers:headers, params:params})
  }
  deleteImage(imageId){
    var auth_token = localStorage.getItem("access_token")
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${auth_token}`,
    })
    console.log("saljemo", imageId)
    const params = new HttpParams().set("idImage",imageId+"")
    return this.http.delete<any>(this.apiroot+"/api/Images/DeleteImage",{headers:headers, params:params})
  }
  insertNewCard(card:Card){
    if(!this.user){
      this.user=JSON.parse(localStorage.getItem("user"))
    }
    var auth_token = localStorage.getItem("access_token")
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${auth_token}`,
    })
    card.PROFILEID = this.user.id
    const params = new HttpParams().set("card",JSON.stringify(card))
    delete card.IDCARD
    console.log("saljemo",card)
    return this.http.post<any>(this.apiroot+"/api/Cards/InsertCard",card,{headers:headers})
  }
  updateCard(card:Card){
    var auth_token = localStorage.getItem("access_token")
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${auth_token}`,
    })
    console.log("saljemo", card)
    return this.http.put<any>(this.apiroot+"/api/Cards/EditCard",card,{headers:headers})
  }
  deleteCard(card:Card){
    var auth_token = localStorage.getItem("access_token")
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${auth_token}`,
    })
    console.log("saljemo", card)
    const params = new HttpParams().set("userID",this.user.id+"").set("cardID",card.IDCARD)
    return this.http.delete<any>(this.apiroot+"/api/Cards/DeleteCard",{headers:headers, params:params})
  }
  getImage(cardUrl){
    var cardNameS = cardUrl.split("/")
    var cardName = cardNameS[cardNameS.length-1]
    console.log("cardName je",cardName)
    var auth_token = localStorage.getItem("access_token")
    const params = new HttpParams()
    .set("sv","2020-08-04")
    .set("ss","bf")
    .set("srt","co")
    .set("sp","rwdlacitfx")
    .set("se","2022-02-28T17:20:54Z")
    .set("st","2021-12-31T09:20:54Z")
    .set("spr","https")
    .set("sig","5FvG2dmnxBii6U7LjFTXYjI%2BH57CVd67MH%2FxeHsj4JQ%3D")
    console.log("parametri su:",params)
    const headers = new HttpHeaders({
      'x-ms-version': `2020-12-06`,
      "x-ms-blob-type":"BlockBlob"
    })
    const options =  {headers:headers,responeType:"image/png"}
    var currentImageUrl
    let promise = new Promise<any>((resolve, reject) => {
      this.http.get(this.apiroot+"/images/"+cardName,{headers,responseType:"blob"}).toPromise().then(res=>{    
        currentImageUrl = this.sanitizer.bypassSecurityTrustUrl(URL.createObjectURL(res));
        resolve(currentImageUrl.changingThisBreaksApplicationSecurity)
      })}
    
    )
    return promise
    }
  getAllUserImages(){
    var auth_token = localStorage.getItem("access_token")
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${auth_token}`
    })
    const params = new HttpParams()
    .set("userId",this.user.id+"")
    return this.http.get<any>(this.apiroot+"/api/Images/GetUserImages",{headers:headers,params:params})
  }
  async getWholeWeekSchedule(startDate:Date){
    var auth_token = localStorage.getItem("access_token")
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${auth_token}`
    })
    const params = new HttpParams()
    .set("userId",this.user.id+"")
    .set("date",formatDate(startDate,"yyyy-MM-dd","en"))
    const params1 = new HttpParams()
    .set("userId",this.user.id+"")
    .set("date",formatDate(this.addDays(startDate,1),"yyyy-MM-dd","en"))
    const params2 = new HttpParams()
    .set("userId",this.user.id+"")
    .set("date",formatDate(this.addDays(startDate,2),"yyyy-MM-dd","en"))
    const params3 = new HttpParams()
    .set("userId",this.user.id+"")
    .set("date",formatDate(this.addDays(startDate,3),"yyyy-MM-dd","en"))
    const params4 = new HttpParams()
    .set("userId",this.user.id+"")
    .set("date",formatDate(this.addDays(startDate,4),"yyyy-MM-dd","en"))
    const params5 = new HttpParams()
    .set("userId",this.user.id+"")
    .set("date",formatDate(this.addDays(startDate,5),"yyyy-MM-dd","en"))
    const params6 = new HttpParams()
    .set("userId",this.user.id+"")
    .set("date",formatDate(this.addDays(startDate,6),"yyyy-MM-dd","en"))
    console.log(params.get("date"), "saljem za ponediljak",formatDate(this.addDays(startDate,0),"yyyy-MM-dd","en"))
    await this.http.get<any>(this.apiroot+"/api/Schedules/GetSchedule",{headers:headers,params:params}).toPromise().then(val=>{
      console.log("dobili za ponediljak", val)
      if(val!=null){
        this.schedules[0]=val
      }
    })
    
    await this.http.get<any>(this.apiroot+"/api/Schedules/GetSchedule",{headers:headers,params:params1}).toPromise().then(val=>{
      console.log("dobili za utorak", val)
      if(val!=null){
        this.schedules[1]=val
      }
    })
    console.log(params.get("date"), "saljem za sridu")
    await this.http.get<any>(this.apiroot+"/api/Schedules/GetSchedule",{headers:headers,params:params2}).toPromise().then(val=>{
      console.log("dobili za srida", val)
      if(val!=null){
        this.schedules[2]=val
      }
    })
    await this.http.get<any>(this.apiroot+"/api/Schedules/GetSchedule",{headers:headers,params:params3}).toPromise().then(val=>{
      console.log("dobili za cetvrtak", val)
      if(val!=null){
        this.schedules[3]=val
      }
    })
    await this.http.get<any>(this.apiroot+"/api/Schedules/GetSchedule",{headers:headers,params:params4}).toPromise().then(val=>{
      console.log("dobili za petak", val)
      if(val!=null){
        this.schedules[4]=val
      }
    })
    await this.http.get<any>(this.apiroot+"/api/Schedules/GetSchedule",{headers:headers,params:params5}).toPromise().then(val=>{
      console.log("dobili za subota", val)
      if(val!=null){
        this.schedules[5]=val
      }
    })
    await this.http.get<any>(this.apiroot+"/api/Schedules/GetSchedule",{headers:headers,params:params6}).toPromise().then(val=>{
      console.log("dobili za nedilja", val)
      if(val!=null){
        this.schedules[6]=val
      }
    })
  }
  addDays(date, days) {
    var result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
  }
  insertSchedule(sch:Schedule){
    if(!this.user){
      this.user=JSON.parse(localStorage.getItem("user"))
    }
    var auth_token = localStorage.getItem("access_token")
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${auth_token}`,
    })
    console.log("saljemo",sch)
    return this.http.post<any>(this.apiroot+"/api/Schedules/InsertSchedule",sch,{headers:headers})
  }
  uploadNewImage(image){
    if(!this.user){
      this.user=JSON.parse(localStorage.getItem("user"))
    }
    const formData: FormData = new FormData();
    formData.append('file', image)
    var genName = image.name + "_Web_"
    genName+= this.getRandomInt(1000)
    genName+= formatDate(new Date(),"dd-mm-yyyy-hh-mm-ss","en")
    console.log("name is ", genName)
    const headers = new HttpHeaders({
      'Content-Type': 'image',
      'x-ms-blob-type': `BlockBlob`,
      "x-ms-version":"2020-12-06"
    })
    const headersAuth = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem("access_token")}`
    })
    const req = new HttpRequest('PUT',this.apirootImg+"/images/"+genName+"?sv=2020-08-04&ss=bf&srt=co&sp=rwdlacitfx&se=2022-02-28T17:20:54Z&st=2021-12-31T09:20:54Z&spr=https&sig=5FvG2dmnxBii6U7LjFTXYjI%2BH57CVd67MH%2FxeHsj4JQ%3D", image, {
      reportProgress: false,
      headers:headers
    });
    var firstTime = true
    return this.http.request(req).pipe(mergeMap((val:any)=>{
      if(firstTime){
        firstTime=false
        console.log("firstTime", val)
        return []
      }
      var urlstr = "https://ojcar2storage2account.blob.core.windows.net/images/" + genName
      console.log("return vala", val)
      
      var tempImg:Image = {URL:urlstr,"IMGTIMESTAMP":formatDate(new Date(),"yyyy-MM-ddThh:mm:ss","en")+".984Z",PROFILEID:this.user.id,SHARED:0,IDIMAGE:-1}
      localStorage.setItem("added_img", JSON.stringify(tempImg))
      console.log("na api saljemo",{
        "URL":urlstr,
        "IMGTIMESTAMP":formatDate(new Date(),"yyyy-MM-ddThh:mm:ss","en")+".984Z",
        "PROFILEID":this.user.id
      } )
      return this.http.post<any>(this.apiroot+"/api/Images/InsertImage",{
        "URL":urlstr,
        "IMGTIMESTAMP":formatDate(new Date(),"yyyy-MM-ddThh:mm:ss","en")+".984Z",
        "PROFILEID":this.user.id
      },{headers:headersAuth})
    }))
  }
  getRandomInt(max) {
    return Math.floor(Math.random() * max);
  }
  async updateWholeSchedule(schedule:Schedule){
    var auth_token = localStorage.getItem("access_token")
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${auth_token}`
    })
    
    if(schedule.IDSCHEDULE != undefined){
      console.log("saljemo za delete ",schedule.IDSCHEDULE)
      const params = new HttpParams().set("idSchedule",schedule.IDSCHEDULE+"")
      await this.http.delete<any>(this.apiroot+"/api/Schedules/DeleteSchedule",{headers:headers,params:params}).toPromise()
    }
    return this.http.post<any>(this.apiroot+"/api/Schedules/InsertSchedule",schedule,{headers:headers}).toPromise()
  }
}
