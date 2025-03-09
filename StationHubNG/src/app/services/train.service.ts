import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../src/environments/environment';
import { Train } from '../models/train';
import { Observable } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class TrainService {
  private url = "gtfsrt"
  constructor(private http: HttpClient) { }

  public getTrains(): Observable<Train[]>{
    return this.http.get<Train[]>(environment.apiUrl+'/'+this.url);
    /*let train = new Train();
    train.name="Intercity";
    train.latitude=50.1234;
    train.longitude=20.1234;
    return [train]*/
  }

}
