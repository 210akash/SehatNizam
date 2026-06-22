import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../../Service/base.service';
import { environment } from '../../../../environments/environment';
import { ServiceEndPoints } from './service.endpoints';

@Injectable({
    providedIn: 'root'
})

export class ServiceService extends BaseService<any> {

    endPointControllerName = "Service";
    constructor(httpClient: HttpClient, private http: HttpClient, private ServiceEndPoints: ServiceEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllServices(query: { departmentId?: number; serviceTypeId?: number; name?: string; isSurgical?: boolean; pagingData?: any }) {
        return this.post(query, this.endPointControllerName + this.ServiceEndPoints.getAllService)
            .pipe(map((data: any) => data));
    }

    getServiceById(id: number) {
        return this.get(id, this.endPointControllerName + this.ServiceEndPoints.getServiceById)
            .pipe(map((data: any) => data));
    }

    saveService(command: { id?: number; code: string; name: string; basePrice: number; departmentId?: number | null }) {
        return this.post(command, this.endPointControllerName + this.ServiceEndPoints.saveService)
            .pipe(map((data: any) => data));
    }

    deleteService(id: number) {
        return this.delete(id, this.endPointControllerName + this.ServiceEndPoints.deleteService)
            .pipe();
    }

    getServiceCode() {
        return this.get(this.endPointControllerName + this.ServiceEndPoints.getCodeService)
            .pipe(map((data: any) => data));
    }

    getServiceName(name: string, departmentId?: number | null) {
        const params = departmentId != null ? `?name=${name}&departmentId=${departmentId}` : `?name=${name}`;
        return this.get(params, this.endPointControllerName + this.ServiceEndPoints.getServiceName)
            .pipe(map((data: any) => data));
    }
}
