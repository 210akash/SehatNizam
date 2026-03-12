import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { RouteService } from '../../route/route.service';

declare const google: any;

@Component({
  selector: 'app-draw-route-shops',
  templateUrl: './draw-route-shops.component.html',
  styleUrls: ['./draw-route-shops.component.css'],standalone: false
})

export class DrawRouteShopsComponent implements OnInit {

  map: any;
  selectedMarkers: any[] = [];
  routePolylines: google.maps.Polyline[] = [];
  polygons: any = {};
  markers: any[] = [];
  isAdmin: boolean = true;
  drawCoordinates: any;
  selectedPin: any;
  currentPolygon: any; // To keep track of the currently drawn polygon
  currentMarker: any;

  constructor(
    private routeService: RouteService,
    private dialogRef: MatDialogRef<DrawRouteShopsComponent>, // Inject MatDialogRef to control dialog
    private notificationsService: NotificationsService,
    private formBuilder: FormBuilder,
    private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.initMap();
  }

  initMap(): void {
    this.map = new google.maps.Map(document.getElementById("map"), {
      center: { lat: 31.51, lng: 74.36 }, // Initial map center Set Lahore
      zoom: 11, // Initial zoom level
      mapTypeControl: false
    });

    // Allow drawing of polygons and placing markers if the user is an admin
    if (this.isAdmin) {
      this.enableDrawingManager();
    }

    if (this.data.element?.selectedMarkers && this.data.element.selectedMarkers.length > 0) {
      this.selectedMarkers = this.data.element.selectedMarkers.map((markerPinsSet: any, index: number) => ({
        ...markerPinsSet,
        sequence: index + 1 // Set the sequence for the markers
      }));
      this.drawRoutes();
    }

    if (this.data.element?.coordinates && this.data.element?.coordinates.length > 0) {
      // Iterate over each set of coordinates in the list
      this.data.element.coordinates.forEach((coordinateSet: any) => {
        // Parse the JSON string into an array of coordinate objects
        const coordinates = JSON.parse(coordinateSet.coordinates);

        // Convert the array into a format suitable for Google Maps
        const path = coordinates.map((coord: any) => new google.maps.LatLng(coord.lat, coord.lng));

        var polygonDrawProperty = this.constantService.getPolygonDrawProperty(coordinateSet.typeId)

        // Create the polygon with border only
        const polygon = new google.maps.Polygon({
          paths: path,
          strokeColor: polygonDrawProperty.borderColor, // Border color
          strokeOpacity: polygonDrawProperty.borderOpacity, // Border opacity
          strokeWeight: polygonDrawProperty.borderWidth, // Border width
          fillColor: polygonDrawProperty.fillColor, // Fill color (not visible due to fillOpacity being 0)
          fillOpacity: polygonDrawProperty.fillOpacity // Make the fill transparent
        });

        // Add the polygon to the map
        polygon.setMap(this.map);

        // Create a LatLngBounds object
        const bounds = new google.maps.LatLngBounds();

        // Extend the bounds with each coordinate
        path.forEach((latLng: any) => bounds.extend(latLng));

        // Adjust the map's viewport to fit the polygon
        this.map.fitBounds(bounds);
      });
    }


    if (this.data.element?.markerPins && this.data.element?.markerPins.length > 0) {
      const bounds = new google.maps.LatLngBounds();

      this.data.element.markerPins.forEach((markerPinsSet: any) => {
        const markerPin = JSON.parse(markerPinsSet.pinLocation);
        const marker = new google.maps.Marker({
          position: new google.maps.LatLng(markerPin.lat, markerPin.lng),
          map: this.map,
          icon: this.getMarkerIcon(markerPinsSet.id), // Initial icon based on selection state
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

        const infoWindow = new google.maps.InfoWindow({
          content: contentString,
        });

        marker.addListener('mouseover', () => {
          infoWindow.open(this.map, marker);
        });

        marker.addListener('mouseout', () => {
          infoWindow.close();
        });

        // Handle marker click to select/unselect
        marker.addListener('click', () => {
          if (this.data.element?.isViewOnly == true) {
            // If view-only mode is enabled, prevent interaction with the marker
            return;
          }
          this.toggleMarkerSelection(markerPinsSet, marker); // Pass the entire markerPinsSet object
        });

      });

    }

  }
  // Toggle marker selection

  async toggleMarkerSelection(markerPinsSet: any, marker: google.maps.Marker): Promise<void> {

    (await this.routeService.isShopOccupied(markerPinsSet.id, this.data.element.routeId)).subscribe(
      {
        next: (data) => {
          if (data.isShopOccupied == true) {
            this.notificationsService.showNotification('This Shop already attach with route: ' + data.routesInformation[0].name, 'snack-bar-danger');
          }
          else {
            const existingMarkerIndex = this.selectedMarkers.findIndex(m => m.id === markerPinsSet.id);

            if (existingMarkerIndex !== -1) {
              // Marker is already selected, so remove it from the selectedMarkers array
              this.selectedMarkers.splice(existingMarkerIndex, 1);
              marker.setIcon(this.getDefaultIcon());

              // Reorder the remaining markers
              this.selectedMarkers.forEach((m, index) => {
                m.sequence = index + 1; // Reset the order after removal
              });

            } else {
              // Marker is not selected, so add it to the selectedMarkers array
              const newOrder = this.selectedMarkers.length + 1;
              this.selectedMarkers.push({
                ...markerPinsSet,
                sequence: newOrder, // Set the order for the new selection
              });
              marker.setIcon(this.getSelectedIcon());
            }

            // Reorder and redraw routes
            this.sortedMarkers();
          }

          // this.dataSource = data;
          // this.updateCheckedStatus();
        },
        error: (error) => {
          console.log(error);
        }
      });
  }


  drawRoutes(): void {
    // Clear existing routes
    this.clearRoutes();

    if (this.selectedMarkers.length < 2) return; // Need at least two markers to draw a route

    for (let i = 0; i < this.selectedMarkers.length - 1; i++) {
      const startMarker = this.selectedMarkers[i];
      const endMarker = this.selectedMarkers[i + 1];

      const startPos = JSON.parse(startMarker.pinLocation);
      const endPos = JSON.parse(endMarker.pinLocation);

      const polyline = new google.maps.Polyline({
        path: [
          new google.maps.LatLng(startPos.lat, startPos.lng),
          new google.maps.LatLng(endPos.lat, endPos.lng)
        ],
        strokeColor: '#FF0000',
        strokeOpacity: 1.0,
        strokeWeight: 2,
        map: this.map
      });

      this.routePolylines.push(polyline);
    }
  }

  clearRoutes(): void {
    this.routePolylines.forEach(polyline => polyline.setMap(null));
    this.routePolylines = [];
  }

  getMarkerIcon(markerId: number): string {
    // Check if the marker with the given ID is in the selectedMarkers array
    const isSelected = this.selectedMarkers.some(marker => marker.id === markerId);
    return isSelected ? this.getSelectedIcon() : this.getDefaultIcon();
  }


  // Define the icon for the selected marker
  getSelectedIcon(): string {
    return 'https://maps.gstatic.com/mapfiles/ms2/micons/green-dot.png'; // Change to desired color/icon
  }

  // Define the icon for the unselected marker
  getDefaultIcon(): string {
    return 'https://maps.gstatic.com/mapfiles/ms2/micons/red-dot.png'; // Default marker color/icon
  }
  sortedMarkers() {
    this.selectedMarkers = this.selectedMarkers.sort((a, b) => a.sequence - b.sequence);

    // Redraw routes between markers
    this.drawRoutes();
  }
  enableDrawingManager(): void {
    // Initialize drawingModes array
    const drawingModes = [];

    // Check and add drawingPolygon mode if it is defined and true
    if (this.data.element?.drawingPolygon) {
      drawingModes.push(google.maps.drawing.OverlayType.POLYGON);
    }

    // Check and add drawingMarker mode if it is defined and true
    if (this.data.element?.drawingMarker) {
      drawingModes.push(google.maps.drawing.OverlayType.MARKER);
    }
    var polygonDrawProperty = this.constantService.getPolygonDrawProperty(this.data.element?.typeId)

    const drawingManager = new google.maps.drawing.DrawingManager({
      drawingMode: drawingModes.length > 0 ? drawingModes[0] : null, // Set the default drawing mode if any are allowed
      drawingControl: true,
      drawingControlOptions: {
        position: google.maps.ControlPosition.TOP_CENTER,
        drawingModes: drawingModes // Allow drawing of polygons and placing markers
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
        draggable: true // Allow marker to be draggable
      }
    });

    drawingManager.setMap(this.map);


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

      // Convert the polygonCoordinates array to a JSON string
      const jsonString = JSON.stringify(polygonCoordinates);
      this.drawCoordinates = jsonString;
    });

    google.maps.event.addListener(drawingManager, 'markercomplete', (marker: any) => {
      this.selectedPin = null;
      if (this.currentMarker) {
        this.currentMarker.setMap(null);
      }
      this.currentMarker = marker;

      this.markers.push(marker); // Store the marker

      const latLng = marker.getPosition();
      let pinString = JSON.stringify({ lat: latLng.lat(), lng: latLng.lng() });

      this.selectedPin = pinString;

    });
  }

  SaveData() {
    this.dialogRef.close(this.selectedMarkers);
    if (this.selectedMarkers.length == 0) {

      this.notificationsService.showNotification('Error! Please select shop first!', 'snack-bar-danger');
    }
    else {
      this.dialogRef.close(this.selectedMarkers);
    }
  }

}
