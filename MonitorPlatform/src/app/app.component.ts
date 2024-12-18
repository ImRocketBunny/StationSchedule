import { Component, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from './api/api.service';
import { interval, Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { MaterialModule } from './material/material.module'
import { FullCourse } from './models/fullcourse';
import { MatButtonModule } from "@angular/material/button";
import { MatDividerModule } from '@angular/material/divider';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatIconModule } from '@angular/material/icon';
import { RouterOutlet } from '@angular/router';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  standalone: false,
  //imports: [MaterialModule, RouterOutlet, CommonModule],
})
export class AppComponent implements OnDestroy {
  line: string | null = null
  courseId: string | null = null
  responseData: any
  name: string | null = null;
  delay: string | null = null;
  headsign: string | null = null;
  headsignFrom: string | null = null;
  departureTime: string | null = null;
  errorMessage: string | null = null;
  routeTo: string[] | null = null;
  routeFrom: string[] | null = null;
  route: string | null = null;
  headsignSize: number | null = null;
  routeSize: number | null = null;
  endOfTheLine: boolean | null = null;

  title = 'MonitorPlatform2'
  private subscription: Subscription;
  constructor(private apiService: ApiService) {
    this.subscription = interval(10000).pipe(
      switchMap(() => this.apiService.postData({ topic: 'station/IX/1/lcd' })) // Wykonuje zapytanie GET
    ).subscribe({
      next: (response) => {
        this.line = response.name == null ? "" : response.name.split("   ").length > 1 ? response.name.split("   ")[1] : response.name.split(" ")[0]
        this.courseId = response.name == null ? null : response.name.split("   ").length > 1 ? response.name.split("   ")[0] : response.name.split("   ")[0]
        this.delay = response.delay;
        this.headsign = response.headsignTo == "" ? response.headsignFrom : response.headsignTo;
        this.routeTo = response.routeTo == null ? [] : response.routeTo.replace(" • ", " -  ").split(" -  ")
        this.routeFrom = response.routeFrom == null ? [] : response.routeFrom.replace(" • ", " -  ").split(" -  ")
        this.departureTime = response.departureTime == null ? response.arrivalTime : response.departureTime;
        this.route = response.routeTo == "" ? this.routeFrom.slice(1, -1).join(' - ') : this.routeTo.slice(1, -1).join(' - ');
        this.responseData = response;
        this.errorMessage = null;
        this.headsignSize = this.headsign.length;
        this.routeSize = this.route.length;
        this.endOfTheLine = response.departureTime == null ? true : false;
        console.log(this.courseId);
        console.log(this.headsignSize);
        console.log(this.headsign);

      },
      error: (error) => {
        this.errorMessage = 'Błąd podczas pobierania danych.';
        console.error('Błąd:', error);
      },
    });
  }

  ngOnDestroy(): void {
    // Unieważnienie subskrypcji przy niszczeniu komponentu
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
