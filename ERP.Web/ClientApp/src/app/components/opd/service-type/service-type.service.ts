import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';
import { ServiceTypeEndPoints } from './service-type.endpoints';

@Injectable({
    providedIn: 'root'
})

export class ServiceTypeService extends BaseService<any> {

    endPointControllerName = "ServiceType";
    constructor(httpClient: HttpClient, private http: HttpClient, private ServiceTypeEndPoints: ServiceTypeEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllServiceTypes(query: { departmentId?: number }) {
        return this.post(query, this.endPointControllerName + this.ServiceTypeEndPoints.getAllServiceType)
            .pipe(map((data: any) => data));
    }

    saveServiceType(command: { id?: number; code: string; name: string; basePrice: number; departmentId?: number | null }) {
        return this.post(command, this.endPointControllerName + this.ServiceTypeEndPoints.saveServiceType)
            .pipe(map((data: any) => data));
    }

    deleteServiceType(id: number) {
        return this.delete(id, this.endPointControllerName + this.ServiceTypeEndPoints.deleteServiceType)
            .pipe();
    }
}
