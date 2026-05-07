import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { LabOrderService } from '../lab-order.service';

@Component({
  selector: 'app-add-lab-order',
  templateUrl: './add-lab-order.component.html',
  styleUrls: ['./add-lab-order.component.css'],
  standalone: false
})
export class AddLabOrderComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;
  constructor(
    private fb: FormBuilder,
    private dialog: MatDialog,
    private service: LabOrderService,
    private notifications: NotificationsService,
    private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }
  ngOnInit(): void {
    this.form = this.fb.group({ id: [0], appointmentId: [0, Validators.required], labOrderTypeId: [0, Validators.required], statusId: [1, Validators.required] });
    if (this.data?.element?.id != null) this.constantService.LoadData(this.data.element, this.form);
  }
  async save(): Promise<void> {
    if (this.form.invalid) return;
    (await this.service.saveLabOrder(this.form.value)).subscribe((res: any) => {
      if (res?.Status === 200) { this.notifications.showNotification('Lab Order Saved Successfully!', 'snack-bar-success'); this.dialog.closeAll(); }
    });
  }
}
