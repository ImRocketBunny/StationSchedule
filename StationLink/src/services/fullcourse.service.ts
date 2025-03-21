import { Injectable } from '@angular/core';
import { FullCourse } from '../models/fullcourse';

@Injectable({
  providedIn: 'root'
})
export class FullcourseService {

  constructor() { }

  public getFullCourse() : FullCourse{
    let course = new FullCourse();
    return course;
  }
}
