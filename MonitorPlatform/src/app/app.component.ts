import { Component, OnDestroy } from '@angular/core';
import { ApiService } from './api.service';
import { interval, Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { FullCourse } from './fullcourse';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css'],
    standalone: false
})
export class AppComponent implements OnDestroy {
  line: string | null = null
  courseId: string | null= null
  responseData: any
  name: string | null = null;
  delay: string | null = null;
  headsign: string | null = null;
  headsignFrom: string | null = null;
  departureTime: string | null = null;
  errorMessage: string | null = null;
  routeTo: string[] | null = null;
  routeFrom: string[] | null = null;

  title = 'MonitorPlatform'
  private subscription: Subscription;
  constructor(private apiService: ApiService) {
    this.subscription = interval(10000).pipe(
      switchMap(() => this.apiService.postData({ topic: 'station/II/22/lcd' })) // Wykonuje zapytanie GET
    ).subscribe({
      next: (response) => {
        this.line = response.name == null ? "" : response.name.split("   ").length > 1 ? response.name.split("   ")[1] : response.name.split(" ")[0]
        this.courseId = response.name == null ? "" : response.name.split("   ").length > 1 ? response.name.split("   ")[0] : response.name.split("   ")[0]
        this.delay = response.delay;
        this.headsign = response.headsignTo == null ? response.headsignFrom : response.headsignTo;
        this.routeTo = response.routeTo == null ? [] : response.routeTo.replace(" • ", " -  ").split(" -  ")
        this.routeFrom = response.routeFrom == null ? [] : response.routeFrom.replace(" • ", " -  ").split(" -  ")
        this.departureTime = response.departureTime == null ? response.arrivalTime : response.departureTime;

        this.responseData = response; // Aktualizacja danych
        this.errorMessage = null;
      },
      error: (error) => {
        this.errorMessage = 'Błąd podczas pobierania danych.';
        console.error('Błąd:', error);
      },
    });
  }

  /*sendData() {
    const body = {
      topic: 'station/V/4/lcd'
    };*/

    /*const headerDict = {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      'Access-Control-Allow-Headers': 'Content-Type',
      'Access-Control-Allow-Origin': 'http://localhost:4200/'
    }*/

    /*const requestOptions = {
      headers: new Headers(headerDict),
      body: body
    };/

    /*this.apiService.postData(body).subscribe({
      next: (response) => {
        this.responseData = response; // Przypisanie odpowiedzi do właściwości
        this.errorMessage = null; 
      },
      error: (error) => {
        this.errorMessage = 'Wystąpił błąd podczas pobierania danych.';
      }
    });
  }
  ngOnDestroy(): void {
    // Unieważnienie subskrypcji przy niszczeniu komponentu
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }*/
}
