import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../Service/constant.service';
import { NotificationsService } from '../../../Service/notification.service';
import { RackService } from '../rack.service';

@Component({
  selector: 'app-add-rack',
  standalone: false,
  templateUrl: './add-rack.component.html',
  styleUrl: './add-rack.component.css'
})
export class AddRackComponent {
  createRackForm!: FormGroup;
  isLoading = false;
  RackListFilerForm!: FormGroup;
  dataSource!: any;
  roleList: any;
  dropdownSettings: any;
  isEditMode: boolean = false;
  pageSize = 1000;
  currentPage = 0;

  constructor(private notificationsService: NotificationsService, private dialog: MatDialog, private formBuilder: FormBuilder, private constantService: ConstantService, private rackService: RackService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.createRackForm = this.formBuilder.group({
      id: [0],
      companyId: [0],
      name: ['', Validators.required]
    });
    this.LoadData(this.data.element);
  }

  get f() {
    return this.createRackForm.controls;
  }

  async saveRack() {
    this.isLoading = true;
    if (this.createRackForm.invalid) {
      this.constantService.markFormGroupTouched(this.createRackForm);
      return;
    }
    let _createRackForm: any = {};
    _createRackForm = Object.assign(_createRackForm, this.createRackForm.value);

    (await this.rackService.saveRack(_createRackForm)).subscribe(
      {
        next: (data: { Status: number; }) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('Rack Saved Successfully!', 'snack-bar-success');
            this.dialog.closeAll();
            this.isLoading = false;
          }
          else if (data.Status == 409) {
            this.notificationsService.showNotification('Name already exist!', 'snack-bar-danger');
            this.isLoading = false;
          }
        },
        error: (error: any) => {
          this.notificationsService.showNotification('Please Fill the required fields!', 'snack-bar-danger');
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  LoadData(element: any) {
    if (this.data.element?.id != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.createRackForm);
    }
    console.log(this.createRackForm);
  }
}
