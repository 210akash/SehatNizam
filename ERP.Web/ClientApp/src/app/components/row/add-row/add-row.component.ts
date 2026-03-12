import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../Service/constant.service';
import { NotificationsService } from '../../../Service/notification.service';
import { RowService } from '../row.service';
import { RackService } from '../../rack/rack.service';

@Component({
  selector: 'app-add-row',
  standalone: false,
  templateUrl: './add-row.component.html',
  styleUrl: './add-row.component.css'
})
export class AddRowComponent {
  createRowForm!: FormGroup;
  rackList: any;
  isLoading = false;
  RowListFilerForm!: FormGroup;
  dataSource!: any;
  roleList: any;
  dropdownSettings: any;
  isEditMode: boolean = false;
  pageSize = 1000;
  currentPage = 0;

  constructor(private rackService : RackService,private notificationsService: NotificationsService, private dialog: MatDialog, private formBuilder: FormBuilder, private constantService: ConstantService, private rowService: RowService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.createRowForm = this.formBuilder.group({
      id: [0],
      rackId: [],
      name: ['', Validators.required]
    });
    this.LoadData(this.data.element);
    this.getRackList();
  }

  get f() {
    return this.createRowForm.controls;
  }

  async saveRow() {
    this.isLoading = true;
    if (this.createRowForm.invalid) {
      this.constantService.markFormGroupTouched(this.createRowForm);
      return;
    }
    let _createRowForm: any = {};
    _createRowForm = Object.assign(_createRowForm, this.createRowForm.value);

    (await this.rowService.saveRow(_createRowForm)).subscribe(
      {
        next: (data: { Status: number; }) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('Row Saved Successfully!', 'snack-bar-success');
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
      this.constantService.LoadData(element, this.createRowForm);
    }
    console.log(this.createRowForm);
  }

  async getRackList() {
    let _rackForm: any = {};
    (await this.rackService.getAllRack(_rackForm)).subscribe((data) => {
     this.rackList = data.item1;
    });
  }
}
