import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { GoogleMap, MapInfoWindow, MapMarker } from '@angular/google-maps';
import { MapPolygon } from '@angular/google-maps';
import { StavbeService } from '../../_services/stavbe.service';
import { Ipoligon, Itocka } from '../../_models/poligon';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-poligon',
  imports: [GoogleMap, MapPolygon, NgFor],
  templateUrl: './poligon.component.html',
  styleUrl: './poligon.component.css'
})
export class PoligonComponent implements OnInit {
  @ViewChild(GoogleMap, { static: false }) map!: GoogleMap;
  @ViewChild(MapInfoWindow, { static: false }) info!: MapInfoWindow;
  @ViewChild(MapMarker, { static: false }) marker!: MapMarker;

  clickedLat: number | null = null;
  clickedLng: number | null = null;


  stavbeService = inject(StavbeService);
  poligon?: Ipoligon;

  vsiPoligoni: Ipoligon[] = [];


  zoom!: number;
  center!: google.maps.LatLng;


  options: google.maps.MapOptions = {

    zoomControl: true,
    scrollwheel: true,
    disableDoubleClickZoom: true,
    mapTypeId: 'roadmap',
    // maxZoom: 25,
    // minZoom: 10,
  }
  polyOptions: google.maps.PolygonOptions = {
    strokeColor: '#FF0000',
    strokeOpacity: 0.9,
    strokeWeight: 1,
    //  fillColor: '#FF0000',
    fillColor: 'gray',
    fillOpacity: 0.35,
    visible: true,
    clickable: true
  };



  ngOnInit(): void {
    //this.center = new google.maps.LatLng({ lat: 46.22383, lng: 14.61013 }); // Kamnik 46,22383  14.61013
    while (this.vsiPoligoni.length) { this.vsiPoligoni.pop(); }

    this.loadGeoTocke();
  }

    onMapClick(event: any) {
    // For Google Maps JS API, event.latLng.lat() and event.latLng.lng()
    this.clickedLat = event.latLng.lat();
    this.clickedLng = event.latLng.lng();
  }

  // Add this helper function to transform Itocka[][] to LatLngLiteral[]
  transformToLatLngLiteral(paths: Itocka[][]): google.maps.LatLngLiteral[][] {
    return paths.map(path =>
      path.map(tocka => ({
        lat: tocka.lat, // Assuming Itocka has 'lat' property
        lng: tocka.lng  // Assuming Itocka has 'lng' property
      }))
    );
  }

  loadGeoTocke() {
    this.stavbeService.getGeoTockeStavbe(this.stavbeService.stavbaNaziv()).subscribe({
      next: (poligon) => {
        this.poligon = poligon;
        this.poligon!.noviObodiLatLng = this.transformToLatLngLiteral(poligon.noviObodiObjekta);
        this.center = poligon.center; // Assuming poligon has a center property of type google.maps.LatLng
        //      this.center = new google.maps.LatLng(poligon!.center.lat(), poligon!.center.lng());
        this.vsiPoligoni = [...this.vsiPoligoni, poligon];

      },
      error: (error) => {
        console.log(error);
      },
      complete: () => {
        console.log('GeoTocke loaded successfully!' + this.vsiPoligoni.length);
        this.vsiPoligoni.forEach((poligon) => {
          console.log('Poligon:', poligon.naziv, 'Tocke:', poligon.noviObodiLatLng);
        });

        var bounds = new google.maps.LatLngBounds();
        this.poligon?.noviObodiObjekta.forEach(tocke => {
          tocke.forEach(tocka => {
            bounds.extend(tocka)
          });
        });
        this.center = bounds.getCenter();
        this.zoom = 19;
        this.polyOptions.fillColor = 'gray';

      }
    });
  }



}
