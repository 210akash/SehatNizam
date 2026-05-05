import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { PatientProblemEndPoints } from './patientproblem.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})
export class PatientProblemService extends BaseService<any> {

    endPointControllerName = 'PatientProblem';

    constructor(private http: HttpClient, httpClient: HttpClient, private patientproblemEndPoints: PatientProblemEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async savePatientProblem(patientproblemForm: any) {
        return await this.post(patientproblemForm, this.endPointControllerName + this.patientproblemEndPoints.savePatientProblem)
            .pipe(map((data: any) => data));
    }
}
