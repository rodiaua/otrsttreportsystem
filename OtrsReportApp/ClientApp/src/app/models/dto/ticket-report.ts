export interface TicketReport {
    tn: number,
    createTime: number,
    client: string,
    zone: string,
    type: string,
    description: string,
    direction: string,
    natInt: string,
    category: string,
    state: string,
    initiator: string,
    ticketPriority: string,
    closeTime: number
}
