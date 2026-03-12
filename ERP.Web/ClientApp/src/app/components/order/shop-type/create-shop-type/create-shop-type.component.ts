import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ShopTypeService } from '../shop-type.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-create-shop-type',
  templateUrl: './create-shop-type.component.html',
  styleUrls: ['./create-shop-type.component.css'],standalone: false
})

export class CreateShopTypeComponent implements OnInit {
  createShopTypeForm!: FormGroup;
  isLoading = false;
  shopTypeListFilerForm!: FormGroup;
  dataSource!: any;
  roleList: any;
  dropdownSettings: any;
  isEditMode: boolean = false;
  pageSize = 1000;
  currentPage = 0;

  constructor(private notificationsService: NotificationsService, private dialog: MatDialog, private formBuilder: FormBuilder, private constantService: ConstantService, private shopTypeService: ShopTypeService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.createShopTypeForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
    });

    this.LoadData(this.data.element);
  }

  get f() {
    return this.createShopTypeForm.controls;
  }

  async saveShopType() {
    this.isLoading = true;
    if (this.createShopTypeForm.invalid) {
      this.constantService.markFormGroupTouched(this.createShopTypeForm);
      return;
    }
    let _createShopTypeForm: any = {};
    _createShopTypeForm = Object.assign(_createShopTypeForm, this.createShopTypeForm.value);

    (await this.shopTypeService.saveShopType(_createShopTypeForm)).subscribe(
      {
        next: (data: { Status: number; }) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('Shop Type Saved Successfully!', 'snack-bar-success');
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

  LoadData(element: any) {
    if (this.data.element?.id != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.createShopTypeForm);
    }
    console.log(this.createShopTypeForm);
  }


}