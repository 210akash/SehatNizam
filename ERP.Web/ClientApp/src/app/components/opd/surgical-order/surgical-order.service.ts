import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';
import { SurgicalOrderEndPoints } from './surgical-order.endpoints';

@Injectable({
  providedIn: 'root'
})
export class SurgicalOrderService extends BaseService<any> {
  endPointControllerName = 'SurgicalOrder';

  constructor(httpClient: HttpClient, private endPoints: SurgicalOrderEndPoints) {
    super(httpClient, environment.dev_uri);
  }

  getAllSurgicalOrders(filter: any) {
    return this.post(filter, this.endPointControllerName + this.endPoints.getAllSurgicalOrders)
      .pipe(map((data: any) => data));
  }

  saveSurgicalOrder(payload: any) {
    return this.post(payload, this.endPointControllerName + this.endPoints.saveSurgicalOrder)
      .pipe(map((data: any) => data));
  }

  deleteSurgicalOrder(id: number) {
    return this.delete(id, this.endPointControllerName + this.endPoints.deleteSurgicalOrder);
  }
}
