import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TemplateService } from '../template.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-add-template',
  templateUrl: './add-template.component.html',
  styleUrls: ['./add-template.component.css'], standalone: false
})

export class AddTemplateComponent implements OnInit {
  createTemplateForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  isEditMode: boolean = false;
  isCodeExists: boolean = false;
  codeList: string[] = [];
  codeToCheck: string = "";

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private templateService: TemplateService, private formBuilder: FormBuilder,
    public constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.createTemplateForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
      description: ['', Validators.required],
      content: ['', Validators.required],
    });
    this.LoadData(this.data.element);
  }

  get f() {
    return this.createTemplateForm.controls;
  }

  async saveTemplate() {
    if (this.createTemplateForm.invalid) {
      this.constantService.markFormGroupTouched(this.createTemplateForm);
      return;
    }
    this.isLoading = true;
    let _createTemplateForm: any = {};
    _createTemplateForm = Object.assign(_createTemplateForm, this.createTemplateForm.value);

    (await this.templateService.save(_createTemplateForm)).subscribe(
      {
        next: (data) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('Template Saved Successfully!', 'snack-bar-success');
            this.dialog.closeAll();
            this.isLoading = false;
          }
          else if (data.Status == 409) {
            this.notificationsService.showNotification('Name already exist!', 'snack-bar-danger');
            this.isLoading = false;
          }
        },
        error: (error) => {
          this.notificationsService.showNotification('Please Fill the required fields!', 'snack-bar-danger');
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.createTemplateForm);
    console.log(this.createTemplateForm);
  }

}