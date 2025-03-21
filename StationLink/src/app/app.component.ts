import { Component } from '@angular/core';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { Train } from '../models/train';
import { TrainService } from '../services/train.service';
import * as L from 'leaflet';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class AppComponent {
  private markers: Map<string, L.Marker> = new Map(); // Przechowuje markery z unikalnym ID
  private intervalId: any;
  title = 'StationHubNG';
  trains: Train[] = []

  private map!: L.Map;

  constructor(private trainService: TrainService) { }

  ngOnInit(): void {
    this.trainService.getTrains().subscribe((result: Train[]) => this.trains = result);
    
  }


  private fetchMarkers(): void {
    this.trainService.getTrains().subscribe((result: Train[]) =>
    result.forEach(markerData => {
      if (this.markers.has(markerData.Name)) {
        this.updateMarker(markerData.Name, markerData.Latitude, markerData.Longitude)
      } else {
        this.addMarker(markerData.Name, markerData.Latitude, markerData.Longitude);
      }
    })

    );
      console.log(this.trains)
      // 1. Aktualizacja i dodawanie markerów
      /*this.trains.forEach(markerData => {
        if (this.markers.has(markerData.Name)) {
          this.updateMarker(markerData.Name, markerData.Latitude, markerData.Longitude)
        } else {
          this.addMarker(markerData.Name, markerData.Latitude, markerData.Longitude);
        }
      });*/

      // 2. Usuwanie markerów, które nie były w najnowszej odpowiedzi API
      /*this.markers.forEach((marker, id) => {
        if (!newIds.has(id)) {
          this.map.removeLayer(marker);
          this.markers.delete(id);
        }
      });*/
    
  }

  private startAutoUpdate(): void {
    this.intervalId = setInterval(() => {
      this.fetchMarkers();
    }, 10000); // 🔄 Odświeżanie co 10 sekund
  }

  private addMarker(id: string, lat: number, lng: number): void {

    const customIcon = L.icon({
      iconUrl: '../assets/front-of-bus.png',
      iconSize: [32, 32],
      iconAnchor: [16, 32],
      popupAnchor: [0, -32]
    });


    const marker = L.marker([lat, lng],{ icon: customIcon })
      .addTo(this.map)
      .bindTooltip(id, { permanent: true, direction: "bottom" })

    this.markers.set(id, marker);
  }

  private updateMarker(id: string, lat: number, lng: number): void {
    const marker = this.markers.get(id);
    if (marker) {
      marker.setLatLng([lat, lng])
        .bindPopup(`ID: ${id}, Nowa pozycja: ${lat.toFixed(5)}, ${lng.toFixed(5)}`);
    }
  }

  private initMap(): void {
    this.map = L.map('map').setView([52.2298, 21.0122], 13); // Warszawa

    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(this.map);

  //this.fetchMarkers()
  }

  ngAfterViewInit(): void {
    this.initMap();
    this.fetchMarkers();
    this.startAutoUpdate();
  }




}


/*
var map = L.map('map').setView([51.505, -0.09], 13);

L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
}).addTo(map);

L.marker([51.5, -0.09]).addTo(map)
    .bindPopup('A pretty CSS popup.<br> Easily customizable.')
    .openPopup();*/