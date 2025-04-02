import { Position } from './position';

export class GtfsTrain {
    id: number = 0
    course_id: string="";
    trip_headsign: string="";
    position: Position = new Position();

}