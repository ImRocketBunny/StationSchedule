import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { FormsModule } from "@angular/forms";

import { AppComponent } from "./app.component";
//import { HelloComponent } from "./hello.component";
import { HttpClientModule } from "@angular/common/http";
import { MaterialModule } from "./material/material.module";


@NgModule({
  imports: [BrowserModule, FormsModule, HttpClientModule, MaterialModule],
  declarations: [AppComponent],
  bootstrap: [AppComponent]

  
})
export class AppModule { }
