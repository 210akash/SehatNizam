import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { StoreService } from '../store.service';
import { LocationService } from '../../location/location.service';
import { CompanyService } from '../../company/company.service';

@Component({
    selector: 'app-delete-store',
    templateUrl: './delete-store.component.html',
    styleUrl: './delete-store.component.css',
    standalone: false
})

export class DeleteStoreComponent {
  storeForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  companyList: any;
  locationList: any;

  constructor( private locationService : LocationService, private companyService: CompanyService,private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private storeService: StoreService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.storeForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
      description: ['', Validators.required],
      address: ['', Validators.required],
      companyId: ['', Validators.required],
      locationId: ['', Validators.required],
      fixedAsset: [{value: false, disabled: true}, Validators.required]
    });
    this.LoadData(this.data.element);
  }


  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.storeForm);
    }
  }

  async delete() {
    (await this.storeService.deleteStore(this.data.element.id)).subscribe({
      next: (data) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        }
      },
      error: (error) => {
        console.log(error);
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }
}
