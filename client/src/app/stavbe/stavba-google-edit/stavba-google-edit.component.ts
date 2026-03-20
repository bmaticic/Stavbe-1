import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { GoogleMap, MapInfoWindow, MapMarker } from '@angular/google-maps';
import { MapPolygon } from '@angular/google-maps';
import { StavbeService } from '../../_services/stavbe.service';
import { Ipoligon, Itocka } from '../../_models/poligon';
import { NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-stavba-google-edit',
  imports: [GoogleMap, MapPolygon, MapMarker, NgFor, NgIf],
  templateUrl: './stavba-google-edit.component.html',
  styleUrl: './stavba-google-edit.component.css'
})
export class StavbaGoogleEditComponent implements OnInit {
  @ViewChild(GoogleMap, { static: false }) map!: GoogleMap;
  @ViewChild(MapInfoWindow, { static: false }) info!: MapInfoWindow;
  @ViewChild(MapMarker, { static: false }) marker!: MapMarker;

  clickedLat: number | null = null;
  clickedLng: number | null = null;


  stavbeService = inject(StavbeService);
  poligon?: Ipoligon;

  vsiPoligoni: Ipoligon[] = [];

  selectedPolygonIndex: number | null = null;
  selectedVertex: { polygonIndex: number; vertexIndex: number } | null = null;
  addingPolygon = false;
  newPolygonPoints: google.maps.LatLngLiteral[] = [];

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
    fillColor: 'gray',
    fillOpacity: 0.35,
    visible: true,
    clickable: true
  };

  selectedPolyOptions: google.maps.PolygonOptions = {
    strokeColor: '#0000FF',
    strokeOpacity: 1,
    strokeWeight: 2,
    fillColor: '#0000FF',
    fillOpacity: 0.35,
    visible: true,
    clickable: true
  };

  newPolyOptions: google.maps.PolygonOptions = {
    strokeColor: '#00FF00',
    strokeOpacity: 0.9,
    strokeWeight: 2,
    fillColor: '#00FF00',
    fillOpacity: 0.15,
    visible: true,
    clickable: false
  };

  vertexMarkerOptions: google.maps.MarkerOptions = {
    icon: {
      path: google.maps.SymbolPath.CIRCLE,
      scale: 6,
      fillColor: '#FF0000',
      fillOpacity: 1,
      strokeColor: '#FFFFFF',
      strokeWeight: 2,
    },
    clickable: true,
    cursor: 'pointer',
    draggable: true
  };

  selectedVertexMarkerOptions: google.maps.MarkerOptions = {
    icon: {
      path: google.maps.SymbolPath.CIRCLE,
      scale: 8,
      fillColor: '#0000FF',
      fillOpacity: 1,
      strokeColor: '#FFFFFF',
      strokeWeight: 2,
    },
    clickable: true,
    cursor: 'pointer',
    draggable: true
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

    if (this.addingPolygon && this.clickedLat !== null && this.clickedLng !== null) {
      this.newPolygonPoints.push({
        lat: this.clickedLat,
        lng: this.clickedLng
      });
    }
  }

  startAddPolygon() {
    this.addingPolygon = true;
    this.newPolygonPoints = [];
    this.selectedPolygonIndex = null;
    this.selectedVertex = null;
  }

  finishAddPolygon() {
    if (this.newPolygonPoints.length < 3) {
      alert('Polygon must have at least 3 points');
      return;
    }

    if (!this.poligon) {
      alert('No polygon data loaded yet');
      return;
    }

    if (!this.poligon.noviObodiLatLng) {
      this.poligon.noviObodiLatLng = [];
    }

    this.poligon.noviObodiLatLng.push([...this.newPolygonPoints]);
    this.addingPolygon = false;
    this.newPolygonPoints = [];
  }

  cancelAddPolygon() {
    this.addingPolygon = false;
    this.newPolygonPoints = [];
  }

  selectPolygon(index: number) {
    this.selectedPolygonIndex = index;
    this.selectedVertex = null;
  }

  deleteSelectedPolygon() {
    if (this.poligon?.noviObodiLatLng && this.selectedPolygonIndex !== null) {
      this.poligon.noviObodiLatLng.splice(this.selectedPolygonIndex, 1);
      if (this.poligon.noviObodiLatLng.length === 0) {
        this.selectedPolygonIndex = null;
      } else if (this.selectedPolygonIndex >= this.poligon.noviObodiLatLng.length) {
        this.selectedPolygonIndex = this.poligon.noviObodiLatLng.length - 1;
      }
    }
  }

  getPolygonOptions(index: number): google.maps.PolygonOptions {
    return (this.selectedPolygonIndex === index) ? this.selectedPolyOptions : this.polyOptions;
  }

  selectVertex(polygonIndex: number, vertexIndex: number) {
    this.selectedVertex = { polygonIndex, vertexIndex };
    this.selectedPolygonIndex = polygonIndex;
  }

  getVertexMarkerOptions(polygonIndex: number, vertexIndex: number): google.maps.MarkerOptions {
    const isSelected = this.selectedVertex?.polygonIndex === polygonIndex && this.selectedVertex?.vertexIndex === vertexIndex;
    return isSelected ? this.selectedVertexMarkerOptions : this.vertexMarkerOptions;
  }

  onVertexMouseOver(polygonIndex: number, vertexIndex: number) {
    // Change cursor to pointer when hovering over vertex
    if (this.map) {
      this.map.googleMap?.setOptions({ draggableCursor: 'pointer' });
    }
  }

  onVertexMouseOut() {
    // Reset cursor when not hovering over vertex
    if (this.map) {
      this.map.googleMap?.setOptions({ draggableCursor: '' });
    }
  }

  onVertexDragEnd(polygonIndex: number, vertexIndex: number, event: any) {
    if (this.poligon?.noviObodiLatLng && event.latLng) {
      this.poligon.noviObodiLatLng[polygonIndex][vertexIndex] = {
        lat: event.latLng.lat(),
        lng: event.latLng.lng()
      };
    }
  }
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
