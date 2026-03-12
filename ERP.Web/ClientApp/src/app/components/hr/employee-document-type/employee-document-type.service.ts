import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { EmployeeDocumentTypeEndPoints } from './employee-document-type.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class EmployeeDocumentTypeService extends BaseService<any> {

    endPointControllerName = "EmployeeDocumentType";
    constructor(httpClient: HttpClient, private http: HttpClient, private employeeDocumentTypeEndPoints: EmployeeDocumentTypeEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllEmployeeDocumentTypes(employeeDocumentTypesFilterForm: any) {
        return this.post(employeeDocumentTypesFilterForm, this.endPointControllerName + this.employeeDocumentTypeEndPoints.getAllEmployeeDocumentTypes)
            .pipe(map((data: any) => data));
    }

    saveEmployeeDocumentType(saveEmployeeDocumentTypeCommand: any) {
        return this.post(saveEmployeeDocumentTypeCommand, this.endPointControllerName + this.employeeDocumentTypeEndPoints.saveEmployeeDocumentType)
            .pipe(map((data: any) => data));
    }

    deleteEmployeeDocumentType(id: number) {
        return this.delete(id, this.endPointControllerName + this.employeeDocumentTypeEndPoints.deleteEmployeeDocumentType)
            .pipe();
    }

    getEmployeeDocumentTypeById(id: number) {
        return this.get(id, this.endPointControllerName + this.employeeDocumentTypeEndPoints.getEmployeeDocumentTypeById)
            .pipe(map((data: any) => data));
    }

    getEmployeeDocumentTypeByName(name: string) {
        return this.get(name, this.endPointControllerName + this.employeeDocumentTypeEndPoints.getEmployeeDocumentTypeByName)
            .pipe(map((data: any) => data));
    }

    getEmployeeDocumentByEmployeeId(employeeId: any) {
        return this.get('?employeeId=' + employeeId, this.endPointControllerName + this.employeeDocumentTypeEndPoints.getEmployeeDocumentByEmployeeId)
            .pipe(map((data: any) => data));
    }

    
}
