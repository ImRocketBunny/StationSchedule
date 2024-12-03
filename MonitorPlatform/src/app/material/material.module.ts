import { NgModule } from '@angular/core';
import { MatButtonModule } from "@angular/material/button";
import { MatDividerModule } from '@angular/material/divider';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatIconModule } from '@angular/material/icon';
const MaterialComponents = [
  MatButtonModule, MatDividerModule, MatSlideToggleModule, MatIconModule
];

@NgModule({
  imports: [
    [...MaterialComponents]
  ],
  exports: [
    [...MaterialComponents]
  ]

})
export class MaterialModule { }
