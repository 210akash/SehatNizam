import { Component, Inject } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { EmployeeLeaveGroupService } from '../employee-leave-group.service';
import { EmployeeLeaveTypeService } from '../../employee-leave-type/employee-leave-type.service';

@Component({
  selector: 'app-add-employee-leave-group',
  templateUrl: './add-employee-leave-group.component.html',
  styleUrl: './add-employee-leave-group.component.css',
  standalone: false
})

export class AddEmployeeLeaveGroupComponent {
  employeeLeaveGroupForm!: FormGroup;
  employeeLeaveTypeList: any[] = [];
  leaveGroupTypes!: FormArray;
  isLoading = true;
  isEditMode = false;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder,
    private employeeLeaveGroupService: EmployeeLeaveGroupService, private employeeLeaveTypeService: EmployeeLeaveTypeService,
    private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
      this.employeeLeaveGroupForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
      // startDate: [new Date(), [Validators.required, Validators.min(0)]],
      // endDate: [new Date(), [Validators.required, Validators.min(0)]],
      // leaveGroupTypes: this.formBuilder.array([])  // Initialize as empty array
    });
      this.LoadData(this.data.element);
  }


  // initializeLeaveGroupTypes() {
  //   this.employeeLeaveTypeList.forEach((leaveType) => {
  //     this.leaveGroupTypes.push(
  //       this.formBuilder.group({
  //         employeeLeaveTypeId: [leaveType.id],
  //         noOfLeaves: ['', [Validators.required, Validators.min(0)]]
         
  //       })
  //     );
  //   });
  // }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
       this.constantService.LoadData(element, this.employeeLeaveGroupForm);
      // this.employeeLeaveGroupForm.patchValue({
      //   id: element.id,
      //   name: element.name
      // });

      // element.employeeLeaveGroupTypes.forEach((leaveGroupType: any) => {
      //   const formGroup = this.leaveGroupTypes.controls.find(
      //     (ctrl: AbstractControl) =>
      //       ctrl.get('employeeLeaveTypeId')?.value === leaveGroupType.employeeLeaveTypeId
      //   );

      //   if (formGroup) {
      //     formGroup.patchValue({
      //       noOfLeaves: leaveGroupType.noOfLeaves
      //     });
      //   }
      // });
    } else {
      this.isEditMode = false;
    }
  }

  SaveData() {
    if (this.employeeLeaveGroupForm.invalid) {
      this.constantService.markFormGroupTouched(this.employeeLeaveGroupForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.employeeLeaveGroupForm.value);
    this.employeeLeaveGroupService.saveEmployeeLeaveGroup(_clienttemperatureForm).subscribe({
      next: (data) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
           this.isLoading = false;
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
}