import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

declare const google: any;

@Component({
  selector: 'app-draw-map',
  templateUrl: './draw-map.component.html',
  styleUrls: ['./draw-map.component.css'],standalone: false
})

export class DrawMapComponent implements OnInit {
  map: any;
  polygons: any = {};
  territoryPolygon: any = {};
  markers: any[] = [];
  isAdmin: boolean = true;
  drawCoordinates: any;
  selectedPin: any;
  currentPolygon: any;
  currentMarker: any;
  infoWindow: any;
  isDrawingMode: boolean = false; // Flag to track drawing mode
  isExpandPolygon: boolean = false;

  constructor(
    private dialogRef: MatDialogRef<DrawMapComponent>,
    private notificationsService: NotificationsService,
    private formBuilder: FormBuilder,
    private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    if (this.data.element?.drawingPolygon == true || this.data.element?.drawingMarker) {
      this.isDrawingMode = true;
    }
    if (this.data.element?.isExpandPolygon == true) {
      this.isExpandPolygon = true;
    }
    this.initMap();
  }

  initMap(): void {
    this.map = new google.maps.Map(document.getElementById("map"), {
      center: { lat: 30.3753, lng: 69.3451 }, // Coordinates for Pakistan's center
      zoom: 6,
      mapTypeControl: false
    });

    this.infoWindow = new google.maps.InfoWindow();

    if (this.isAdmin) {
      this.enableDrawingManager();
    }

    if (this.data.element?.coordinates?.length > 0) {
      this.data.element.coordinates.forEach((coordinateSet: any) => {
        this.territoryPolygon = null;
        const coordinates = JSON.parse(coordinateSet.coordinates);
        const path = coordinates.map((coord: any) => new google.maps.LatLng(coord.lat, coord.lng));

        var polygonDrawProperty = this.constantService.getPolygonDrawProperty(coordinateSet.typeId);

        const polygon = new google.maps.Polygon({
          paths: path,
          strokeColor: polygonDrawProperty.borderColor,
          strokeOpacity: polygonDrawProperty.borderOpacity,
          strokeWeight: polygonDrawProperty.borderWidth,
          fillColor: polygonDrawProperty.fillColor,
          fillOpacity: polygonDrawProperty.fillOpacity,
          editable: this.isExpandPolygon,
        });

        polygon.setMap(this.map);

        if (this.isExpandPolygon) {
          this.updatePolygonCoordinates(polygon); // Initialize coordinates for the editable polygon
          this.addPolygonEditListeners(polygon);  // Add listeners for polygon editing
        }
        
        if (this.data.element?.isShowInfoBox) {
          polygon.name = coordinateSet.name;

          google.maps.event.addListener(polygon, 'mouseover', (event: any) => {
            if (!this.isDrawingMode) {  // Show info box only when not in drawing mode
              this.infoWindow.setContent(polygon.name);
              this.infoWindow.setPosition(event.latLng);
              this.infoWindow.open(this.map);
            }
          });

          google.maps.event.addListener(polygon, 'mouseout', () => {
            this.infoWindow.close();
          });
        }

        if (coordinateSet.typeId == 2) {
          this.territoryPolygon = polygon;
        }

        if (this.data.element?.isFocusDrawPolygon) {
          const bounds = new google.maps.LatLngBounds();
          path.forEach((latLng: any) => bounds.extend(latLng));
          this.map.fitBounds(bounds);

          google.maps.event.addListenerOnce(this.map, 'bounds_changed', () => {
            const zoom = this.map.getZoom();
            this.map.setZoom(zoom - 1);
          });
        }
      });
    }

    if (this.data.element?.markerPins?.length > 0) {
      const bounds = new google.maps.LatLngBounds();

      this.data.element.markerPins.forEach((markerPinsSet: any) => {
        var pinLocationDrawProperty = this.constantService.getPinLocationDrawProperty(markerPinsSet.typeId);

        const icon = {
          url: pinLocationDrawProperty.iconFilePath,
          scaledSize: new google.maps.Size(50, 50),
          anchor: new google.maps.Point(22, 50),
        };

        const markerPin = JSON.parse(markerPinsSet.pinLocation);
        const marker = new google.maps.Marker({
          position: new google.maps.LatLng(markerPin.lat, markerPin.lng),
          map: this.map
        });

        const contentString = `
        <div style="">
          <h3 style="
            margin: 0;
            font-size: 16px;
            font-weight: bold;
            padding-bottom: 5px; /* Add spacing below the heading */
            border-bottom: 1px solid #ddd; /* Optional: add a border for visual separation */
          ">${markerPinsSet.name}</h3>
          <p style="
            margin: 0;
            padding: 5px 0; /* Adjust padding to control spacing */
          ">Address: ${markerPinsSet.address}</p>
          <p style="
            margin: 0;
            padding: 5px 0; /* Adjust padding to control spacing */
          ">Phone No: ${markerPinsSet.phoneNo}</p>
        </div>`;

        const infoWindowPin = new google.maps.InfoWindow({
          content: contentString,
        });

        marker.name = markerPinsSet.name;

        if (this.data.element?.isShowInfoBox) {
          google.maps.event.addListener(marker, 'click', (event: any) => {
            if (!this.isDrawingMode) {  // Show info box only when not in drawing mode
              // infoWindowPin.setContent(marker.name);
              // infoWindowPin.setPosition(marker.getPosition());
              // infoWindowPin.open(this.map);
              infoWindowPin.open(this.map, marker);
            }
          });
        }

        bounds.extend(marker.getPosition());
      });

      this.map.fitBounds(bounds);

      if (this.data.element?.isFocusDrawMarker) {
        google.maps.event.addListenerOnce(this.map, 'bounds_changed', () => {
          const zoom = this.map.getZoom();
          this.map.setZoom(zoom - 5);
        });
      }
    }
  }
  updatePolygonCoordinates(polygon: any): void {
    const vertices = polygon.getPath();
    const polygonCoordinates: { lat: number, lng: number }[] = [];
  
    vertices.forEach((vertex: any) => {
      const latLng = vertex.toJSON();
      polygonCoordinates.push({
        lat: parseFloat(latLng.lat.toFixed(5)),
        lng: parseFloat(latLng.lng.toFixed(5)),
      });
    });
  
    // Convert coordinates to JSON and update this.drawCoordinates
    this.drawCoordinates = JSON.stringify(polygonCoordinates);
  }
  
