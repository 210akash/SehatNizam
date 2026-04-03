import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';
import { DoctorEndPoints } from './doctor.endpoints';

@Injectable({
  providedIn: 'root'
})
export class DoctorService extends BaseService<any> {
  endPointControllerName = 'Doctor';

  constructor(
    private http: HttpClient,
    httpClient: HttpClient,
    private doctorEndPoints: DoctorEndPoints
  ) {
    super(httpClient, environment.dev_uri);
  }

  async getAllDoctors(filterForm: any) {
    return await this.post(
      filterForm,
      this.endPointControllerName + this.doctorEndPoints.getAllDoctors
    ).pipe(map((data: any) => data));
  }

   getDoctorByName(employeeFilterForm: any) {
        return this.post(employeeFilterForm, this.endPointControllerName + this.doctorEndPoints.getDoctorByName)
            .pipe(map((data: any) => data));
    }
}
