import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConstantService } from '../../../Service/constant.service';

@Component({
    selector: 'app-view-accountgroup',
    templateUrl: './view-accountgroup.component.html',
    styleUrl: './view-accountgroup.component.css',
    standalone: false
})

export class ViewAccountGroupComponent {
  isLoading = false;
  accountFlow :any;
  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }
  ngOnInit(): void {
    this.accountFlow = this.data.element.account.accountFlow.name;
    }
}  
