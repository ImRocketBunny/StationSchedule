import { Component } from '@angular/core';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { Train } from '../models/train';
import { TrainService } from '../services/train.service';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'StationHubNG';
  trains: Train[] = []

  constructor(private trainService: TrainService) { }

  ngOnInit(): void {
    this.trainService.getTrains().subscribe((result: Train[]) => this.trains = result);
  }
}
