import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { RouteService } from '../route.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-add-shops-route-frequency',
  templateUrl: './add-shops-route-frequency.component.html',
  styleUrls: ['./add-shops-route-frequency.component.css'],standalone: false
})

export class AddShopsRouteFrequencyComponent implements OnInit {
  isLoading: any;
  addShopsRouteForm!: FormGroup;
  dialogRef: any;

  daysOfWeek: string[] = ['monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday', 'sunday'];
  storeSchedules: { [key: number]: { [key: string]: boolean } } = {};

  savedData: any;
  gRoute: any;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private routeService: RouteService, private formBuilder: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  async ngOnInit(): Promise<void> {
    this.addShopsRouteForm = this.formBuilder.group({
      id: [0],
      routeName: [''],
      zone: [''],
      territory: [''],
      totalShops: [0]
    });

    this.getRouteById(this.data.element?.id);
  }

  async LoadData(element: any) {

    this.initializeSchedules();
    
    this.addShopsRouteForm.get('zone')?.patchValue(element.territory?.area?.zone?.name);
    this.addShopsRouteForm.get('territory')?.patchValue(element.territory?.name);
    this.addShopsRouteForm.get('routeName')?.patchValue(element.name);
    this.addShopsRouteForm.get('totalShops')?.patchValue(element.shopRouteFrequency?.filter((x: { isActive: boolean; }) => x.isActive === true)?.length);
  }

  async saveAddShops() {
    const finalSchedule = this.gRoute.territory?.shop.map((store: any) => {
      const schedule = this.daysOfWeek.reduce((acc, day) => {
        acc[day] = this.storeSchedules[store.id]?.[day] || false;
        return acc;
      }, {} as { [key: string]: boolean });

      return {
        shopId: store.id,
        schedule
      };
    });

    let shopsFrequencyToAdd = {
      'routeId': this.gRoute.id,
      'routeFrequencyList': finalSchedule
    };

    (await this.routeService.addShopsRouteFrequency(shopsFrequencyToAdd)).subscribe(
      {
        next: (data: { Status: number; }) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('Shops Frequency Saved Successfully', 'snack-bar-success');
            this.dialog.closeAll();
            this.isLoading = false;
          }
          else if (data.Status == 409) {
            this.notificationsService.showNotification('Name already exist!', 'snack-bar-danger');
            this.isLoading = false;
          }
        },
        error: (error: any) => {
          this.notificationsService.showNotification('Please Fill the required fields!', 'snack-bar-danger');
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  async initializeSchedules() {
    this.gRoute.territory?.shop.forEach((store: any) => {
      this.storeSchedules[store.id] = this.daysOfWeek.reduce((acc, day) => {
        acc[day] = false; // Default to false (unchecked)
        return acc;
      }, {} as { [key: string]: boolean });
    });

    await this.getShopRouteFrequencyByTerritoryId();
  }

  async getShopRouteFrequencyByTerritoryId() {
    (await this.routeService.getShopRouteFrequencyByTerritoryId(this.gRoute.territoryId)).subscribe(
      {
        next: (data: any) => {
          this.savedData = data;
          this.loadSavedDataIntoStoreSchedules();
          // this.LoadData(this.data.element);
        },
        error: (error: any) => {
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  loadSavedDataIntoStoreSchedules() {
    // Loop through the saved data and populate storeSchedules
    this.savedData.forEach((item: { [x: string]: boolean; shopId: any; }) => {
      const shopId = item.shopId;

      // For each shop, set the values for each day of the week
      if (this.storeSchedules[shopId]) {
        // Dynamically assign the saved values from the API to the storeSchedules object
        this.daysOfWeek.forEach(day => {
          this.storeSchedules[shopId][day] = item[day.toLowerCase()] || false; // Use lowercase keys like 'monday'
        });
      }
    });

    // If you want to log the updated storeSchedules for debugging
    console.log('Updated Store Schedules:', this.storeSchedules);
  }

  async getRouteById(routeId: any) {
    (await this.routeService.getRouteById(routeId)).subscribe({
      next: (data: any) => {
        this.gRoute = data;
        this.LoadData(data);
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }


}
