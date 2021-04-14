import {  SelectItem } from 'primeng/api';
export interface FilteringItems{
    zones: SelectItem[],
    types: SelectItem[],
    directions: SelectItem[],
    natInts: SelectItem[],
    states: SelectItem[],
    initiators: SelectItem[],
    ticketPriorities: SelectItem[],
    categories: SelectItem[],
    problemSides: SelectItem[]
}