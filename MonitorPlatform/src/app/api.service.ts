import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HttpHeaders } from '@angular/common/http';
import { FullCourse } from './fullcourse';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = 'http://localhost:5271/api/Details';

  constructor(private http: HttpClient) { }



  postData(data: any): Observable<FullCourse> {
    const url = `${this.apiUrl}`; // Endpoint API
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });
    return this.http.post<FullCourse>(url, data); // Wywołanie POST z body
  }
}
