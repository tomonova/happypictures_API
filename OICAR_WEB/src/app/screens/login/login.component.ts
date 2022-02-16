import { Component, OnInit } from '@angular/core';

import {FormBuilder, FormControl, FormGroup, Validators} from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  registerForm: FormGroup;
  public loginInvalid = false;
  private formSubmitAttempt = false;
  loading=false
  currTabIndex=0
  constructor(private fb: FormBuilder,
    private router: Router,
    private authService: AuthService) { 
      this.loginForm = this.fb.group({
        username: ['', Validators.email],
        password: ['', Validators.required]
      });
      this.registerForm = this.fb.group({
        username: ['', Validators.email],
        password: ['', Validators.required],
        passwordRep: ['', Validators.required],
        name: ['', Validators.required],
        surname: ['', Validators.required],
        nickname: ['', Validators.required]
      });
  }

  ngOnInit(): void {
    
  }
  logIn(){
    
  }
  signUp(){

  }
  async onSubmit(): Promise<void> {
    this.loading = true
    this.loginInvalid = false;
    this.formSubmitAttempt = false;
    if (this.loginForm.valid) {
      try {
        const username = this.loginForm.get('username')?.value;
        const password = this.loginForm.get('password')?.value;
        var success = await this.authService.login(username, password);
        console.log("ovo printam tek nakon logina")
        this.loading=false
      } catch (err) {
        this.loginInvalid = true;
        this.loading = false
      }
    } else {
      this.formSubmitAttempt = true;
      this.loading = false
    }
  }
  async onSubmitRegister(): Promise<void> {
    this.loading=true
    this.loginInvalid = false;
    this.formSubmitAttempt = false;
    console.log("Register attemped")
    if (this.registerForm.valid) {
      try {
        const username = this.registerForm.get('username')?.value;
        const password = this.registerForm.get('password')?.value;
        const passwordRep = this.registerForm.get('passwordRep')?.value;
        if(password != passwordRep){
          alert("Passwords need to match.")
          return
        }
        const name = this.registerForm.get('name')?.value;
        const surname = this.registerForm.get('surname')?.value;
        const nickname = this.registerForm.get('nickname')?.value;
        this.authService.register2(username, password,name,surname,nickname).subscribe(val=>{
          this.loading=false
          console.log("register success")
          this.router.navigateByUrl("home")
        },err=>{
          console.log("desio se error")
          this.loading=false
        })
       
      } catch (err) {
        this.loading=false
        console.log("invalid login")
        this.loginInvalid=true
      }
    } else {
      this.loading=false
      this.formSubmitAttempt = true;
    }
  }
  createNewAcc(){
    this.currTabIndex=1
  }
  backToLoginClick(){
    this.currTabIndex=0
  }
}
