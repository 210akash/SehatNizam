import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { PriorityEndPoints } from './priority.endpoints';

@Injectable({
    providedIn: 'root'
})

export class PriorityService extends BaseService<any> {

    endPointControllerName = "Priority";
    constructor(httpClient: HttpClient, private http: HttpClient, private priorityEndPoints: PriorityEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllPrioritys(prioritysFilterForm: any) {
        return this.post(prioritysFilterForm, this.endPointControllerName + this.priorityEndPoints.getAllPrioritys)
            .pipe(map((data: any) => data));
    }

    savePriority(savePriorityCommand: any) {
        return this.post(savePriorityCommand, this.endPointControllerName + this.priorityEndPoints.savePriority)
            .pipe(map((data: any) => data));
    }

    deletePriority(id: number) {
        return this.delete(id, this.endPointControllerName + this.priorityEndPoints.deletePriority)
            .pipe();
    }

    getPriorityById(id: number) {
        return this.get(id, this.endPointControllerName + this.priorityEndPoints.getPriorityById)
            .pipe(map((data: any) => data));
    }

    getPriorityByName(name: string) {
        return this.get(name, this.endPointControllerName + this.priorityEndPoints.getPriorityByName)
            .pipe(map((data: any) => data));
    }

    GetPriorityByCompany(companyId: number) {
        return this.get('?CompanyId=' + companyId, this.endPointControllerName + this.priorityEndPoints.getPriorityByCompany)
            .pipe(map((data: any) => data));
    }
}
