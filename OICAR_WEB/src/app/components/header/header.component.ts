import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { User } from 'src/app/models/userModel';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  user:User
  constructor(public router:Router) {
    this.user=JSON.parse(localStorage.getItem("user"))
    console.log("naš user je ", this.user)
  }

  ngOnInit(): void {
  }
}
