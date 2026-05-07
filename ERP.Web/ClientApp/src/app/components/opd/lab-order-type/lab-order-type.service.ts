import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';
import { LabOrderTypeEndPoints } from './lab-order-type.endpoints';

@Injectable({
    providedIn: 'root'
})
export class LabOrderTypeService extends BaseService<any> {
    endPointControllerName = 'LabOrderType';

    constructor(httpClient: HttpClient, private endPoints: LabOrderTypeEndPoints) {
        super(httpClient, environment.dev_uri);
    }

    getAllLabOrderTypes(filter: any) {
        return this.post(filter, this.endPointControllerName + this.endPoints.getAllLabOrderTypes)
            .pipe(map((data: any) => data));
    }

    getLabOrderTypeById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.endPoints.getLabOrderTypeById)
            .pipe(map((data: any) => data));
    }

    saveLabOrderType(payload: any) {
        return this.post(payload, this.endPointControllerName + this.endPoints.saveLabOrderType)
            .pipe(map((data: any) => data));
    }

    deleteLabOrderType(id: number) {
        return this.delete(id, this.endPointControllerName + this.endPoints.deleteLabOrderType)
            .pipe();
    }
}
