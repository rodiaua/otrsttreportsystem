import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { LogItem } from "../models/log-item";
import * as signalR from "@microsoft/signalr";
import { HttpClient } from "@angular/common/http";

@Injectable({ providedIn: "root" })
export class LoggingHubService {
    private baseUrl = {
        server: 'http://localhost:5000/',
        domain: 'http://otrsreport.gms-worldwide.com/',
        apiUrl: 'api/'
    }
    private connection: signalR.HubConnection;
    logItems = new Subject<LogItem[]>();
    connectionEstablished = new Subject<Boolean>();

    get isLinux(){
        return localStorage.getItem("os") == "Linux";
    }

    constructor() {}

    async connect() {
        if (!this.connection) {
                if(this.isLinux){
                    this.connection = new signalR.HubConnectionBuilder().withUrl(this.baseUrl.domain+'logs').build();
                }else{
                    this.connection = new signalR.HubConnectionBuilder().withUrl(this.baseUrl.server+'logs').build();
                }
                
                this.connection.start().then(() => { this.connectionEstablished.next(true) }).catch(err => console.log(err));

                this.connection.on('GetLog', (logItems) => {
                    this.logItems.next(logItems);
                })
        }
    }

    // get isLinux(){
    //     console.log(navigator);
    //     return navigator.appVersion.indexOf("Linux") != -1;
    // }

    disconnect() {
        if (this.connection) {
            this.connection.stop();
            this.connection = null;
            console.log("connection closed")
        }
    }
}