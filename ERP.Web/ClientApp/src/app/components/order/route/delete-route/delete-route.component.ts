import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { RouteService } from '../route.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-delete-route',
  templateUrl: './delete-route.component.html',
  styleUrls: ['./delete-route.component.css'],standalone: false
})

export class DeleteRouteComponent implements OnInit {
  deleteRouteForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  roleList: any;

  gRoute: any;

  constructor(private notificationsService: NotificationsService, private dialog: MatDialog, private routeService: RouteService, private formBuilder: FormBuilder, private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.deleteRouteForm = this.formBuilder.group({
      id: [0],
      name: [''],
      zone: [''],
      territory: ['']
    });

    this.getRouteById(this.data.element?.id);
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.deleteRouteForm);
    this.deleteRouteForm.get('zone')?.patchValue(element.territory?.area?.zone.name);
    this.deleteRouteForm.get('territory')?.patchValue(element.territory?.name);
  }

  async delete() {
    (await this.routeService.deleteRoute(this.gRoute.element.id)).subscribe({
      next: (data: { Status: number; Message: string; }) => {
        if (data.Status == 200) {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Message, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Message, 'snack-bar-danger');
        }
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async getRouteById(routeId: any) {
    (await this.routeService.getRouteById(routeId)).subscribe({
      next: (data: any) => {
        this.gRoute = data;
        this.LoadData(data);
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }


}
