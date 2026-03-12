import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';
import { EmployeeLeaveGroupEndPoints } from './employee-leave-group.endpoints';

@Injectable({
    providedIn: 'root'
})

export class EmployeeLeaveGroupService extends BaseService<any> {

    endPointControllerName = "EmployeeLeaveGroup";
    constructor(httpClient: HttpClient, private http: HttpClient, private employeeLeaveGroupEndPoints: EmployeeLeaveGroupEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllEmployeeLeaveGroups(employeeLeaveGroupsFilterForm: any) {
        return this.post(employeeLeaveGroupsFilterForm, this.endPointControllerName + this.employeeLeaveGroupEndPoints.getAllEmployeeLeaveGroups)
            .pipe(map((data: any) => data));
    }

    saveEmployeeLeaveGroup(saveEmployeeLeaveGroupCommand: any) {
        return this.post(saveEmployeeLeaveGroupCommand, this.endPointControllerName + this.employeeLeaveGroupEndPoints.saveEmployeeLeaveGroup)
            .pipe(map((data: any) => data));
    }

    saveGroupLeaveType(saveEmployeeLeaveGroupCommand: any) {
        return this.post(saveEmployeeLeaveGroupCommand, this.endPointControllerName + this.employeeLeaveGroupEndPoints.saveGroupLeaveType)
            .pipe(map((data: any) => data));
    }

    deleteEmployeeLeaveGroup(id: number) {
        return this.delete(id, this.endPointControllerName + this.employeeLeaveGroupEndPoints.deleteEmployeeLeaveGroup)
            .pipe();
    }

    getEmployeeLeaveGroupById(id: number) {
        return this.get(id, this.endPointControllerName + this.employeeLeaveGroupEndPoints.getEmployeeLeaveGroupById)
            .pipe(map((data: any) => data));
    }

    getEmployeeLeaveGroupByName(name: string) {
        return this.get(name, this.endPointControllerName + this.employeeLeaveGroupEndPoints.getEmployeeLeaveGroupByName)
            .pipe(map((data: any) => data));
    }
}
