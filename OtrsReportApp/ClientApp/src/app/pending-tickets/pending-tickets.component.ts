import { Component, OnInit } from '@angular/core';
import { OtrsTTService } from '../services/otrs-tt-service';

@Component({
  selector: 'app-pending-tickets',
  templateUrl: './pending-tickets.component.html',
  styleUrls: ['./pending-tickets.component.scss']
})
export class PendingTicketsComponent implements OnInit {

  list1: any = [];
  list2: any = [];
  logs: string = "";

  constructor(private otrsService: OtrsTTService) { }

  async ngOnInit(){
    this.list1 = await this.otrsService.getPendingTickets();
    this.list2 = await this.otrsService.getAcknowledgedTickets();
  }

  onMoveToTargetHandle(event){
    this.otrsService.saveAcknowledgedTickets(event.items.map(x => x.id));
  }

  onMoveToSourceHandle(event){
    this.otrsService.removeAcknowledgedTickets(event.items.map(x => x.id));
  }

  onMoveAllToTargetHandle(event){
    this.otrsService.saveAcknowledgedTickets(event.items.map(x => x.id));
  }

  onMoveAllToSourceHandle(event){
    this.otrsService.removeAcknowledgedTickets(event.items.map(x => x.id));
  }

  async onChangeHandle(event){
    if(event.index == 1){
      this.logs = await this.otrsService.getLogs();
    }
  }

}
