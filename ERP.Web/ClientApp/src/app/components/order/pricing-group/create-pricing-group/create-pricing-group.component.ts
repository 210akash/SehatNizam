import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { PricingGroupService } from '../pricing-group.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-create-pricing-group',
  templateUrl: './create-pricing-group.component.html',
  styleUrls: ['./create-pricing-group.component.css'], standalone: false
})

export class CreatePricingGroupComponent implements OnInit {
  createPricingGroupForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private pricingGroupService: PricingGroupService, private constantService: ConstantService, private dialog: MatDialog, private formBuilder: FormBuilder, private notificationsService: NotificationsService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.createPricingGroupForm = this.formBuilder.group({
      id: [0],
      title: ['', Validators.required],
      description: ['']
    });
    this.LoadData(this.data.element);
  }
  LoadData(element: any) {
    if (this.data.element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.createPricingGroupForm);
    }
  }
  get f() {
    return this.createPricingGroupForm.controls;
  }
  async savePricingGroup() {
    this.isLoading = true;
    if (this.createPricingGroupForm.invalid) {
      this.constantService.markFormGroupTouched(this.createPricingGroupForm);
      return;
    }
    let _createPricingGroupForm: any = {};
    _createPricingGroupForm = Object.assign(_createPricingGroupForm, this.createPricingGroupForm.value);

    (await this.pricingGroupService.savePricingGroup(_createPricingGroupForm)).subscribe(
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
  }
}
