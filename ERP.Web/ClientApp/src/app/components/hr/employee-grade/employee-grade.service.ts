import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { EmployeeGradeEndPoints } from './employee-grade.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class EmployeeGradeService extends BaseService<any> {

    endPointControllerName = "EmployeeGrade";
    constructor(httpClient: HttpClient, private http: HttpClient, private employeeGradeEndPoints: EmployeeGradeEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllEmployeeGrades(employeeGradesFilterForm: any) {
        return this.post(employeeGradesFilterForm, this.endPointControllerName + this.employeeGradeEndPoints.getAllEmployeeGrades)
            .pipe(map((data: any) => data));
    }

    saveEmployeeGrade(saveEmployeeGradeCommand: any) {
        return this.post(saveEmployeeGradeCommand, this.endPointControllerName + this.employeeGradeEndPoints.saveEmployeeGrade)
            .pipe(map((data: any) => data));
    }

    deleteEmployeeGrade(id: number) {
        return this.delete(id, this.endPointControllerName + this.employeeGradeEndPoints.deleteEmployeeGrade)
            .pipe();
    }

    getEmployeeGradeById(id: number) {
        return this.get(id, this.endPointControllerName + this.employeeGradeEndPoints.getEmployeeGradeById)
            .pipe(map((data: any) => data));
    }

    getEmployeeGradeByName(name: string) {
        return this.get(name, this.endPointControllerName + this.employeeGradeEndPoints.getEmployeeGradeByName)
            .pipe(map((data: any) => data));
    }
}
