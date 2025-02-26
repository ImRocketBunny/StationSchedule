import { Component, ElementRef, Input, OnDestroy, ViewChild } from '@angular/core';
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
import { ActivatedRoute, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  standalone: false,
  //imports: [MaterialModule, RouterOutlet, CommonModule],
})
export class AppComponent implements OnDestroy {

  videoSrc = 'http://localhost:4200/logo-pkp_20250208182712-r20250208-1.webm';
  videoPlaylist: string[] = ['http://localhost:4200/logo-pkp_20250208182712-r20250208-1.webm','http://localhost:4200/logo-pkp_20250208182712-r20250208-1.webm']/*['PLK_wylamiane_rogatki_nowe-r20250123-7.webm', 'POK_skm_CZARODZIEJSKI-FLET-DLA-DZIECI_03.2025-r20250205-9.webm',
    'Praca_SKM_elektryk_1920x810-r20250116-5.webm', 'TS_Mahagonny_1920x810-r20241204-3.webm'
    , '4_UTK_animacja_BAGAZ_NEW-r20250113-5.webm','ZTM_Warszawa_mruga_9.02-r20250203-1.webm','POK_VENUS-AND-ADONIS_25.02-r20250115-3.webm'
  ,'TS_STARA-1920x810-r20241017-19.webm','ZTM_PLAKAT_E_HOLOGRAM_DLA_HB_TABOR_SKM_2-r20250117-9.webm','SKM_20lecie_1920x810-r20240510-15.webm' ]*/
  fileNum: number = this.videoPlaylist.length;
  videoNum: number = 0;
  videoStr: string | null ='';
  @ViewChild('videoPlayer') videoplayer!: ElementRef;

  icon: string = "https://www.mazowieckie.com.pl/sites/default/files/site/logo.svg"
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
  

  title = 'MonitorPlatform'
  private subscription: Subscription;
  platformurl: string ="";
  trackurl: string ="";
  constructor(private apiService: ApiService, private urlroute: ActivatedRoute) {
    this.subscription = interval(10000).pipe(
      switchMap(() => this.apiService.postData({
        topic: 'station/'
          + this.urlroute.snapshot.queryParamMap.get('platform') +
          /*'/' +
          this.urlroute.snapshot.queryParamMap.get('track') + */
          '/lcd'
         
      }))

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
        this.headsignSize = this.headsign == null?0:this.headsign.length;
        this.routeSize = this.route.length;
        this.endOfTheLine = response.departureTime == null ? true : false;

        switch (response.name.split(" ")[0]) {
          case "KM":
            this.icon = "https://www.mazowieckie.com.pl/sites/default/files/site/logo.svg";
            break;
          case "IC":
            this.icon = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d7/Logo_pkp_ic.svg/512px-Logo_pkp_ic.svg.png";
            break;
          case "EIC":
            this.icon = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d7/Logo_pkp_ic.svg/512px-Logo_pkp_ic.svg.png";
            break;
          case "EIP":
            this.icon = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d7/Logo_pkp_ic.svg/512px-Logo_pkp_ic.svg.png";
            break;
          case "IR":
            this.icon = "https://framerusercontent.com/images/OXiguSVS0CKZDYW1lTxlkVKGQZo.png";
            break;
          case "SKW":
            this.icon = "https://www.skm.warszawa.pl/wp-content/uploads/2020/10/SKM_logo_PNG.png";
            break;
          case "WKD":
            this.icon = "https://upload.wikimedia.org/wikipedia/commons/thumb/1/11/WKD.svg/2048px-WKD.svg.png";
            break;
          case "R":
            this.icon = "https://framerusercontent.com/images/OXiguSVS0CKZDYW1lTxlkVKGQZo.png";
            break;
          case "TLK":
            this.icon = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d7/Logo_pkp_ic.svg/512px-Logo_pkp_ic.svg.png";
            break;
          default:
            this.icon = "";
        }

      },
      error: (error) => {
        this.errorMessage = 'Błąd podczas pobierania danych.';
        console.error('Błąd:', error);
      },
    });
  }

  @Input('videoSrc') set setVideoSrc(value: string) {
    this.videoSrc = value
    this.videoplayer?.nativeElement.load();
    this.videoplayer?.nativeElement.play();
  }

  videoEnd() {
    this.videoSrc = 'http://localhost:4200/' + this.videoPlaylist[this.videoNum]
    this.videoNum++;
    if (this.videoNum == this.videoPlaylist.length) {
      this.apiService.getAdvertPlaylist().subscribe({
        next: (response) => {
        this.videoPlaylist=response
        this.videoNum = 0;
        //this.videoSrc = 'http://localhost:4200/logo-pkp_20250208182712-r20250208-1.webm';
      },
      error: (error) => {
        this.errorMessage = 'Błąd podczas pobierania danych.';

        console.error('Błąd:', error);
    },})
    }
    this.videoplayer?.nativeElement.load();
    this.videoplayer?.nativeElement.play();
  }

  ngOnDestroy(): void {
    // Unieważnienie subskrypcji przy niszczeniu komponentu
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
  ngOnInit() {
    this.apiService.getAdvertPlaylist().subscribe({
      next: (response) => {
      this.videoPlaylist=response
      this.videoNum = 0;
      //this.videoSrc = 'http://localhost:4200/logo-pkp_20250208182712-r20250208-1.webm';
    },
    error: (error) => {
      this.errorMessage = 'Błąd podczas pobierania danych.';

      console.error('Błąd:', error);
  },})
  }



}
