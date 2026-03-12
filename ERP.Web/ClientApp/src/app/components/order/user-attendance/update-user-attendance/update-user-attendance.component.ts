import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { UserAttendanceService } from '../user-attendance.service';

@Component({
    selector: 'app-update-user-attendance',
    templateUrl: './update-user-attendance.component.html',
    styleUrl: './update-user-attendance.component.css',
    standalone: false
})

export class UpdateUserAttendanceComponent {
  userattendanceForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  companyList: any;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private userAttendanceService: UserAttendanceService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

ngOnInit(): void {
  const attendanceDate = this.data.element.attendanceDate;

  this.userattendanceForm = this.formBuilder.group({
    id: [this.data.element.id],
    userId: [this.data.element.userId],
    attendanceDate: [attendanceDate],

    timeIn: [
      this.data.element.timeIn
        ? this.constantService.toDateTimeLocal(this.data.element.timeIn)
        : this.constantService.toDateTimeLocal(attendanceDate),
      Validators.required
    ],

    timeOut: [
      this.data.element.timeOut
        ? this.constantService.toDateTimeLocal(this.data.element.timeOut)
        : this.constantService.toDateTimeLocal(attendanceDate),
      Validators.required
    ],

    attendanceType: [1],
    deviceType: [3],
    isManualIn: [this.data.element.timeIn ? 0 : 1, Validators.required],
    isManualOut: [this.data.element.timeOut ? 0 : 1, Validators.required]
  });

}

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.userattendanceForm);
    }
  }

  async SaveData() {
    if (this.userattendanceForm.invalid) {
      this.constantService.markFormGroupTouched(this.userattendanceForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.userattendanceForm.value);

    (await this.userAttendanceService.saveUserAttendance(_clienttemperatureForm)).subscribe({
      next: (data:any) => {
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

onTimeInChange(event: Event) {
  const input = event.target as HTMLInputElement;
  const [selectedDate, selectedTime] = input.value.split('T');
 var originalDate = this.data.element.attendanceDate.split('T')[0];
  // force original date, keep selected time
  const correctedValue = `${originalDate}T${selectedTime}`;

  this.userattendanceForm.get('timeIn')?.setValue(correctedValue, {
    emitEvent: false
  });
}

onTimeOutChange(event: Event) {
  const input = event.target as HTMLInputElement;
  const selectedValue = input.value;

  if (!selectedValue) return;

  const [, selectedTime] = selectedValue.split('T');

  const attendanceDate = new Date(this.data.element.attendanceDate);

  // Allowed dates
  const sameDay = this.constantService.formatDate(attendanceDate);
  const nextDay = this.constantService.formatDate(
    new Date(attendanceDate.getTime() + 24 * 60 * 60 * 1000)
  );

  const selectedDate = selectedValue.split('T')[0];

  let allowedDate: string;

  if (selectedDate === sameDay || selectedDate === nextDay) {
    allowedDate = selectedDate;
  } else {
    // default fallback → same day
    allowedDate = sameDay;
  }

  const correctedValue = `${allowedDate}T${selectedTime}`;

  this.userattendanceForm.get('timeOut')?.setValue(correctedValue, {
    emitEvent: false
  });
}

onTimeOutChange1(event: Event) {
  const input = event.target as HTMLInputElement;
  const selectedValue = input.value;

  if (!selectedValue) return;

  const [selectedDate, selectedTime] = selectedValue.split('T');

  const attendanceDate = new Date(this.data.element.attendanceDate);
  attendanceDate.setHours(0, 0, 0, 0);

  const sameDay = this.constantService.formatDate(attendanceDate);

  const nextDayDate = new Date(attendanceDate);
  nextDayDate.setDate(nextDayDate.getDate() + 1);
  const nextDay = this.constantService.formatDate(nextDayDate);

  // ---------- Rule 1: Allowed date ----------
  let allowedDate =
    selectedDate === sameDay || selectedDate === nextDay
      ? selectedDate
      : sameDay;

  let correctedDateTime = new Date(`${allowedDate}T${selectedTime}`);

  // ---------- Rule 2: timeOut >= timeIn ----------
  const timeInValue = this.userattendanceForm.get('timeIn')?.value;
  if (timeInValue) {
    const timeInDate = new Date(timeInValue);
    if (correctedDateTime < timeInDate) {
      correctedDateTime = new Date(timeInDate);
    }
  }

  // ---------- Rule 3: timeOut <= attendanceDate + 1 day ----------
  const maxOutDate = new Date(nextDayDate);
  maxOutDate.setHours(23, 59, 59, 999);

  if (correctedDateTime > maxOutDate) {
    correctedDateTime = maxOutDate;
  }

  this.userattendanceForm.get('timeOut')?.setValue(
    correctedDateTime,
    { emitEvent: false }
  );
}

}
