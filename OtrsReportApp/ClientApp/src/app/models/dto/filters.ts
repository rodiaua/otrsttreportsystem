import {Period} from '../dto/period';
export interface Filters{
    period: Period
    zones: any[],
    types: any[],
    initiators: any[],
    directions: any[],
    natInts: any[],
    priorities: any[],
    states: any[],
    categories: any[],
    problemSides: any[]
}