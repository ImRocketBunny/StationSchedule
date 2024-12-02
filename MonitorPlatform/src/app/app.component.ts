import { Component, OnDestroy } from '@angular/core';
import { ApiService } from './api.service';
import { interval, Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnDestroy {
  responseData: any; // Właściwość do przechowywania odpowiedzi
  errorMessage: string | null = null;
  title = 'MonitorPlatform'// Właściwość do przechowywania błędu
  private subscription: Subscription;
  constructor(private apiService: ApiService) {
    this.subscription = interval(10000).pipe(
      switchMap(() => this.apiService.postData({ topic: 'station/WKD/lcd' })) // Wykonuje zapytanie GET
    ).subscribe({
      next: (response) => {
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
  }*/
  ngOnDestroy(): void {
    // Unieważnienie subskrypcji przy niszczeniu komponentu
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
