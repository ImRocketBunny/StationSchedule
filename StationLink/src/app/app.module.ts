import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { TrainService } from '../services/train.service'
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule, TrainService
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
