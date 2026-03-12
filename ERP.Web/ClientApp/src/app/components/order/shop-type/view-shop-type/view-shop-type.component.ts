import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-view-shop-type',
  templateUrl: './view-shop-type.component.html',
  styleUrls: ['./view-shop-type.component.css'],standalone: false
})

export class ViewShopTypeComponent implements OnInit {
  viewShopTypeForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  roleList: any;
  constructor(private dialog: MatDialog, private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.viewShopTypeForm = this.formBuilder.group({
      id: [0],
      name: [''],
    });

    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.viewShopTypeForm);
  }


}
