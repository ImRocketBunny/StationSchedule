import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { MqttModule, IMqttServiceOptions } from 'ngx-mqtt';

const MQTT_SERVICE_OPTIONS: IMqttServiceOptions = {
  hostname: '127.0.0.1',
  port: 1883,
  path: '/station'
};

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    MqttModule.forRoot(MQTT_SERVICE_OPTIONS) // Rejestracja modułu ngx-mqtt
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
