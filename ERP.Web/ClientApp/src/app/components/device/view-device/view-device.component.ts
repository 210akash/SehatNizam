import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConstantService } from '../../../Service/constant.service';

@Component({
  selector: 'app-view-device',
  templateUrl: './view-device.component.html',
  styleUrls: ['./view-device.component.css'],
  standalone: false,
})
export class ViewDeviceComponent implements OnInit {
  viewDeviceForm!: FormGroup;
  isLoading = false;
  dialogRef: any;

  constructor(
    private formBuilder: FormBuilder,
    private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) {}

  ngOnInit(): void {
    this.viewDeviceForm = this.formBuilder.group({
        id: [0],
         name: ['', Validators.required],
         phoneNo: [''],
         address: [''],
         iPAddress: ['', Validators.required],
         port: [0, Validators.required],
         isActive: [true, Validators.required],
    });

    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.viewDeviceForm);
  }
}
