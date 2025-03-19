import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatButtonModule } from '@angular/material/button';
import { HttpClientModule, provideHttpClient } from "@angular/common/http";
import { MatCardModule } from '@angular/material/card';
import { MqttModule, IMqttServiceOptions } from 'ngx-mqtt';
//import { VideogularModule } from 'ngx-videogular';
//import { MatVideoModule } from 'mat-video';

const MQTT_SERVICE_OPTIONS: IMqttServiceOptions = {
  hostname: '127.0.0.1', // Twój broker MQTT
  port: 1884, // Port WebSocket
  path: '/mqtt' // Ścieżka WebSocket
};


@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    MatButtonModule, MatDividerModule, MatIconModule, MatCardModule,
    MqttModule.forRoot(MQTT_SERVICE_OPTIONS) // Rejestracja MQTT
  ],
  providers: [
    provideAnimationsAsync(),
    provideHttpClient()
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
