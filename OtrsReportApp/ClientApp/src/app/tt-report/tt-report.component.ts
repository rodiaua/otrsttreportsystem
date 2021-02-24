import { Component, OnInit} from '@angular/core';
import { TicketReport } from '../models/dto/ticket-report';
import { OtrsTTService } from '../services/otrs-tt-service';
import { EpochConverter } from '../extensions/epoch-converter';
import { MessageService, MenuItem, SelectItem } from 'primeng/api';
import { Period } from '../models/dto/period';
import { Filters } from '../models/dto/filters';
import * as FileSaver from 'file-saver';
import { Report } from '../models/dto/report';

@Component({
  selector: 'app-tt-report',
  templateUrl: './tt-report.component.html',
  styleUrls: ['./tt-report.component.scss'],
  providers: [MessageService]
})
export class TtReportComponent implements OnInit {

  time: Date[];
  period: Period;
  ticketsReport: TicketReport[];
  report: Report;

  totalRecords: number;
  pagingMode;
  hasRecords;
  downloading;
  loadingData;
  first = 0;
  last = 20;

  zones: SelectItem[];
  types: SelectItem[];
  initiators: SelectItem[];
  directions: SelectItem[];
  natInts: SelectItem[];
  priorities: SelectItem[];
  states: SelectItem[];

  selectedZones: SelectItem[];
  selectedTypes: SelectItem[];
  selectedInitiators: SelectItem[];
  selectedDirections: SelectItem[];
  selectedNatInts: SelectItem[];
  selectedPriorities: SelectItem[];
  selectedStates: SelectItem[];



  constructor(private otrsService: OtrsTTService, private messageService: MessageService) { }

  ngOnInit(): void {
    this.downloading = false;
    this.hasRecords = false;
    this.pagingMode = true;
    this.loadingData = false;
    this.zones = [];
    this.types = [];
    this.initiators = [];
    this.directions = [];
    this.natInts = [];
    this.priorities = [];
    this.states = [];
  }

  epochToString(epochTime: number) {
    return EpochConverter.toHumanReadableString(epochTime)
  }

  async exportExcel() {
    this.downloading  = true;
    let filename = `report_${this.epochToString(Math.trunc((this.time[0].valueOf() / 1000)))}_${this.epochToString(Math.trunc((this.time[1].valueOf() / 1000)))}.xlsx`;
    this.otrsService.downloadReport(this.createFilter()).subscribe(resultBlob => {
      FileSaver.saveAs(resultBlob, filename);
      this.downloading = false;
    });;
  }

  async onApply() {
    if (this.time === undefined || this.time[1] === null) {
      this.showWarn('Specify the period!');
    }
    else {
      this.first = 0;
      this.loadingData = true;
      let start = this.time[0].valueOf();
      let end = this.time[1].valueOf();
      this.period = {
        startTime: Math.trunc(start / 1000),
        endTime: Math.trunc(end / 1000)
      }
      // this.totalRecords = await this.otrsService.getTicketsTotalRecord(this.period);
      this.report = await this.otrsService.getFilteredTicketsReport(this.createFilter());
      this.ticketsReport = this.report.ticketReportDTOs;
      if(this.ticketsReport.length > 0 ?? this.ticketsReport.length <= 1000) {
        this.hasRecords = true;
        this.pagingMode = true;
      }
      this.totalRecords = this.ticketsReport.length;

      let filteringItems = this.report.filteringItems;
      if(filteringItems !== null){
      filteringItems.zones?.length < 1 ? this.selectedZones = this.zones = [] : this.zones = filteringItems.zones;
      filteringItems.types?.length < 1 ? this.selectedTypes = this.types = [] : this.types = filteringItems.types;
      filteringItems.states?.length < 1 ? this.selectedStates = this.states = [] : this.states = filteringItems.states;
      filteringItems.ticketPriorities?.length < 1 ? this.selectedPriorities = this.priorities = [] : this.priorities = filteringItems.ticketPriorities;
      filteringItems.initiators?.length < 1 ? this.selectedInitiators = this.initiators = [] : this.initiators = filteringItems.initiators;
      filteringItems.natInts?.length < 1 ? this.selectedNatInts = this.natInts = [] : this.natInts = filteringItems.natInts;
      filteringItems.directions?.length < 1 ? this.selectedDirections = this.directions = [] : this.directions = filteringItems.directions;
      }
      this.loadingData = false;
    }
  }

  createFilter(): Filters {

    return {
      period: this.period,
      zones: this.selectedZones?.map(zone => zone),
      types: this.selectedTypes?.map(type => type),
      states: this.selectedStates?.map(state => state),
      directions: this.selectedDirections?.map(direction => direction),
      initiators: this.selectedInitiators?.map(initiator => initiator),
      natInts: this.selectedNatInts?.map(natInt => natInt),
      priorities: this.selectedPriorities?.map(priority => priority),
    }
  }

  onTodayClick() {
    let start = new Date(Date.now());
    start.setHours(0, 0);
    this.time = [start, new Date(Date.now())];
  }

  onYesterdayClick() {
    let start = new Date(Date.now());
    start.setDate(start.getDate() - 1);
    start.setHours(0, 0);
    this.time = [start];
    let end = new Date(start.getTime());
    end.setHours(23, 59, 59)
    this.time.push(end);
  }

  onWeekClick() {
    let start = new Date(Date.now());
    start.setDate(start.getDate() - 7);
    this.time = [start, new Date(Date.now())];
  }

  onTwoWeekClick() {
    let start = new Date(Date.now());
    start.setDate(start.getDate() - 14);
    this.time = [start, new Date(Date.now())];
  }

  onMonthClick() {
    let start = new Date(Date.now());
    start.setMonth(start.getMonth() - 1);
    this.time = [start, new Date(Date.now())];
  }

  onYearClick() {
    let start = new Date(Date.now());
    start.setMonth(start.getMonth() - 12);
    this.time = [start, new Date(Date.now())];
    // this.period = {
    //     startTime:  Math.trunc(this.time[0].valueOf() / 1000),
    //     endTime: Math.trunc(this.time[1].valueOf() / 1000)
    //   }
  }

  showWarn(message: string) {
    this.messageService.add({ severity: 'warn', summary: 'Warn', detail: message });
  }


}
