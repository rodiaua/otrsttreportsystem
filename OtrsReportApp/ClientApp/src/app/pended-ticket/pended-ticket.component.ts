import { stringify } from '@angular/compiler/src/util';
import { Component, OnInit } from '@angular/core';
import * as moment from 'moment';
import { Moment } from 'moment';
import { SelectItem } from 'primeng/api';
import { ColumnFilterFormElement } from 'primeng/table';
import { EpochConverter } from '../extensions/epoch-converter';
import { FilteringItems } from '../models/dto/filtering-items';
import { Filters } from '../models/dto/filters';
import { PendedTicket } from '../models/pended-ticket';
import { OtrsTTService } from '../services/otrs-tt-service';

@Component({
  selector: 'app-pended-ticket',
  templateUrl: './pended-ticket.component.html',
  styleUrls: ['./pended-ticket.component.scss']
})
export class PendedTicketComponent implements OnInit {

  pendedTickets: PendedTicket[] = [];

  timeRange: { startDate: Moment, endDate: Moment } = { startDate: moment().startOf('day'), endDate: moment() };

  loadingInProcess: boolean = false;
  totalTickets: number;

  displayComment = false;

  commentText = "";
  editingPendedTicket: PendedTicket;

  filteringItems: FilteringItems;

  zones: SelectItem[];
  types: SelectItem[];
  initiators: SelectItem[];
  natInts: SelectItem[];
  categories: SelectItem[];
  problemSides: SelectItem[];

  selectedZones: SelectItem<string>[];
  selectedTypes: SelectItem<string>[];
  selectedInitiators: SelectItem<string>[];
  selectedNatInts: SelectItem<string>[];
  selectedCategories: SelectItem<string>[];
  selectedProblemSides: SelectItem<string>[];

  ranges: any = {
    'Today': [moment().startOf('day'), moment()],
    'Yesterday': [moment().subtract(1, 'days').startOf('day'), moment().subtract(1, 'days').endOf("day")],
    'Last 7 Days': [moment().subtract(6, 'days').startOf('day'), moment()],
    'Last 30 Days': [moment().subtract(29, 'days').startOf('day'), moment()],
    'This Month': [moment().startOf('month'), moment().endOf('month')],
    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')],
    'This Year': [moment().startOf('year'), moment().endOf('year')]
  }

  constructor(private otrsService: OtrsTTService) { }

  async ngOnInit() {
    await this.loadFilters();
    await this.getPendedTickets();
  }

  toggleComments(tt: PendedTicket) {
    this.displayComment = this.displayComment ? false : true;
    if (this.displayComment) {
      this.commentText = tt.comment?.comment;
      this.editingPendedTicket = tt;
    } else {
      this.commentText = "";
      this.editingPendedTicket = null;
    }
  }

  isTicketCommented(tt: PendedTicket){
    
    return tt.comment?.comment != null &&  tt.comment?.comment.trim() != '' ? "green" :"darkgrey";
  }

  async loadFilters(){
    this.filteringItems = await this.otrsService.getDynamicFieldsWithValues();
    if(this.filteringItems !== null){
      this.filteringItems.zones?.length < 1 ? this.selectedZones = this.zones = [] : this.zones = this.filteringItems.zones;
      this.filteringItems.types?.length < 1 ? this.selectedTypes = this.types = [] : this.types = this.filteringItems.types;
      this.filteringItems.initiators?.length < 1 ? this.selectedInitiators = this.initiators = [] : this.initiators = this.filteringItems.initiators;
      this.filteringItems.natInts?.length < 1 ? this.selectedNatInts = this.natInts = [] : this.natInts = this.filteringItems.natInts;
      this.filteringItems.categories?.length < 1 ? this.selectedCategories = this.categories = [] : this.categories = this.filteringItems.categories;
      this. filteringItems.problemSides?.length < 1 ? this.selectedProblemSides = this.problemSides = [] : this.problemSides = this.filteringItems.problemSides;
    }
  }

  cancelCommentHandle(){
    this.displayComment = this.displayComment ? false : true;
    this.commentText = "";
    this.editingPendedTicket = null;
  }

  epochToString(epochTime: number) {
    return EpochConverter.toHumanReadableString(epochTime)
  }

  getSeverity(overdue: number) {
    if (overdue < 60) return 'success';
    return overdue > 180 ? 'danger' : 'warning';
  }

  async saveCommentHandle() {
    var com = await this.otrsService.addCommentToTicket({
      pendedTicketId: this.editingPendedTicket.ticketId,
      commentText: this.commentText,
      commentId: this.editingPendedTicket.comment?.id
    });
    this.pendedTickets.find(t => t.ticketId == this.editingPendedTicket.ticketId).comment = com;
    this.editingPendedTicket = null;
    this.displayComment = false;
  }

  async getPendedTickets() {
    this.loadingInProcess = true;
    this.pendedTickets = await this.otrsService.getPendedTickets(this.createFilter()).finally(() => { this.loadingInProcess = false });
    this.totalTickets = this.pendedTickets.length;
  }

  async onHideHandle(){
    await this.getPendedTickets();
  }

  createFilter(): Filters {
    var filters = {
      period: {startTime: this.timeRange.startDate.unix(), endTime: this.timeRange.endDate.unix()},
      zones: this.selectedZones?.map(zone => zone.label),
      types: this.selectedTypes?.map(type => type.label),
      states: null,
      directions: null,
      initiators: this.selectedInitiators?.map(initiator => initiator.label),
      natInts: this.selectedNatInts?.map(natInt => natInt.label),
      priorities: null,
      categories: this.selectedCategories?.map(category => category.label),
      problemSides: this.selectedProblemSides?.map(problemSide => problemSide.label)
    };
    return filters;
  }


}
