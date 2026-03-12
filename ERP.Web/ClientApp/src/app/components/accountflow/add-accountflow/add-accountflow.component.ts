import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { AccountFlowService } from '../accountflow.service';
import { AccountHeadService } from '../../accounthead/accounthead.service';

@Component({
    selector: 'app-add-accountflow',
    templateUrl: './add-accountflow.component.html',
    styleUrl: './add-accountflow.component.css',
    standalone: false
})

export class AddAccountFlowComponent {
  accountflowForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  storeList: any;
  selectedRolls: any;
  accountHeadList : any;

  constructor(
    private accountHeadService: AccountHeadService,
    private dialog: MatDialog, 
    private notificationsService: NotificationsService, 
    private formBuilder: FormBuilder, 
    private accountflowService: AccountFlowService, 
    private constantService: ConstantService, 
    @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.accountflowForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      name: ['', Validators.required],
      description: ['', Validators.required],
      companyId: [0],
    });
    
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.accountflowForm);
    }
    else   
     this.getAccountFlowCode();
  }

  SaveData() {
    if (this.accountflowForm.invalid) {
      this.constantService.markFormGroupTouched(this.accountflowForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.accountflowForm.value);

    this.accountflowService.saveAccountFlow(_clienttemperatureForm).subscribe({
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

  getAccountFlowCode() {
    this.accountflowService.getAccountFlowCode().subscribe((data: any) => {
      this.accountflowForm.get('code')?.patchValue(data.code);
      console.log(data.code);
      console.log(this.accountflowForm.get('code')?.value);
    });
  }
}
