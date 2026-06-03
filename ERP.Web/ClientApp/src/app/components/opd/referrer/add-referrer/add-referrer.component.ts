import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { ReferrerService } from '../referrer.service';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-add-referrer',
  templateUrl: './add-referrer.component.html',
  styleUrls: ['./add-referrer.component.css'],
  standalone: false
})
export class AddReferrerComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;
  departments: any[] = [];
  isEditMode: boolean = false;

  constructor(
    private fb: FormBuilder,
    private dialog: MatDialog,
    private Referrer: ReferrerService,
    private notifications: NotificationsService,
    private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      id: [0],
      name: ['', Validators.required],
      hospital: ['', Validators.required],
      phoneNo: ['']
    });

    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.form);
    }
  }

  save(): void {
    if (this.form.invalid) return;

    this.isLoading = true;
    const command = this.form.value;

    this.Referrer.saveReferrer(command).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res.Status === 200) {
          this.notifications.showNotification(res.Data || 'Referrer Saved Successfully!', 'snack-bar-success');
          this.dialog.closeAll();
        } else if (res.Status === 409) {
          this.notifications.showNotification('Referrer with this code already exists!', 'snack-bar-danger');
        } else {
          this.notifications.showNotification(res.Message || 'Error saving Referrer!', 'snack-bar-danger');
        }
      },
      error: (error: any) => {
        this.isLoading = false;
        const message = error?.error?.Message || 'An error occurred';
        this.notifications.showNotification(message, 'snack-bar-danger');
      }
    });
  }
}
