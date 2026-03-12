import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-view-user-attendance',
  templateUrl: './view-user-attendance.component.html',
  styleUrls: ['./view-user-attendance.component.css'],standalone: false
})

export class ViewUserattendanceComponent implements OnInit {
  viewUserattendance!: FormGroup;
  isLoading = false;
  isAbsent = false;

  constructor(private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.viewUserattendance = this.formBuilder.group({
      id: [0],
      user: [''],
      regionName: [''],
      zoneName: [''],
      areaName: [''],
      territoryName: [''],
      dealershipName: [''],
      isPresent: [''],
      reason: [''],
      attendanceDate: [''],
      checkoutDate: [''],
    });
this
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.viewUserattendance);
    var user = element.user?.firstName + ' ' + element.user?.lastName + '(' + element.user?.aspNetUserRoles[0].role?.name + '-' + element.user?.email + ')';

    this.viewUserattendance.get('regionName')?.patchValue(element.dealership?.territory?.area?.zone?.region?.name);
    this.viewUserattendance.get('zoneName')?.patchValue(element.dealership?.territory?.area?.zone?.name);
    this.viewUserattendance.get('areaName')?.patchValue(element.dealership?.territory?.area?.name);
    this.viewUserattendance.get('territoryName')?.patchValue(element.dealership?.territory?.name);
    this.viewUserattendance.get('dealershipName')?.patchValue(element.dealership?.name);


    this.viewUserattendance.get('user')?.patchValue(user);
    this.viewUserattendance.get('isPresent')?.patchValue(element.isPresent == true ? 'Present' : 'Leave');
    this.isAbsent = !element.isPresent;
    const attendanceDate = new Date(element.attendanceDate);

    const formattedDate = attendanceDate.toLocaleString('en-US', {
                          day: '2-digit',
                          month: 'short',
                          year: 'numeric',
                          hour: 'numeric',
                          minute: 'numeric',
                          hour12: true,
                        });

this.viewUserattendance.get('attendanceDate')?.patchValue(formattedDate);
debugger;
const checkoutTime = new Date(element.checkOut);

const formattedcheckoutTime = checkoutTime.toLocaleString('en-US', {
  hour: 'numeric',
  minute: 'numeric',
  hour12: true,
});

this.viewUserattendance.get('checkoutDate')?.patchValue(formattedcheckoutTime);

    // this.viewUserattendance.get('user')?.patchValue(FullName);
    // this.viewUserattendance.get('zone')?.patchValue(element.zone?.name);
    // this.viewUserattendance.get('territory')?.patchValue(element.isAllTerritoryCheck === true ? 'All Territories' : element.territory?.name);
  }
}
