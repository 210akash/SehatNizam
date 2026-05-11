import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../../Service/base.service';
import { environment } from '../../../../environments/environment';
import { LabOrderTypeEndPoints } from './lab-order-type.endpoints';

@Injectable({
    providedIn: 'root'
})

export class LabOrderTypeService extends BaseService<any> {

    endPointControllerName = 'LabOrderType';
    constructor(httpClient: HttpClient, private http: HttpClient, private endPoints: LabOrderTypeEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllLabOrderTypes(query: any) {
        return this.post(query, this.endPointControllerName + this.endPoints.getAllLabOrderTypes)
            .pipe(map((data: any) => data));
    }

    getLabOrderTypeById(id: number) {
        return this.get(id, this.endPointControllerName + this.endPoints.getLabOrderTypeById)
            .pipe(map((data: any) => data));
    }

    saveLabOrderType(command: { id?: number; name: string }) {
        return this.post(command, this.endPointControllerName + this.endPoints.saveLabOrderType)
            .pipe(map((data: any) => data));
    }

    deleteLabOrderType(id: number) {
        return this.delete(id, this.endPointControllerName + this.endPoints.deleteLabOrderType)
            .pipe();
    }
}
