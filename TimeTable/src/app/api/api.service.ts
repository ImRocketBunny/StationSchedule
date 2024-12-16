import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HttpHeaders } from '@angular/common/http';
import { Course } from './../models/course';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = 'http://localhost:5271/api/Details';

  constructor(private http: HttpClient) { }



  postData(data: any): Observable<Course[]> {
    const url = `${this.apiUrl}`;
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });
    return this.http.post<Course[]>(url, data);
  }
}
