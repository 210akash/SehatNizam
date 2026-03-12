import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { CreatePricingGroupComponent } from '../create-pricing-group/create-pricing-group.component';
import { PricingGroupService } from '../pricing-group.service';
import { MatTableDataSource } from '@angular/material/table';
import { CreatePricingGroupDetailsComponent } from '../create-pricing-group-details/create-pricing-group-details.component';
import { CreateDistributorPriceGroupComponent } from '../create-distributor-price-group/create-distributor-price-group.component';
import { NotificationsService } from '../../../../Service/notification.service';
import { CopyPricingGroupDetailsComponent } from '../copy-pricing-group-details/copy-pricing-group-details.component';

@Component({
  selector: 'app-pricing-group-list',
  templateUrl: './pricing-group-list.component.html',
  styleUrls: ['./pricing-group-list.component.css'], standalone: false
})

export class PricingGroupListComponent implements OnInit {
  dataSource: any;
  isLoading = false;
  gPricingGroupObj: any;
  dialogRef: any;
  displayedColumns: string[] = ['title', 'description', 'createdDate', 'actions'];

  constructor(private notificationsService: NotificationsService,private pricingGroupService: PricingGroupService,private dialog: MatDialog,) { }

  ngOnInit(): void {
    this.bindData();
  }

  openProductDialog(element: any): void {
    const dialogRef = this.dialog.open(CreatePricingGroupComponent, {
      data: { element: element },
      width: '30%',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  openDistributorPriceGroup(element: any): void {
    const dialogRef = this.dialog.open(CreateDistributorPriceGroupComponent, {
      data: { element: element },
      width: '80%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }
  
  async openProductPricing(element: any, isViewOnly: boolean){
    element.isViewOnly = isViewOnly;
    const dialogRef = this.dialog.open(CreatePricingGroupDetailsComponent, {
      data: { element: element },
      width: '80%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  async copyProductPricing(element: any, isViewOnly: boolean){
    element.isViewOnly = isViewOnly;
    const dialogRef = this.dialog.open(CopyPricingGroupDetailsComponent, {
      data: { element: element },
      width: '80%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  openDeletePricingGroupPopup(element: any, template: any) {
    this.gPricingGroupObj = element;
    this.dialogRef = this.dialog.open(template, {
      width: '30%',
      height: 'auto%',
      disableClose: true,
    });
  }

  async deletePricing() {
    (await this.pricingGroupService.deletePricingGroup(this.gPricingGroupObj.id)).subscribe({
      next: (data) => {
        if (data.Status == 200) {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Message, 'snack-bar-success');
          this.dialog.closeAll();
          this.bindData();
        }
        else {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Message, 'snack-bar-danger');
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

    async bindData() {
      this.isLoading = true;
  
      let _productListFilerForm: any = {};
      _productListFilerForm = Object.assign(_productListFilerForm);
      (await this.pricingGroupService.getAllPricingGroup(_productListFilerForm)).subscribe({
        next: (data) => {
          this.dataSource = new MatTableDataSource(data.item1);

          this.isLoading = false;
        },
        error: (error) => {
          console.log(error);
          this.isLoading = false;
        }
      });
    }
  
}
