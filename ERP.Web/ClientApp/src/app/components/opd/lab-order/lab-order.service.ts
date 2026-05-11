import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';
import { LabOrderEndPoints } from './lab-order.endpoints';

@Injectable({
    providedIn: 'root'
})
export class LabOrderService extends BaseService<any> {
    endPointControllerName = 'LabOrder';
    constructor(httpClient: HttpClient, private endPoints: LabOrderEndPoints) {
        super(httpClient, environment.dev_uri);
    }

    async saveLabOrder(payload: any) {
        return await this.post(payload, this.endPointControllerName + this.endPoints.saveLabOrder)
            .pipe(map((data: any) => data));
    }

    getAllLabOrders(filter: any) {
        return this.post(filter, this.endPointControllerName + this.endPoints.getAllLabOrders).pipe(map((data: any) => data));
    }

    getLabOrderById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.endPoints.getLabOrderById).pipe(map((data: any) => data));
    }

    async deleteLabOrder(id: number) {
        return this.delete(id, this.endPointControllerName + this.endPoints.deleteLabOrder)
            .pipe();
    }
}
