import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../../Service/base.service';
import { environment } from '../../../../environments/environment';
import { RadiologyTypeEndPoints } from './radiologytype.endpoints';

@Injectable({
    providedIn: 'root'
})

export class RadiologyTypeService extends BaseService<any> {

    endPointControllerName = "RadiologyType";
    constructor(httpClient: HttpClient, private http: HttpClient, private RadiologyTypeEndPoints: RadiologyTypeEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllRadiologyTypes(query: any) {
        return this.post(query, this.endPointControllerName + this.RadiologyTypeEndPoints.getAllRadiologyTypes)
            .pipe(map((data: any) => data));
    }

    getRadiologyTypeById(id: number) {
        return this.get(id, this.endPointControllerName + this.RadiologyTypeEndPoints.getRadiologyTypeById)
            .pipe(map((data: any) => data));
    }

    saveRadiologyType(command: { id?: number; name: string; serviceId: number }) {
        return this.post(command, this.endPointControllerName + this.RadiologyTypeEndPoints.saveRadiologyType)
            .pipe(map((data: any) => data));
    }

    deleteRadiologyType(id: number) {
        return this.delete(id, this.endPointControllerName + this.RadiologyTypeEndPoints.deleteRadiologyType)
            .pipe();
    }
}
