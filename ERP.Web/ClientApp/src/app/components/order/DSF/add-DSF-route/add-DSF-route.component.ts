import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { DSFService } from '../DSF.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { RouteService } from '../../route/route.service';
import { ShopService } from '../../shop/shop.service';
import { TerritoryService } from '../../territory/territory.service';
import { ZoneService } from '../../zone/zone.service';

@Component({
  selector: 'app-add-DSF-route',
  templateUrl: './add-DSF-route.component.html',
  styleUrls: ['./add-DSF-route.component.css'], standalone: false
})

export class AddDSFRouteComponent implements OnInit {
  isLoading: any;
  addDSFRouteForm!: FormGroup;
  dataSource: any;
  displayedColumns: string[] = ['select', 'name', 'territory','totalShops'];

  selection = new Set<any>();
  isAllSelected: boolean = false;
  selectMultipleButtonsShow: boolean = false;

  finalRows: any;
  selectedRowsCount: any;

  selectedRoutes: Set<number> = new Set();

  zoneList: any;
  territoryList: any;

  constructor(private dSFService: DSFService, private zoneService: ZoneService, private territoryService: TerritoryService, private dialog: MatDialog, private notificationsService: NotificationsService, private routeService: RouteService, private formBuilder: FormBuilder, private constantService: ConstantService, private shopService: ShopService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.addDSFRouteForm = this.formBuilder.group({
      id: [0],
      name: [''],
      phoneNumber: [''],
      address: [''],
      designation: [''],
      shiftTimeStart: [''],
      shiftTimeend: ['']
    });

    this.LoadData(this.data.element);
    this.addDSFRouteForm.get('name')?.patchValue(this.data.element.firstName + ' ' + this.data.element.lastName);
    this.addDSFRouteForm.get('phoneNumber')?.patchValue(this.data.element.phoneNumber);
    this.addDSFRouteForm.get('address')?.patchValue(this.data.element.address);
    this.addDSFRouteForm.get('designation')?.patchValue(this.data.element.aspNetUserRoles[0]?.role?.name);

    this.getRoutes();
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.addDSFRouteForm);
  }

  // async getZones() {
  //   let _zoneFilterForm = {};
  //   (await this.zoneService.getAllZone(_zoneFilterForm)).subscribe({
  //     next: (data) => {
  //       this.zoneList = data.item1;
  //     },
  //     error: (error) => {
  //       console.log(error);
  //       this.isLoading = false;
  //     }
  //   });
  // }

  // async onZoneChange() {
  //   let zoneId = this.addDSFRouteForm.get('zoneId').value;
  //   (await this.territoryService.getTerritoryByAreaId(zoneId)).subscribe({
  //     next: (data) => {
  //       this.territoryList = data;
  //     },
  //     error: (error) => {
  //       console.log(error);
  //       this.isLoading = false;
  //     }
  //   });
  // }

  async getRoutes() {
    // let territoryId = this.addDSFRouteForm.get('territoryId').value;
    let dsfId = this.data.element.id;
    (await this.routeService.getRouteByDSFTerritory(dsfId)).subscribe(
      {
        next: (data) => {
          this.dataSource = data;
          this.updateCheckedStatus();
        },
        error: (error) => {
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  updateCheckedStatus() {
    const routeIdsToCheck = this.data.element.dsfRoute.filter((route: { isActive: any; }) => route.isActive).map((route: { routeId: any; }) => route.routeId);
    this.dataSource.forEach((route: { id: number; }) => {
      if (routeIdsToCheck.includes(route.id)) {
        this.selectedRoutes.add(route.id);
      }
    });

    const routeIdsToCheckSet = new Set(routeIdsToCheck);

    this.dataSource.forEach((item: { id: unknown; }) => {
      if (item.id && routeIdsToCheckSet.has(item.id)) {
        this.selection.add(item);       // Add item to selection if its ID is in the Set
      }
    });
  }

  async saveAddShops() {

    let routesToAdd = {
      'dsf': this.data.element,
      'routesToAdd': this.finalRows
    };

    (await this.dSFService.addDSFRoute(routesToAdd)).subscribe(
      {
        next: (data: { Status: number; }) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('Routed Saved Successfully', 'snack-bar-success');
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

  toggleCheckbox(row: any) {
    this.selectedRoutes = new Set();
    if (this.selection.has(row)) {
      this.selection.delete(row);
    } else {
      this.selection.add(row);
    }
    this.isAllSelected = this.isAllSelectedCheckbox();
    this.checkLength();
  }

  selectAll(event: any) {
    this.selectedRoutes = new Set();
    this.isAllSelected = event.checked;
    this.dataSource.forEach((row: any) => {
      if (this.isAllSelected) {
        this.selection.add(row);
        this.selectMultipleButtonsShow = true;
      } else {
        this.selection.delete(row);
        this.selectMultipleButtonsShow = false;
      }
    });
    this.checkLength();
  }

  isAllSelectedCheckbox(): boolean {
    const numSelected = this.selection.size;
    if (numSelected > 0) {
      this.selectMultipleButtonsShow = true;
    }
    else {
      this.selectMultipleButtonsShow = false;
    }
    const numRows = this.dataSource.length;
    return numSelected === numRows;
  }

  checkLength() {
    // Get the selected rows
    const selectedRows = Array.from(this.selection);
    this.finalRows = selectedRows;
    // Log the selected rows to the console
    console.log("Selected Rows:", selectedRows);
    this.selectedRowsCount = selectedRows.length;
  }

  countTotalShops(element: any) {

    var result = element?.shopRouteFrequency?.filter((x: { isActive: boolean; }) => x.isActive == true)?.length; // We are assigning only one route for now
    return result;
  }


}
