import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  /**
   *
   */
  constructor(http: HttpClient) {
    http.get<boolean>("api/oS/isLinux").toPromise().then(result => {
      let value = localStorage.getItem("os");
      if(value != null || value != undefined){
        localStorage.removeItem("os");
        localStorage.setItem("os", result?"Linux":"Windows");
      }else{
        localStorage.setItem("os", result?"Linux":"Windows");
      }
    });
  }
} 
