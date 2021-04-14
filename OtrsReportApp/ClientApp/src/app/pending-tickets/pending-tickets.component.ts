import { Component, OnDestroy, OnInit } from '@angular/core';
import { OtrsTTService } from '../services/otrs-tt-service';
import { LoggingHubService } from '../services/logging-hub.service'
import { LogItem } from '../models/log-item';
import { EpochConverter } from '../extensions/epoch-converter';
import * as moment from 'moment';
import * as momentTimezone from 'moment-timezone';
import { OtrsTicket } from '../models/otrs-ticket';
import { timer } from 'rxjs';
import { AcknowledgedTicket } from '../models/acknowledgedTicket';



@Component({
  selector: 'app-pending-tickets',
  templateUrl: './pending-tickets.component.html',
  styleUrls: ['./pending-tickets.component.scss']
})
export class PendingTicketsComponent implements OnInit, OnDestroy {

  list1: OtrsTicket[] = [];
  list2: OtrsTicket[] = [];
  logItems: LogItem[] = [];
  logs: string[] = [];
  loadDataInBackground = true;
  searchPattern = "";
  pendedTicketsOpened = false;
  isLoading = false;


  natIntTypes = [
    { name: "National", key: "NationalKey"},
    { name: "International", key:"InternationalKey" },
    { name: "All", key: "All" }
  ];

  natIntSelected: { name: string, key:string } = { name: "International", key:"InternationalKey" };

  constructor(private otrsService: OtrsTTService, private logService: LoggingHubService) { }
  ngOnDestroy(): void {
    this.logService.disconnect();
    this.loadDataInBackground = false;
  }

  async onSelectedHandle(event){
    this.isLoading = true;
    this.list1 = await this.otrsService.getPendingTickets(event.value.key);
    this.list1.sort((a, b) => moment.utc(a.createTime).valueOf() - moment.utc(b.createTime).valueOf());
    this.list2 = await this.otrsService.getAcknowledgedTickets(event.value.key);
    this.list2.sort((a, b) => moment.utc(a.createTime).valueOf() - moment.utc(b.createTime).valueOf());
    this.isLoading = false;
  }

  searchModelChange(event) {
    this.logs = this.logItems.map(l => this.logItemToString(l)).filter(l => this.searchPattern != "" ? l.includes(this.searchPattern) : true);
  }

  async ngOnInit() {
    this.startLoadDataInBackground();
  }

  delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  async startLoadDataInBackground() {
    while (this.loadDataInBackground) {
      this.list1 = await this.otrsService.getPendingTickets(this.natIntSelected.key);
      this.list1.sort((a, b) => moment.utc(a.createTime).valueOf() - moment.utc(b.createTime).valueOf());
      this.list2 = await this.otrsService.getAcknowledgedTickets(this.natIntSelected.key);
      this.list2.sort((a, b) => moment.utc(a.createTime).valueOf() - moment.utc(b.createTime).valueOf());
      await this.delay(1000 * 60);
    }
  }

  onMoveToTargetHandle(event) {
    this.list2.sort((a, b) => moment.utc(a.createTime).valueOf() - moment.utc(b.createTime).valueOf());
    this.otrsService.saveAcknowledgedTickets((event.items as OtrsTicket[]).map(x => {
      return <AcknowledgedTicket>
        {
          ticketId: x.id,
          createTime: moment(x.createTime).unix(),
          natInt: x.natInt
        }
    }));
  }

  onMoveToSourceHandle(event) {
    this.list1.sort((a, b) => moment.utc(a.createTime).valueOf() - moment.utc(b.createTime).valueOf());
    this.otrsService.removeAcknowledgedTickets(event.items.map(x => x.id));
  }

  onMoveAllToTargetHandle(event) {
    this.list2.sort((a, b) => moment.utc(a.createTime).valueOf() - moment.utc(b.createTime).valueOf());
    this.otrsService.saveAcknowledgedTickets((event.items as OtrsTicket[]).map(x => {
      return <AcknowledgedTicket>
        {
          ticketId: x.id,
          createTime: moment(x.createTime).unix(),
          natInt: x.natInt
        }
    }));
  }

  onMoveAllToSourceHandle(event) {
    this.list1.sort((a, b) => moment.utc(a.createTime).valueOf() - moment.utc(b.createTime).valueOf());
    this.otrsService.removeAcknowledgedTickets(event.items.map(x => x.id));
  }

  async onChangeHandle(event) {
    this.pendedTicketsOpened = false;
    if (event.index == 1) {
      this.loadDataInBackground = false;
      this.logItems = await this.otrsService.getPendingTicketLogs();
      this.logs = this.logItems.map(l => this.logItemToString(l)).filter(l => this.searchPattern != "" ? l.includes(this.searchPattern) : true);
      this.logService.connect();
      this.logService.connectionEstablished.subscribe(succeeded => { if (succeeded) console.log("hub connection started"); });
      this.logService.logItems.subscribe(logItems => {
        this.logItems.unshift(...logItems)
        this.logs = this.logItems.map(l => this.logItemToString(l)).filter(l => this.searchPattern != "" ? l.includes(this.searchPattern) : true);
      });
    }
    else if (event.index == 0) {
      this.loadDataInBackground = true;
      this.startLoadDataInBackground();
      this.logService.disconnect();
    } else if (event.index == 2) {
      this.pendedTicketsOpened = true;
    }
  }

  epochToString(epochTime: number) {
    return EpochConverter.toHumanReadableString(epochTime)
  }

  isoTimeToDateTime(time: string) {
    return moment(time).format("DD.MM.YYYY HH:mm:ss");
  }

  logItemToString(logItem: LogItem) {
    return `${this.isoTimeToDateTime(logItem.time)} [${logItem.firstName || logItem.lastName ? logItem.firstName + " " + logItem.lastName + " (" + logItem.userName + ")" : logItem.userName}] ${logItem.content}`;
  }

  getTicketOverdueInMinutes(ticket: OtrsTicket) {
    var now = momentTimezone(moment.now());
    var cr = moment.utc(ticket.createTime).local();
    var diff = now.diff(cr);
    return Math.round(diff / 60000);
  }

  getSeverity(ticket: OtrsTicket) {
    var overdue = this.getTicketOverdueInMinutes(ticket);
    if (overdue < 60) return 'success';
    return overdue > 180 ? 'danger' : 'warning';
  }

}
