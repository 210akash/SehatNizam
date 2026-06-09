import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, map, of } from 'rxjs';
import { PatientEndPoints } from './patient.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class PatientService extends BaseService<any> {

    endPointControllerName = 'Patient';

    constructor(private http: HttpClient, httpClient: HttpClient, private visitTypeEndPoints: PatientEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async savePatient(createPatientForm: any) {
        return await this.post(createPatientForm, this.endPointControllerName + this.visitTypeEndPoints.savePatient)
            .pipe(map((data: any) => data));
    }

    async getAllPatient(visitTypeFilterForm: any) {
        return await this.post(visitTypeFilterForm, this.endPointControllerName + this.visitTypeEndPoints.getAllPatient)
            .pipe(map((data: any) => data));
    }

    getPatientByName(search: string) {
        if (typeof search !== 'string' || !search.trim()) {
            return of([]);
        }

        const query = '?search=' + encodeURIComponent(search.trim());
        return this.get(query, this.endPointControllerName + this.visitTypeEndPoints.getPatientByName)
            .pipe(
                map((data: any) => {
                    const list = data?.item1 ?? data?.Item1 ?? data?.data ?? data?.Data ?? data;
                    return Array.isArray(list) ? list : [];
                }),
                catchError(() => of([]))
            );
    }
}
