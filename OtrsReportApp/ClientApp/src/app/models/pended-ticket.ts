export interface PendedTicket{
    ticketId: number,
    tn: string,
    createTime: number,
    client: string,
    zone: string,
    type: string,
    description: string,
    direction: string,
    natInt: string,
    state: string,
    initiator: string,
    ticketPriority: string,
    closeTime: number,
    overdue: number
}