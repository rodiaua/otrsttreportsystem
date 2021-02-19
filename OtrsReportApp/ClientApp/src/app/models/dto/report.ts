import {TicketReport} from '../dto/ticket-report';
import { FilteringItems } from "../dto/filtering-items";
export interface Report{
    ticketReportDTOs: TicketReport[],
    filteringItems: FilteringItems
}