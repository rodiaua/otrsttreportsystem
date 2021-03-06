import { Injectable } from "@angular/core";
import { Period } from "../models/dto/period";
import { TTDynamicFields } from "../models/dto/tt-dynamic-fileds";
import { HttpClient } from "@angular/common/http";
import { TicketReport } from "../models/dto/ticket-report";
import { FilteringItems } from "../models/dto/filtering-items";
import { Filters } from '../models/dto/filters';
import { Report } from '../models/dto/report';
import { LogItem } from "../models/log-item";
import {OtrsTicket} from "../models/otrs-ticket";
import { AcknowledgedTicket } from "../models/acknowledgedTicket";
import {PendedTicket} from "../models/pended-ticket"


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
        return this.http.get<OtrsTicket[]>(`${this.domain}/getPendingTickets`).toPromise();
    }

    getAcknowledgedTickets(){
        return this.http.get<OtrsTicket[]>(`${this.domain}/getAcknowledgedTickets`).toPromise();
    }

    saveAcknowledgedTickets(acknowledgedTickets: AcknowledgedTicket[]){
        return this.http.post(`${this.domain}/saveAcknowledgedTickets`,acknowledgedTickets).toPromise();
    }

    removeAcknowledgedTickets(ids: number[]){
        return this.http.post(`${this.domain}/removeAcknowledgedTickets`,ids).toPromise();
    }

    getPendingTicketLogs(){
        return this.http.get<LogItem[]>(`${this.domain}/getPendingTicketLogs`).toPromise();
    }

    savePendedTicket(ticketId:number, overdue: number){
        return this.http.post(`${this.domain}/savePendedTicket`,{id:0, ticketId, overdue}).toPromise();
    }

    getPendedTickets(period: Period){
        return this.http.post<PendedTicket[]>(`${this.domain}/getPendedTickets`,period).toPromise();
    }

    getPendedTicketsTotal(period: Period){
        return this.http.post<number>(`${this.domain}/totalPendedTickets`,period).toPromise();
    }


}