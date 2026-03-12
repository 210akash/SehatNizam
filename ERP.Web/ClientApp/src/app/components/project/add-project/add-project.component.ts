import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { ProjectService } from '../project.service';
import { CompanyService } from '../../company/company.service';
import { StoreService } from '../../store/store.service';

@Component({
    selector: 'app-add-project',
    templateUrl: './add-project.component.html',
    styleUrl: './add-project.component.css',
    standalone: false
})

export class AddProjectComponent {
  projectForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  companyList: any;
  storeList: any;
  selectedRolls: any;

  constructor(private storeService : StoreService , private companyService: CompanyService,private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private projectService: ProjectService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.projectForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
      description: ['', Validators.required],
      companyId: [0],
      storeIds: [0],
    });
    
    this.LoadData(this.data.element);
    this.getCompanyList();
        this.getStoreList();
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.projectForm);
    }
  }

  SaveData() {
    if (this.projectForm.invalid) {
      this.constantService.markFormGroupTouched(this.projectForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.projectForm.value);

    this.projectService.saveProject(_clienttemperatureForm).subscribe({
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

    getStoreList() {
       let _StoreForm: any = {};
    this.storeService.getAllStores(_StoreForm).subscribe((data) => {
     this.storeList = data.item1;
    });
  }

//   getSelectedStoreNames(): string {
//   return this.storeList
//     .filter((store: { id: any; }) => this.selectedRolls?.includes(store.id))
//     .map((store: { name: any; }) => store.name)
//     .join(', ');
// }

}
