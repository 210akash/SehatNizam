import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { TriageEndPoints } from './triage.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})
export class TriageService extends BaseService<any> {

    endPointControllerName = 'Triage';

    constructor(private http: HttpClient, httpClient: HttpClient, private triageEndPoints: TriageEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async saveTriage(triageForm: any) {
        return await this.post(triageForm, this.endPointControllerName + this.triageEndPoints.saveTriage)
            .pipe(map((data: any) => data));
    }

    async getAllTriage(triageFilterForm: any) {
        return await this.post(triageFilterForm, this.endPointControllerName + this.triageEndPoints.getAllTriage)
            .pipe(map((data: any) => data));
    }

    async deleteTriage(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.triageEndPoints.deleteTriage)
            .pipe(map((data: any) => data));
    }
}
