import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

declare const google: any; 

@Component({
  selector: 'app-gmapviewer',
  templateUrl: './gmapviewer.component.html',
  styleUrls: ['./gmapviewer.component.css'],standalone: false
})
export class GmapviewerComponent implements OnInit {

  map: any;
  polygons: any = {};
  markers: any[] = [];
  isAdmin: boolean = true; 
  
  constructor(
    private dialogRef: MatDialogRef<GmapviewerComponent>, // Inject MatDialogRef to control dialog
    private notificationsService: NotificationsService,
    private formBuilder: FormBuilder,
    private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) {}

  ngOnInit(): void {
    this.initMap();
  }

  initMap(): void {
    this.map = new google.maps.Map(document.getElementById("map"), {
      center: { lat: 31.51, lng: 74.36 }, // Initial map center Set Lahore
      zoom: 11, // Initial zoom level
      mapTypeControl: false
    });

    // Load zones from JSON file (mocked for now)
    this.loadZones();

    // Allow drawing of polygons and placing markers if the user is an admin
    if (this.isAdmin) {
      this.enableDrawingManager();
    }
  }

  

  loadZones(): void {
    const zones = [
      {
        zoneName: "Zone 1",
        color: "#FF0000",
        coordinates: [
          {lat: 31.50369, lng: 74.31217},
          {lat: 31.50033, lng: 74.31758},
          {lat: 31.50391, lng: 74.33183},
          {lat: 31.47478, lng: 74.34384},
          {lat: 31.46322, lng: 74.31775},
          {lat: 31.46981, lng: 74.30316},
          {lat: 31.47976, lng: 74.30041},
          {lat: 31.48664, lng: 74.29286},
        ],
        salesmen: [
          { name: "DSF A", lastLocation: { lat: 31.48, lng: 74.30 } },
          { name: "DSF B", lastLocation: { lat: 31.49, lng: 74.31 } }
        ]
      },
      // Add more zones as needed
    ];
    zones.forEach(zone => {
      const polygon = new google.maps.Polygon({
        paths: zone.coordinates.map(coord => new google.maps.LatLng(coord.lat, coord.lng)),
        strokeColor: zone.color,
        strokeOpacity: 1,
        strokeWeight: 2,
        fillColor: zone.color,
        fillOpacity: 0, // Transparent inside
      });
      polygon.setMap(this.map);
      this.polygons[zone.zoneName] = polygon;

      // Place markers for salesmen in this zone
      zone.salesmen.forEach(dsf => {
        const marker = new google.maps.Marker({
          position: new google.maps.LatLng(dsf.lastLocation.lat, dsf.lastLocation.lng),
          map: this.map,
          title: dsf.name
        });
        this.markers.push(marker);
      });
    });
  }

  enableDrawingManager(): void {
    const drawingManager = new google.maps.drawing.DrawingManager({
      drawingMode: google.maps.drawing.OverlayType.POLYGON,
      drawingControl: true,
      drawingControlOptions: {
        position: google.maps.ControlPosition.TOP_CENTER,
        drawingModes: ['polygon', 'marker'] // Allow drawing of polygons and placing markers
      },
      polygonOptions: {
        fillColor: '#FF0000',
        fillOpacity: 0.35,
        strokeColor: '#FF0000',
        strokeOpacity: 0.8,
        strokeWeight: 2,
        clickable: false,
        editable: true,
        zIndex: 1
      },
      markerOptions: {
        draggable: true // Allow marker to be draggable
      }
    });
    drawingManager.setMap(this.map);

    google.maps.event.addListener(drawingManager, 'polygoncomplete', (polygon: any) => {
      const vertices = polygon.getPath();
      const polygonCoordinates: { lat: number, lng: number }[] = [];
    
      vertices.forEach((vertex: any) => {
        const latLng = vertex.toJSON();
        polygonCoordinates.push({
          lat: latLng.lat.toFixed(5),
          lng: latLng.lng.toFixed(5)
        });
      });
    
      // Now, polygonCoordinates array has the coordinates in the accepted format
      this.displayCoordinates(polygonCoordinates);
    });

    google.maps.event.addListener(drawingManager, 'markercomplete', (marker: any) => {
      this.markers.push(marker); // Store the marker

      const latLng = marker.getPosition();
      let coordinatesDisplay = `<strong>Marker Coordinates:</strong> (${latLng.lat().toFixed(5)}, ${latLng.lng().toFixed(5)})<br>`;
      // this.displayCoordinates(coordinatesDisplay);
    });
  }

  displayCoordinates(coordinates: { lat: number, lng: number }[]): void {
    const coordinatesElement = document.getElementById('coordinates');
    if (coordinatesElement) {
      coordinatesElement.innerHTML = '<strong>Polygon Coordinates:</strong><br>';
      coordinates.forEach(coord => {
        coordinatesElement.innerHTML += `{lat: ${coord.lat}, lng: ${coord.lng}},<br>`;
      });
    }
  }


  SaveData() {
    const resultData = {
      message: 'Data saved successfully',
    };

    this.dialogRef.close(resultData);
  }


}
