import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';
import { PrescriptionEndPoints } from './prescription.endpoints';

@Injectable({
    providedIn: 'root'
})
export class PrescriptionService extends BaseService<any> {

    endPointControllerName = 'Prescription';

    constructor(private http: HttpClient, httpClient: HttpClient, private prescriptionEndPoints: PrescriptionEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async savePrescription(prescriptionForm: any) {
        return await this.post(prescriptionForm, this.endPointControllerName + this.prescriptionEndPoints.savePrescription)
            .pipe(map((data: any) => data));
    }
}
