import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from './material/material.module'
import { ApiService } from './api/api.service';
import { interval, Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'

})
export class AppComponent {
  private subscription: Subscription;
  title = 'TimeTable';
  constructor(private apiService: ApiService) {
    this.subscription = interval(10000).pipe(
      switchMap(() => this.apiService.postData({ topic: 'station/main/arrivals' }))
    ).subscribe({
      next: (response) => {
        response.forEach(function (course){
          console.log(course.name);
        })
      },
      error: (error) => {
        console.error('Błąd:', error);
      },
    });

  }
}
