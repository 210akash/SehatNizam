import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';
import { PriorityLevelEndPoints } from './candidatescoringscale.endpoints';

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

    async getAllCandidateScoringScales() {
        return this.get(this.endPointControllerName + this.priorityLevelEndPoints.getAllCandidateScoringScales)
            .pipe(map((data: any) => data));
    }
}