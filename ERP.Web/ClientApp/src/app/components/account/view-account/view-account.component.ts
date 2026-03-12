import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConstantService } from '../../../Service/constant.service';

@Component({
    selector: 'app-view-account',
    templateUrl: './view-account.component.html',
    styleUrl: './view-account.component.css',
    standalone: false
})

export class ViewAccountComponent {
  isLoading = false;
  isEditMode: boolean = true;
  accountViewForm!: FormGroup;
  constructor(private constantService : ConstantService,private formBuilder: FormBuilder,@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }
  ngOnInit(): void {
      this.accountViewForm = this.formBuilder.group({
        code: [''],
        name: [''],
        accountTypeId: [''],
        description: [''],
      });
      
      this.LoadData(this.data.element);
    }
  
    LoadData(element: any) {
      if (this.data.element.id != null) {
        this.isEditMode = true;
      }
      this.constantService.LoadData(element, this.accountViewForm);
    }
}  
