import { Injectable } from "@angular/core";
import { Action } from "rxjs/internal/scheduler/Action";
@Injectable({providedIn:"root"})
export class Timer{

    private subscribed = false;

    async performEvery(seconds: number){
        this.subscribed = true;
        return 
    }
}