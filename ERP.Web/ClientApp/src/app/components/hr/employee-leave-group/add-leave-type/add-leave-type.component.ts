import { Component, Inject } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { EmployeeLeaveGroupService } from '../employee-leave-group.service';
import { EmployeeLeaveTypeService } from '../../employee-leave-type/employee-leave-type.service';
import { HRYearService } from '../../hryear/hryear.service';

@Component({
  selector: 'app-add-leave-type',
  templateUrl: './add-leave-type.component.html',
  styleUrls: ['./add-leave-type.component.css'],
  standalone: false,
})
export class AddLeaveTypeComponent {
  employeeLeaveGroupForm!: FormGroup;
  employeeLeaveTypeList: any[] = [];
  hrYearList: any[] = [];
  isEditMode = false;
  isLoading = false;

  constructor(
    private dialog: MatDialog,
    private notificationsService: NotificationsService,
    private formBuilder: FormBuilder,
    private employeeLeaveGroupService: EmployeeLeaveGroupService,
    private employeeLeaveTypeService: EmployeeLeaveTypeService,
    private hrYearService: HRYearService,
    private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.employeeLeaveGroupForm = this.formBuilder.group({
      employeeLeaveGroupId: [this.data.element.id],
      name: [this.data.element.name, Validators.required],
      employeeGroupLeaveType: this.formBuilder.array([])
    });
    this.getEmployeeLeaveTypesList();
    this.getHrYearList();
    this.LoadData(this.data.element);
  }

  get employeeGroupLeaveType(): FormArray {
    return this.employeeLeaveGroupForm.get('employeeGroupLeaveType') as FormArray;
  }

  Detail(index: number): FormArray {
    return this.employeeGroupLeaveType.at(index).get('employeeGroupLeaveTypeDetail') as FormArray;
  }

  createDetail(): FormGroup {
    return this.formBuilder.group({
      id: [0],
      employeeGroupLeaveTypeId: [0],
      employeeLeaveTypeId: [0, Validators.required],
      noOfLeaves: [0, [Validators.required, Validators.min(1)]]
    }, {
      validators: this.dateRangeValidator
    });
  }

  dateRangeValidator(group: AbstractControl): { [key: string]: any } | null {
    const start = group.get('startDate')?.value;
    const end = group.get('endDate')?.value;
    return start && end && new Date(start) > new Date(end) ? { dateRangeInvalid: true } : null;
  }

  addTransaction(index: number) {
    const group = this.formBuilder.group({
      id: [0],
      hrYearId: [null, Validators.required],
      employeeLeaveGroupId :  [this.data.element.id, Validators.required],
      employeeGroupLeaveTypeDetail: this.formBuilder.array([this.createDetail()])
    });
    this.employeeGroupLeaveType.insert(index + 1, group);
  }

  removeTransaction(index: number) {
    if (this.employeeGroupLeaveType.length > 1) {
      this.employeeGroupLeaveType.removeAt(index);
    } else {
      this.notificationsService.showNotification('At least one leave group is required.', 'snack-bar-danger');
    }
  }

  addDetail(detailIndex: number, parentIndex: number) {
    this.Detail(parentIndex).insert(detailIndex + 1, this.createDetail());
  }

  removeDetail(detailIndex: number, parentIndex: number) {
    const array = this.Detail(parentIndex);
    if (array.length > 1) {
      array.removeAt(detailIndex);
    } else {
      this.notificationsService.showNotification('At least one leave type is required.', 'snack-bar-danger');
    }
  }

  getEmployeeLeaveTypesList(): void {
    this.employeeLeaveTypeService.getAllEmployeeLeaveTypes({}).subscribe(data => {
      this.employeeLeaveTypeList = data.item1;
    });
  }

  getHrYearList(): void {
    this.hrYearService.getAllHryear({}).subscribe(data => {
      this.hrYearList = data.item1;
    });
  }

  LoadData(element: any) {
    if (this.data?.element?.employeeGroupLeaveType?.length != 0) {
      this.isEditMode = true;
      this.employeeGroupLeaveType.clear();

      const detailsArray = this.employeeLeaveGroupForm.get('employeeGroupLeaveType') as FormArray<FormGroup>;

      if (element.employeeGroupLeaveType && element.employeeGroupLeaveType.length > 0) {
        element.employeeGroupLeaveType.forEach((GroupLeaveTypeDetail: any) => {
          const detailArray = this.formBuilder.array<FormGroup<any>>([]);

          GroupLeaveTypeDetail.employeeGroupLeaveTypeDetail.forEach((detail: any) => {
            const detailGroup = this.formBuilder.group({
              id: [detail.id],
              employeeGroupLeaveTypeId: [detail.employeeGroupLeaveTypeId],
              employeeLeaveTypeId: [detail.employeeLeaveTypeId, Validators.required],
              noOfLeaves: [detail.noOfLeaves, [Validators.required, Validators.min(1)]]
            });

            detailArray.push(detailGroup);
          });

          const groupLeaveTypeFormGroup = this.formBuilder.group({
            id: [GroupLeaveTypeDetail.id],
             employeeLeaveGroupId :  [GroupLeaveTypeDetail.employeeLeaveGroupId, Validators.required],
            hrYearId: [GroupLeaveTypeDetail.hrYearId, Validators.required],
            employeeGroupLeaveTypeDetail: detailArray
          });

          detailsArray.push(groupLeaveTypeFormGroup); // ✅ now correctly typed
        });
        console.log(detailsArray);
      }
    }
     else {
        this.addTransaction(0); // Start with one block
      }
  }

  SaveData() {
    if (this.employeeLeaveGroupForm.invalid) {
      this.constantService.markFormGroupTouched(this.employeeLeaveGroupForm);
      return;
    }

    this.isLoading = true;
    const payload = this.employeeLeaveGroupForm.value;

    this.employeeLeaveGroupService.saveGroupLeaveType(payload).subscribe({
      next: (data) => {
        if (data.Status === 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        } else {
          this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
        }
        this.isLoading = false;
      },
      error: (err) => {
        this.notificationsService.showNotification(err.message, 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }

  isCurrentDateBetween(detail:any): boolean {
    return true;
  // if (!detail) {
  //   return false; // return false if any date is missing
  // }
  // const matchedYear = this.hrYearList.find(year => year.id === detail.hrYearId);
  // const currentDate = new Date();
  // return currentDate >= new Date(matchedYear.startDate) && currentDate <= new Date(matchedYear.endDate);
}
}
