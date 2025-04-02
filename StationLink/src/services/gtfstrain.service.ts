
import { Injectable } from '@angular/core';
import { GtfsTrain } from '../models/gtfstrain';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
@Injectable({
  providedIn: 'root'
})


export class GtfsTrainService{
    private url = "gtfskmpositions"
    constructor(private http: HttpClient) { }

     public getKMTrains(): Observable<GtfsTrain[]>{
        return this.http.get<GtfsTrain[]>(environment.apiUrl+'/'+this.url);
        /*let train = new Train();
        train.name="Intercity";
        train.latitude=50.1234;
        train.longitude=20.1234;
        return [train]*/
      }

}