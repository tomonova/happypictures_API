import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, mergeMap, retry } from 'rxjs/operators';
import { Router } from '@angular/router';
import { User } from '../models/userModel';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  apiroot = environment.apiUrl;
  constructor(private http: HttpClient,private router:Router) { }

  async login(username,password){
    let body = new URLSearchParams();
    body.set('username', username);
    body.set('password', password);
    body.set('grant_type',"password")
      this.http.post<any>(this.apiroot+"/token",body.toString(),{headers : new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded grant_type' })}).subscribe(val =>{
        console.log(val)
        localStorage.setItem("access_token",val.access_token)
        localStorage.setItem("expires_in",val.expires_in)
        localStorage.setItem("user_name",val.userName)
        this.getAccount(val.access_token)
        return true
      },error =>{
        alert("Wrong email or password")
        return false
      })
  }
  async register(username,password, name,surname,nickname){
    let body = new URLSearchParams();
    body.set('Email', username);
    body.set('Password', password);
    body.set('ConfirmPassword',password)
    this.http.post<any>(this.apiroot+"/api/Account/Register",{'Email':username,'Password':password,'ConfirmPassword':password}).subscribe(val =>{
        console.log("succesful 1/3 registration")
        localStorage.setItem("user_id",val.IDPROFILE)
        this.login(username,password).then(val =>{
          console.log("REGISTER 2/3", val)
          this.updateProfile("",name,surname,nickname)
        }  
        )
        return true
      },error =>{
        return false
      })
  }
  register2(username,password, name,surname,nickname) :Observable<any>{
    return this.http.post<any>(this.apiroot+"/api/Account/Register",{'Email':username,'Password':password,'ConfirmPassword':password}).pipe(mergeMap(
      (val1) =>{
        console.log("register 1/3", val1)
        localStorage.setItem("user_id",val1[0].IDPROFILE)
        let body = new URLSearchParams();
        body.set('username', username);
        body.set('password', password);
        body.set('grant_type',"password")
        return this.http.post<any>(this.apiroot+"/token",body.toString(),{headers : new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded grant_type' })}).pipe(mergeMap(
          (val2) =>{
            console.log("register 2/3", val2)
            localStorage.setItem("access_token",val2.access_token)
            localStorage.setItem("expires_in",val2.expires_in)
            localStorage.setItem("user_name",val2.userName)
            //this.getAccount(val2.access_token)
            var user:User = {id:val1[0].IDPROFILE,first_name:name,last_name:surname,nickname:nickname,bio:"",email:username}
            localStorage.setItem("user",JSON.stringify(user))
            const headers = new HttpHeaders({
              'Authorization': `Bearer ${val2.access_token}`
            })
            console.log("saljem",{"BIO":"","FIRSTNAME":name,"IDPROFILE":val1[0].IDPROFILE,"LASTNAME":surname,"NICKNAME":nickname})
            return this.http.put<any>(this.apiroot+"/api/Account/UpdateProfile",{"BIO":"","FIRSTNAME":name,"IDPROFILE":val1[0].IDPROFILE,"LASTNAME":surname,"NICKNAME":nickname},{headers:headers})
          },error =>{
            console.log("error2")
            return error
          })
        )
      }),
      catchError( (err) => {
        console.log("erroe1",err)
        return err
    }
  ))
}

  async getAccount(auth_token){
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${auth_token}`
    })
    return this.http.get<any>(this.apiroot+"/api/Account/GetProfile",{headers:headers}).subscribe(val =>{
      console.log(val)
      var user:User = {id:val[0].IDPROFILE,first_name:val[0].FIRSTNAME,last_name:val[0].LASTNAME,nickname:val[0].NICKNAME,bio:val[0].BIO,email:val[0].EMAIL}
      console.log("user",user)
      localStorage.setItem("user",JSON.stringify(user))
      this.router.navigateByUrl("home")
    },error=>{
      
    })
  }
  async updateProfile(bio, firstname,lastname,nickname){
    let body = new HttpParams();
    var uid = localStorage.getItem("user_id")
    body.set('BIO', bio);
    body.set('FIRSTNAME', firstname);
    body.set('LASTNAME',lastname)
    body.set('NICKNAME',nickname)
    body.set('IDPROFILE',uid)
    this.http.put<any>(this.apiroot+"/api/Account/UpdateProfile",{params:body}).subscribe(val =>{
      console.log("succesful 1/3 registration", val)
      this.getAccount(val.access_token)
      return true
    },error =>{
      console.log("errror", error)
      return false
    })
  }
}
