import { Component, Inject, OnInit } from '@angular/core';
import { PricingGroupService } from '../pricing-group.service';
import { FormBuilder, FormGroup, FormArray, FormControl } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-create-distributor-price-group',
  templateUrl: './create-distributor-price-group.component.html',
  styleUrls: ['./create-distributor-price-group.component.css'], standalone: false
})
export class CreateDistributorPriceGroupComponent implements OnInit {
  isLoading = false;
  createDistributorPricingGroupForm!: FormGroup;
  distributorList: any[] = [];

  constructor(
    private notificationsService: NotificationsService,
    private pricingGroupService: PricingGroupService,
    private constantService: ConstantService,
    private formBuilder: FormBuilder,
    private dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.createDistributorPricingGroupForm = this.formBuilder.group({
      distributors: this.formBuilder.array([]) // Initialize as a FormArray
    });

    this.getAllDistributor();
  }

  async getAllDistributor() {
    this.isLoading = true;
    (await this.pricingGroupService.getAllDistributorByGroupId(this.data.element.id)).subscribe({
      next: (data: { Status: number; Data: any[]; }) => {
        if (data.Status === 200) {
          this.distributorList = data.Data;
          this.populateFormArray();
          this.isLoading = false;
        } else {
          this.notificationsService.showNotification('Some Error has occurred!', 'snack-bar-danger');
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

  populateFormArray() {
    const distributorsArray = this.createDistributorPricingGroupForm.get('distributors') as FormArray;
    distributorsArray.clear();

    this.distributorList.forEach(distributor => {
      distributorsArray.push(
        this.formBuilder.group({
          DealershipId: [distributor.DealershipId], // Add DealershipId here
          IsSelected: [{ value: distributor.IsSelected, disabled: distributor.IsOccupiedInOtherGroup }],
          DealershipName: [distributor.DealershipName],
          TerritoryName: [distributor.TerritoryName],
          AreaName: [distributor.AreaName],
          ZoneName: [distributor.ZoneName],
          RegionName: [distributor.RegionName],
          GroupName: [distributor.GroupName],
          IsOccupiedInOtherGroup: [distributor.IsOccupiedInOtherGroup]
        })
      );
    });
  }


  get distributorsControls() {
    return (this.createDistributorPricingGroupForm.get('distributors') as FormArray).controls;
  }

  async saveDistributorPricingGroup() {
    const formValues = this.createDistributorPricingGroupForm.getRawValue().distributors; // Includes disabled fields

    // Create the command object to match the C# model
    const command = {
      GroupId: this.data.element.id, // Assign GroupId
      GetAllDistributorByGroupId: formValues // Assign distributors list
    };

    console.log('Command to send:', command);
    (await this.pricingGroupService.saveDistributorPricingGroup(command)).subscribe(
      {
        next: (data: { Status: number; }) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('Pricing Group Saved Successfully!', 'snack-bar-success');
            this.dialog.closeAll();
            this.isLoading = false;
          }
          else if (data.Status == 409) {
            this.notificationsService.showNotification('Group with same Title already exist!', 'snack-bar-danger');
            this.isLoading = false;
          }
        },
        error: (error: any) => {
          this.notificationsService.showNotification('Please Fill the required fields!', 'snack-bar-danger');
          console.log(error);
          this.isLoading = false;
        }
      });
    // Now send `command` to API (if needed)
    // this.yourService.saveDistributorPricingGroup(command).subscribe(response => {
    //   console.log('Response:', response);
    // });
  }

}
