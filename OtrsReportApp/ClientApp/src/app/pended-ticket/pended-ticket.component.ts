import { Component, OnInit } from '@angular/core';
import * as moment from 'moment';
import { Moment } from 'moment';
import { EpochConverter } from '../extensions/epoch-converter';
import { PendedTicket } from '../models/pended-ticket';
import { OtrsTTService } from '../services/otrs-tt-service';

@Component({
  selector: 'app-pended-ticket',
  templateUrl: './pended-ticket.component.html',
  styleUrls: ['./pended-ticket.component.scss']
})
export class PendedTicketComponent implements OnInit {

  pendedTickets: PendedTicket[] = [];

  timeRange: { startDate: Moment, endDate: Moment } = {startDate: moment().startOf('day'), endDate:moment() };

  loadingInProcess: boolean = false;
  totalTickets: number;

  ranges: any = {
    'Today': [moment().startOf('day'), moment()],
    'Yesterday': [moment().subtract(1, 'days').startOf('day'), moment().subtract(1, 'days').endOf("day")],
    'Last 7 Days': [moment().subtract(6, 'days').startOf('day'), moment()],
    'Last 30 Days': [moment().subtract(29, 'days').startOf('day'), moment()],
    'This Month': [moment().startOf('month'), moment().endOf('month')],
    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')],
    'This Year' : [moment().startOf('year'), moment().endOf('year') ]
  }

  constructor(private otrsService: OtrsTTService) { }

  async ngOnInit(){
    await this.getPendedTickets();
  }

  epochToString(epochTime: number) {
    return EpochConverter.toHumanReadableString(epochTime)
  }

  getSeverity(overdue: number) {
    if (overdue < 60) return 'success';
    return overdue > 180 ? 'danger': 'warning';
  }

  async getPendedTickets(){
    this.loadingInProcess = true;
    this.totalTickets = await this.otrsService.getPendedTicketsTotal({
      startTime: this.timeRange.startDate.unix(),
       endTime:  this.timeRange.endDate.unix()});
      this.pendedTickets = await this.otrsService.getPendedTickets({
        startTime: this.timeRange.startDate.unix(),
         endTime:  this.timeRange.endDate.unix()}).finally(()=> {this.loadingInProcess = false});
  }


}
