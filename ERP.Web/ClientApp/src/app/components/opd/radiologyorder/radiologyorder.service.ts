import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../../Service/base.service';
import { environment } from '../../../../environments/environment';
import { RadiologyOrderEndPoints } from './radiologyorder.endpoints';

@Injectable({
    providedIn: 'root'
})

export class RadiologyOrderService extends BaseService<any> {

    endPointControllerName = "RadiologyOrder";
    constructor(httpClient: HttpClient, private http: HttpClient, private RadiologyOrderEndPoints: RadiologyOrderEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    saveRadiologyOrder(command: { id?: number; appointmentId: number; radiologyTypeId: number; clinicalNotes?: string; statusId: number }) {
        return this.post(command, this.endPointControllerName + this.RadiologyOrderEndPoints.saveRadiologyOrder)
            .pipe(map((data: any) => data));
    }


    deleteRadiologyOrder(id: number) {
        return this.delete(id, this.endPointControllerName + this.RadiologyOrderEndPoints.deleteRadiologyOrder)
            .pipe();
    }

    
    getAllRadiologyOrders(filter: any) {
        return this.post(filter, this.endPointControllerName + this.RadiologyOrderEndPoints.getAllRadiologyOrders).pipe(map((data: any) => data));
    }

    getRadiologyOrderById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.RadiologyOrderEndPoints.getRadiologyOrderById).pipe(map((data: any) => data));
    }
    
saveRadiologyResult(payload: any) {
    return this.post(payload, this.endPointControllerName + this.RadiologyOrderEndPoints.saveRadiologyResult)
      .pipe(map((data: any) => data));
  }

  confirmRadiologyOrder(payload: any) {
    return this.post(payload, this.endPointControllerName + this.RadiologyOrderEndPoints.confirmRadiologyOrder)
      .pipe(map((data: any) => data));
  }
}
