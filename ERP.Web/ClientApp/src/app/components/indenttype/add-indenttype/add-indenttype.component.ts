import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { IndentTypeService } from '../indenttype.service';

@Component({
    selector: 'app-add-indenttype',
    templateUrl: './add-indenttype.component.html',
    styleUrl: './add-indenttype.component.css',
    standalone: false
})

export class AddIndentTypeComponent {
  indenttypeForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private indenttypeService: IndentTypeService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.indenttypeForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
      companyId: [0],
    });
    
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.indenttypeForm);
    }
  }

  SaveData() {
    if (this.indenttypeForm.invalid) {
      this.constantService.markFormGroupTouched(this.indenttypeForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.indenttypeForm.value);

    this.indenttypeService.saveIndentType(_clienttemperatureForm).subscribe({
      next: (data: { Status: number; Data: string; }) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else
          this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error: string) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        console.error(error);
        this.isLoading = false;
      }
    });
  }


}
