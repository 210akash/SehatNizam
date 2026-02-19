import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { StoreService } from '../store.service';
import { CompanyService } from '../../company/company.service';
import { LocationService } from '../../location/location.service';

@Component({
    selector: 'app-add-store',
    templateUrl: './add-store.component.html',
    styleUrl: './add-store.component.css',
    standalone: false
})

export class AddStoreComponent {
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
      fixedAsset: [false, Validators.required]
    });
    
    this.LoadData(this.data.element);
    this.getCompanyList();
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.storeForm);
      this.storeForm.get('companyId')?.patchValue(element.location.companyId);
      this.getLocationList();
    }
  }

  SaveData() {
    if (this.storeForm.invalid) {
      this.constantService.markFormGroupTouched(this.storeForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.storeForm.value);

    this.storeService.saveStore(_clienttemperatureForm).subscribe({
      next: (data) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else
          this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        console.error(error);
        this.isLoading = false;
      }
    });
  }

  getCompanyList(): void {
    let _companyForm: any = {};
    this.companyService.getAllCompanys(_companyForm).subscribe(data => {
      this.companyList = data.item1;
    });
  }

  getLocationList(): void {
    var companyId = this.storeForm.get('companyId')?.value;
    this.locationService.getLocationByCompany(companyId).subscribe(data => {
      this.locationList = data;
    });
  }
}
