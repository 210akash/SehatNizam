import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { VisitTypeEndPoints } from './visit-type.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class VisitTypeService extends BaseService<any> {

    endPointControllerName = 'VisitType';

    constructor(private http: HttpClient, httpClient: HttpClient, private visitTypeEndPoints: VisitTypeEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async saveVisitType(createVisitTypeForm: any) {
        return await this.post(createVisitTypeForm, this.endPointControllerName + this.visitTypeEndPoints.saveVisitType)
            .pipe(map((data: any) => data));
    }

    async getAllVisitType(visitTypeFilterForm: any) {
        return await this.post(visitTypeFilterForm, this.endPointControllerName + this.visitTypeEndPoints.getAllVisitType)
            .pipe(map((data: any) => data));
    }

    async deleteVisitType(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.visitTypeEndPoints.deleteVisitType)
            .pipe(map((data: any) => data));
    }
}