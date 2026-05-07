import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-view-service',
  templateUrl: './view-service.component.html',
  styleUrls: ['./view-service.component.css'],
  standalone: false
})
export class ViewServiceComponent implements OnInit {
  form!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      code: [''],
      name: [''],
      basePrice: [0],
      departmentId: [''],
      isActive: [false]
    });
    this.constantService.LoadData(this.data.element, this.form);
  }
}
