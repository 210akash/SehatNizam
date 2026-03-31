import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { PriorityLevelEndPoints } from './prioritylevel.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class PriorityLevelService extends BaseService<any> {

    endPointControllerName = 'PriorityLevel';

    constructor(private http: HttpClient, httpClient: HttpClient, private priorityLevelEndPoints: PriorityLevelEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async savePriorityLevel(createPriorityLevelForm: any) {
        return await this.post(createPriorityLevelForm, this.endPointControllerName + this.priorityLevelEndPoints.savePriorityLevel)
            .pipe(map((data: any) => data));
    }

    async getAllPriorityLevel(priorityLevelFilterForm: any) {
        return await this.post(priorityLevelFilterForm, this.endPointControllerName + this.priorityLevelEndPoints.getAllPriorityLevel)
            .pipe(map((data: any) => data));
    }

    async deletePriorityLevel(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.priorityLevelEndPoints.deletePriorityLevel)
            .pipe(map((data: any) => data));
    }
}