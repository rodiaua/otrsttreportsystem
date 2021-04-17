export interface PendedTicket{
    ticketId: number,
    title: string,
    tn: string,
    createTime: number,
    client: string,
    problemSide: string,
    zone: string,
    type: string,
    description: string,
    direction: string,
    natInt: string,
    category: string,
    state: string,
    initiator: string,
    ticketPriority: string,
    closeTime: number,
    overdue: number,
    comment: TicketComment
}

export interface TicketComment{
    id: number,
    comment: string,
    commentedBy: string,
    updateTime: number
}