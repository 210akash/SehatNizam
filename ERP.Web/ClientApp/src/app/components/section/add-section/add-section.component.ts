import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../Service/constant.service';
import { NotificationsService } from '../../../Service/notification.service';
import { RackService } from '../../rack/rack.service';
import { SectionService } from '../section.service';
import { RowService } from '../../row/row.service';

@Component({
  selector: 'app-add-section',
  standalone: false,
  templateUrl: './add-section.component.html',
  styleUrl: './add-section.component.css'
})
export class AddSectionComponent {
  createSectionForm!: FormGroup;
  rowList: any;
  rackList: any;
  isLoading = false;
  dataSource!: any;
  roleList: any;
  dropdownSettings: any;
  isEditMode: boolean = false;
  pageSize = 1000;
  currentPage = 0;

  constructor(private rackService: RackService,private rowService : RowService,private notificationsService: NotificationsService, private dialog: MatDialog, private formBuilder: FormBuilder, private constantService: ConstantService, private sectionService: SectionService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.createSectionForm = this.formBuilder.group({
      id: [0],
      rackId : [0],
      rowId: [0],
      name: ['', Validators.required]
    });
    this.LoadData(this.data.element);
    this.getRackList();
    //this.getRowList();
  }

  get f() {
    return this.createSectionForm.controls;
  }

  async saveSection() {
    this.isLoading = true;
    if (this.createSectionForm.invalid) {
      this.constantService.markFormGroupTouched(this.createSectionForm);
      return;
    }
    let _createSectionForm: any = {};
    _createSectionForm = Object.assign(_createSectionForm, this.createSectionForm.value);

    (await this.sectionService.saveSection(_createSectionForm)).subscribe(
      {
        next: (data: { Status: number; }) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('Section Saved Successfully!', 'snack-bar-success');
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
      this.constantService.LoadData(element, this.createSectionForm);
    }
    console.log(this.createSectionForm);
  }

  // async getRowList() {
  //   let _rowForm: any = {};
  //   (await this.rowService.getAllRow(_rowForm)).subscribe((data) => {
  //    this.rowList = data.item1;
  //   });
  // }

  async getRowListById() {

    var rackId =  this.createSectionForm.get('rackId')?.value;
    (await this.rowService.getRowByRackId(rackId)).subscribe((data: any) => {
     this.rowList = data;
    });
  }

  async getRackList() {
    let _rackForm: any = {};
    (await this.rackService.getAllRack(_rackForm)).subscribe((data) => {
     this.rackList = data.item1;
    });
  }
}
