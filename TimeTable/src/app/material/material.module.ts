import { NgModule } from '@angular/core';
import { MatButtonModule } from "@angular/material/button";
import { MatDividerModule } from '@angular/material/divider';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatTableModule } from '@angular/material/table'
const MaterialComponents = [
  MatButtonModule, MatDividerModule, MatSlideToggleModule, MatIconModule, MatListModule, MatTableModule
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
