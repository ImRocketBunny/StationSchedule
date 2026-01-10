import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { TrainService } from '../services/train.service'
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { StationLinkMenuComponent } from './station-link-menu/station-link-menu.component';
import { Menu } from "primeng/menu";

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule, HttpClientModule, StationLinkMenuComponent,
    Menu
],
  providers: [],
  exports: [StationLinkMenuComponent],
  bootstrap: [AppComponent]
})
export class AppModule { }
