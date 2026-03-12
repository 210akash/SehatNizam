import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { EmployeeWorkSiteTypeEndPoints } from './employee-worksitetype.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class EmployeeWorkSiteTypeService extends BaseService<any> {

    endPointControllerName = "EmployeeWorkSiteType";
    constructor(httpClient: HttpClient, private http: HttpClient, private employeeWorkSiteTypeEndPoints: EmployeeWorkSiteTypeEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllEmployeeWorkSiteTypes(employeeWorkSiteTypesFilterForm: any) {
        return this.post(employeeWorkSiteTypesFilterForm, this.endPointControllerName + this.employeeWorkSiteTypeEndPoints.getAllEmployeeWorkSiteTypes)
            .pipe(map((data: any) => data));
    }

    saveEmployeeWorkSiteType(saveEmployeeWorkSiteTypeCommand: any) {
        return this.post(saveEmployeeWorkSiteTypeCommand, this.endPointControllerName + this.employeeWorkSiteTypeEndPoints.saveEmployeeWorkSiteType)
            .pipe(map((data: any) => data));
    }

    deleteEmployeeWorkSiteType(id: number) {
        return this.delete(id, this.endPointControllerName + this.employeeWorkSiteTypeEndPoints.deleteEmployeeWorkSiteType)
            .pipe();
    }

    getEmployeeWorkSiteTypeById(id: number) {
        return this.get(id, this.endPointControllerName + this.employeeWorkSiteTypeEndPoints.getEmployeeWorkSiteTypeById)
            .pipe(map((data: any) => data));
    }

    getEmployeeWorkSiteTypeByName(name: string) {
        return this.get(name, this.endPointControllerName + this.employeeWorkSiteTypeEndPoints.getEmployeeWorkSiteTypeByName)
            .pipe(map((data: any) => data));
    }
}
