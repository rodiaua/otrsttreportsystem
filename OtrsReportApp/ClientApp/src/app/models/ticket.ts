import { Queue } from "./queue";
import { TicketState } from "./ticket-state";

export interface Ticket{

    id: number,
    tn: string,
    title: string,
    queueId: number,
    queue: Queue, 
    ticketState: TicketState,
    createTime: number
}