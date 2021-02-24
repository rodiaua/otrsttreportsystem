import { Injectable } from "@angular/core";
import { Period } from "../models/dto/period";
import { TTDynamicFields } from "../models/dto/tt-dynamic-fileds";
import { HttpClient } from "@angular/common/http";
import { TicketReport } from "../models/dto/ticket-report";
import { FilteringItems } from "../models/dto/filtering-items";
import { Filters } from '../models/dto/filters';
import { Report } from '../models/dto/report';


@Injectable({ providedIn: 'root' })
export class OtrsTTService{
    private readonly domain = "api/otrsTicket";

    constructor(private http: HttpClient) { }

    getTicketsReport(period: Period){
        return this.http.post<TicketReport[]>(`${this.domain}/getTicketsReport`, period).toPromise();
    }

    getTicketsTotalRecord(period: Period){
        return this.http.post<number>(`${this.domain}/getTicketTotalNumber`, period).toPromise();
    }

    getFilteredTicketsReport(filters: Filters){
        return this.http.post<Report>(`${this.domain}/getFilteredTicketsReportBulk`, filters).toPromise();
    }

    getFilteringItems(period: Period){
        return this.http.post<FilteringItems>(`${this.domain}/getFilteringItems`, period).toPromise();
    }

    downloadReport(filters: Filters){
        return this.http.post(`${this.domain}/downloadReport`, filters, {responseType: 'blob'});
    }

    getPendingTickets(){
        return this.http.get(`${this.domain}/getPendingTickets`).toPromise();
    }

    getAcknowledgedTickets(){
        return this.http.get(`${this.domain}/getAcknowledgedTickets`).toPromise();
    }

    saveAcknowledgedTickets(ids: number[]){
        return this.http.post(`${this.domain}/saveAcknowledgedTickets`,ids).toPromise();
    }

    removeAcknowledgedTickets(ids: number[]){
        return this.http.post(`${this.domain}/removeAcknowledgedTickets`,ids).toPromise();
    }
}