  addPolygonEditListeners(polygon: any): void {
    const vertices = polygon.getPath();
    
    // Listen for changes to the polygon's path
    google.maps.event.addListener(vertices, 'set_at', () => {
      this.updatePolygonCoordinates(polygon); // Update coordinates on vertex move
    });
  
    google.maps.event.addListener(vertices, 'insert_at', () => {
      this.updatePolygonCoordinates(polygon); // Update coordinates on vertex addition
    });
  
    google.maps.event.addListener(vertices, 'remove_at', () => {
      this.updatePolygonCoordinates(polygon); // Update coordinates on vertex removal
    });
  }
  
  enableDrawingManager(): void {
    const drawingModes = [];

    if (this.data.element?.drawingPolygon) {
      drawingModes.push(google.maps.drawing.OverlayType.POLYGON);
    }

    if (this.data.element?.drawingMarker) {
      drawingModes.push(google.maps.drawing.OverlayType.MARKER);
    }

    const polygonDrawProperty = this.constantService.getPolygonDrawProperty(this.data.element?.typeId);

    const drawingManager = new google.maps.drawing.DrawingManager({
      drawingMode: drawingModes.length > 0 ? drawingModes[0] : null,
      drawingControl: true,
      drawingControlOptions: {
        position: google.maps.ControlPosition.TOP_CENTER,
        drawingModes: drawingModes
      },

      polygonOptions: {
        fillColor: polygonDrawProperty.fillColor,
        fillOpacity: polygonDrawProperty.fillOpacity,
        strokeColor: polygonDrawProperty.borderColor,
        strokeOpacity: polygonDrawProperty.borderOpacity,
        strokeWeight: polygonDrawProperty.borderWidth,
        clickable: false,
        editable: true,
        zIndex: 1
      },
      markerOptions: {
        draggable: true
      }
    });

    drawingManager.setMap(this.map);

    // Set drawing mode flag when in drawing mode
    google.maps.event.addListener(drawingManager, 'drawingmode_changed', () => {
      this.isDrawingMode = !!drawingManager.getDrawingMode();
    });

    google.maps.event.addListener(drawingManager, 'polygoncomplete', (polygon: any) => {
      this.drawCoordinates = null;
      if (this.currentPolygon) {
        this.currentPolygon.setMap(null);
      }
      this.currentPolygon = polygon;
      const vertices = polygon.getPath();
      const polygonCoordinates: { lat: number, lng: number }[] = [];

      vertices.forEach((vertex: any) => {
        const latLng = vertex.toJSON();
        polygonCoordinates.push({
          lat: parseFloat(latLng.lat.toFixed(5)),
          lng: parseFloat(latLng.lng.toFixed(5))
        });
      });

      const jsonString = JSON.stringify(polygonCoordinates);
      this.drawCoordinates = jsonString;
    });

    google.maps.event.addListener(drawingManager, 'markercomplete', (marker: any) => {
      this.selectedPin = null;
      if (this.currentMarker) {
        this.currentMarker.setMap(null);
      }
      this.currentMarker = marker;

      this.markers.push(marker);

      const latLng = marker.getPosition();
      let pinString = JSON.stringify({ lat: latLng.lat(), lng: latLng.lng() });

      if (google.maps.geometry.poly.containsLocation(latLng, this.territoryPolygon)) {
        this.markers.push(marker);
        this.selectedPin = pinString;
      } else {
        this.notificationsService.showNotification('Please mark the location inside the draw area!', 'snack-bar-warning');
        marker.setMap(null);
      }

      this.selectedPin = pinString;
    });
  }

  SaveData() {
    if (this.data.element?.drawingPolygon == true && this.drawCoordinates == null) {
      this.notificationsService.showNotification('Error! Please draw map first!', 'snack-bar-danger');
    }
    else if (this.data.element?.drawingMarker == true && this.selectedPin == null) {
      this.notificationsService.showNotification('Error! Please select the pin location first!', 'snack-bar-danger');
    }
    else {
      if (this.data.element?.drawingPolygon) {
        this.dialogRef.close(this.drawCoordinates);
      }
      else if (this.data.element?.isExpandPolygon) {
        this.dialogRef.close(this.drawCoordinates);
      }
      else if (this.data.element?.drawingMarker) {
        this.dialogRef.close(this.selectedPin);
      }
    }
  }
}
