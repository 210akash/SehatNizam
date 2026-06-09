import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';
import { AdmissionServiceEndPoints } from './admissionservice.endpoints';

@Injectable({
    providedIn: 'root'
})
export class AdmissionServiceService extends BaseService<any>{

    endPointControllerName = "AdmissionService";
    constructor(httpClient: HttpClient, private http: HttpClient, private admissionServiceEndPoints: AdmissionServiceEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllAdmissionServices(AdmissionServiceFilterForm: any) {
        return this.post(AdmissionServiceFilterForm, this.endPointControllerName + this.admissionServiceEndPoints.getAllAdmissionServices)
            .pipe(map((data: any) => data));
    }

    saveAdmissionService(AdmissionServiceFilterForm: any) {
        return this.post(AdmissionServiceFilterForm, this.endPointControllerName + this.admissionServiceEndPoints.saveAdmissionService)
            .pipe(map((data: any) => data));
    }

    deleteAdmissionService(id: number) {
      return this.delete(id, this.endPointControllerName + this.admissionServiceEndPoints.deleteAdmissionService)
          .pipe();
    }
}
