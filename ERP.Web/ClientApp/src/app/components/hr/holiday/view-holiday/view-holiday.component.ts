import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
    selector: 'app-view-holiday',
    templateUrl: './view-holiday.component.html',
    styleUrl: './view-holiday.component.css',
    standalone: false
})

export class ViewHolidayComponent {
  holidayForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;

  constructor(private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.holidayForm = this.formBuilder.group({
       title: ['', Validators.required],
        date: [new Date(), [Validators.required, Validators.min(0)]],
        description: ['', Validators.required],
    });
    
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (this.data.element.id != null) {
      this.isEditMode = true;
    }
    this.constantService.LoadData(element, this.holidayForm);
  }
}
