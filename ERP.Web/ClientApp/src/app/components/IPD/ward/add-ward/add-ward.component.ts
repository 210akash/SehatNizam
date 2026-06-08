import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { DepartmentService } from '../../../department/department.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { WardService } from '../ward.service';

@Component({
    selector: 'app-add-ward',
    templateUrl: './add-ward.component.html',
    styleUrl: './add-ward.component.css',
    standalone: false
})

export class AddWardComponent {
  wardForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  storeList: any;
  selectedRolls: any;
  departmentList : any;

  constructor(
    private departmentService: DepartmentService,
    private dialog: MatDialog, 
    private notificationsService: NotificationsService, 
    private formBuilder: FormBuilder, 
    private wardService: WardService, 
    private constantService: ConstantService, 
    @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.wardForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      name: ['', Validators.required],
      description: ['', Validators.required],
      departmentId: ['', Validators.required],
    });
    
    this.LoadData(this.data.element);
    this.getdepartmentList();
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.wardForm);
    }
    else   
     this.getWardCode();
  }

  SaveData() {
    if (this.wardForm.invalid) {
      this.constantService.markFormGroupTouched(this.wardForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.wardForm.value);

    this.wardService.saveWard(_clienttemperatureForm).subscribe({
      next: (data: { Status: number; Data: string; }) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else
          this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error: string) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        console.error(error);
        this.isLoading = false;
      }
    });
  }

  getWardCode() {
    this.wardService.getWardCode().subscribe((data: any) => {
      this.wardForm.get('code')?.patchValue(data.code);
      console.log(data.code);
      console.log(this.wardForm.get('code')?.value);
    });
  }

  getdepartmentList() {
    this.departmentService.getAllDepartments({}).subscribe((data: any) => {
     this.departmentList = data.item1;
    });
  }
}
