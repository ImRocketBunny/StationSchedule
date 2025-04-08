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
import { MqttService, IMqttMessage } from 'ngx-mqtt';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  standalone: false,
  //imports: [MaterialModule, RouterOutlet, CommonModule],
})
export class AppComponent implements OnDestroy {

  videoSrc = 'http://localhost:4200/logo-pkp_20250208182712-r20250208-1.webm';
  videoPlaylist: string[] = []/*["PLK_wylamiane_rogatki_nowe-r20250123-7.webm", "POK_skm_CZARODZIEJSKI-FLET-DLA-DZIECI_03.2025-r20250205-9.webm",
  "Praca_SKM_elektryk_1920x810-r20250116-5.webm", "TS_Mahagonny_1920x810-r20241204-3.webm"
  , "4_UTK_animacja_BAGAZ_NEW-r20250113-5.webm","ZTM_Warszawa_mruga_9.02-r20250203-1.webm","POK_VENUS-AND-ADONIS_25.02-r20250115-3.webm"
,"TS_STARA-1920x810-r20241017-19.webm","ZTM_PLAKAT_E_HOLOGRAM_DLA_HB_TABOR_SKM_2-r20250117-9.webm","SKM_20lecie_1920x810-r20240510-15.webm" ]*/
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
  urlPlatform: string ='';

  message: string = 'Oczekiwanie na wiadomość...';
  //private subscription!: Subscription;
  

  title = 'MonitorPlatform'
  private subscription!: Subscription;
  private adSubscription!: Subscription;
  platformurl: string ="";
  trackurl: string ="";


  constructor(private mqttService: MqttService,private apiService: ApiService,private urlroute: ActivatedRoute) {
    
    
  }
  @Input('videoSrc') set setVideoSrc(value: string) {
    this.videoSrc = value
    this.videoplayer?.nativeElement.load();
    this.videoplayer?.nativeElement.play();
  }

  private subscribeAdSource(topic: string): void{
      this.adSubscription = this.mqttService.observe(topic).subscribe((message: IMqttMessage) =>{
          const payload = message.payload.toString();
          const data = JSON.parse(payload);
          this.videoPlaylist=data;
          console.log(this.videoPlaylist)
          this.videoNum=0;
        }
      );
  }
  private subscribeToTopic(topic: string): void {
    this.subscription = this.mqttService.observe(topic
         ).subscribe((message: IMqttMessage) => {
      try {
        const payloadStr = message.payload.toString();
        const data = JSON.parse(payloadStr);
        this.line = data.name == null ? "" : data.name.split("   ").length > 1 ? data.name.split("   ")[1] : data.name.split(" ")[0]
        this.courseId = data.name == null ? null : data.name.split("   ").length > 1 ? data.name.split("   ")[0] : data.name.split("   ")[0]
        this.delay = data.delay;
        this.headsign = data.headsignTo == "" ? data.headsignFrom : data.headsignTo;
        this.routeTo = data.routeTo == null ? [] : data.routeTo.replace(" • ", " -  ").split(" -  ")
        this.routeFrom = data.routeFrom == null ? [] : data.routeFrom.replace(" • ", " -  ").split(" -  ")
        this.departureTime = data.departureTime == null ? data.arrivalTime : data.departureTime;
        this.route = data.routeTo == "" ? this.routeFrom!.slice(1, -1).join(' - ') : this.routeTo!.slice(1, -1).join(' - ');
        this.responseData = data;
        this.errorMessage = null;
        this.headsignSize = this.headsign == null?0:this.headsign.length;
        this.routeSize = this.route.length;
        this.endOfTheLine = data.departureTime == null ? true : false;
        if(data.name != null)
        switch (data.name.split(" ")[0]) {
                  case "KM":
                    this.icon = "KM S.A.";
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
                    this.icon = "Polregio S.A.";
                    break;
                  case "SKW":
                    this.icon = "SKM S.A.";
                    break;
                  case "WKD":
                    this.icon = "https://upload.wikimedia.org/wikipedia/commons/thumb/1/11/WKD.svg/2048px-WKD.svg.png";
                    break;
                  case "R":
                    this.icon = "Polregio S.A.";
                    break;
                  case "TLK":
                    this.icon = "Intercity S.A.";
                    break;
                  default:
                    this.icon = "";
                }
      } catch (error) {
        console.error('Błąd parsowania JSON:', error);
      }
    });
  }

  videoEnd() {
    this.videoSrc = 'http://localhost:4200/' + this.videoPlaylist[this.videoNum]
    this.videoNum++;
    if (this.videoNum == this.videoPlaylist.length) {
      this.videoNum=0;
    }
    this.videoplayer?.nativeElement.load();
    this.videoplayer?.nativeElement.play();
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
      this.adSubscription.unsubscribe();
    }
  }
  ngOnInit() {
    this.urlroute.queryParams.subscribe(params => {
      this.urlPlatform = params['platform'];
      this.subscribeToTopic('station/'+this.urlPlatform+'/lcd');
      this.subscribeAdSource('station/'+this.urlPlatform.split('/')[0]+'/adPlaylist');
    })
    /*this.apiService.getAdvertPlaylist().subscribe({
      next: (response) => {
      this.videoPlaylist=response
      this.videoNum = 0;
      },
    error: (error) => {
      this.errorMessage = 'Błąd podczas pobierania danych.';

      console.error('Błąd:', error);
  },})*/
  }



}
