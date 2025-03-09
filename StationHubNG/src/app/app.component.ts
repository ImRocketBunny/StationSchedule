import { HttpClientModule, HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Train } from './models/train';
import { TrainService } from './services/train.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [ HttpClientModule,CommonModule ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'StationHubNG';
  trains: Train[]=[]

  constructor(private trainService: TrainService){}

  ngOnInit():void{
    this.trainService.getTrains().subscribe((result: Train[])=>this.trains=result);
  }

}
