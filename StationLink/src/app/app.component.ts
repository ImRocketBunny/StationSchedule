import { Component } from '@angular/core';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { Train } from '../models/train';
import { TrainService } from '../services/train.service';
import { GtfsTrainService } from '../services/gtfstrain.service';
import * as L from 'leaflet';
import { GtfsTrain } from '../models/gtfstrain';
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

  constructor(private trainService: TrainService, private gtfsTraionService: GtfsTrainService) { }

  ngOnInit(): void {
    //this.trainService.getTrains().subscribe((result: Train[]) => this.trains = result);
    
  }


  private fetchMarkers(): void {


    this.gtfsTraionService.getKMTrains().subscribe((result: GtfsTrain[])=>{
        const newIds = new Set(result.map(marker => marker.course_id)); 

        result.forEach(markerData =>{
          if(this.markers.has(markerData.course_id)){
            this.updateMarker(markerData.course_id,markerData.position.shape_pt_lat,markerData.position.shape_pt_lon,markerData.trip_headsign)
          }else{
            this.addMarker(markerData.course_id,markerData.position.shape_pt_lat,markerData.position.shape_pt_lon,markerData.trip_headsign)

          }
        })

        this.markers.forEach((marker, id) => {
          if (!newIds.has(id)) {
            this.map.removeLayer(marker);
            this.markers.delete(id);
          }
        });

      }

        
    )

    
  }

  private startAutoUpdate(): void {
    this.intervalId = setInterval(() => {
      this.fetchMarkers();
    }, 10000); // 🔄 Odświeżanie co 10 sekund
  }

  private addMarker(id: string, lat: number, lng: number,headsign: string): void {

    const customIcon = L.icon({
      iconUrl: 'train-subway-KM.svg',
      iconSize: [48, 48],
      iconAnchor: [16, 32],
      popupAnchor: [0, -32]
    });


    const marker = L.marker([lat, lng],{ icon: customIcon })
      .addTo(this.map)
      .bindTooltip(id+' -> '+headsign, { permanent: true, direction: "bottom" })

    this.markers.set(id, marker);
  }

  private updateMarker(id: string, lat: number, lng: number,headsign: string): void {
    
    const marker = this.markers.get(id);
    if (marker) {
      marker.setLatLng([lat, lng])
        //.bindPopup(`ID: ${id}, Nowa pozycja: ${lat.toFixed(5)}, ${lng.toFixed(5)}`);
    }
  }

  private initMap(): void {
    this.map = L.map('map').setView([52.2298, 21.0122], 13); // Warszawa
    /*L.tileLayer('http://{a-c}.tiles.openrailwaymap.org/standard/{z}/{x}/{y}.png', {
      attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(this.map);*/
    /*var openrailwaymap = new L.TileLayer('http://{s}.tiles.openrailwaymap.org/standard/{z}/{x}/{y}.png',
      {
        attribution: '<a href="https://www.openstreetmap.org/copyright">© OpenStreetMap contributors</a>, Style: <a href="http://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA 2.0</a> <a href="http://www.openrailwaymap.org/">OpenRailwayMap</a> and OpenStreetMap',
        minZoom: 2,
        maxZoom: 19,
        tileSize: 256
      }).addTo(this.map);*/

      const osm = L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
      }).addTo(this.map);
    
      // Warstwa kolejowa OpenRailMap
      const openRailMap = L.tileLayer('https://{s}.tiles.openrailwaymap.org/standard/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openrailwaymap.org/">OpenRailwayMap</a> contributors',
        maxZoom: 19
      }).addTo(this.map);
    

  //this.fetchMarkers()
  }

  /*private getRailway(){
    fetch('https://overpass-api.de/api/interpreter?data=[out:json];way["railway"](bbox);out geom;')
  .then(response => response.json())
  .then(data => {
    this.railways = this.extractRailwayLines(data);
  });
  }*/

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