import { Route } from '@angular/compiler/src/core';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.scss']
})
export class NavBarComponent implements OnInit {

  items: MenuItem[];
  constructor(private router:Router, public service: UserService) {
    this.items = [];
    this.items.push({label: 'TT Report', routerLink: ['/ttreport'], icon: 'pi pi-calendar'})
    this.items.push({label: 'Users', routerLink: ['/user'], icon: 'pi pi-user'})
   }

  ngOnInit(): void {
  }

  onLogout(){
    localStorage.removeItem('token');
    localStorage.removeItem('otpToken');
    this.router.navigateByUrl('/login');
  }

  get userConfirmed(){
    return this.service.userConfirmed();
  }
}
