import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-view-lab-order-type',
  templateUrl: './view-lab-order-type.component.html',
  styleUrls: ['./view-lab-order-type.component.css'],
  standalone: false
})
export class ViewLabOrderTypeComponent implements OnInit {
  form!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      name: [''],
      description: [''],
      serviceId: [''],
      customFieldsSchema: ['']
    });
    this.constantService.LoadData(this.data.element, this.form);
  }
}
