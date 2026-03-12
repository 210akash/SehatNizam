import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { EmployeeLeaveEndPoints } from './employee-leave.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class EmployeeLeaveService extends BaseService<any> {

    endPointControllerName = "EmployeeLeave";
    constructor(httpClient: HttpClient, private http: HttpClient, private employeeLeaveEndPoints: EmployeeLeaveEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllEmployeeLeaves(employeeLeavesFilterForm: any) {
        return this.post(employeeLeavesFilterForm, this.endPointControllerName + this.employeeLeaveEndPoints.getAllEmployeeLeaves)
            .pipe(map((data: any) => data));
    }

    getAllDepartmentLeaves(employeeLeavesFilterForm: any) {
        return this.post(employeeLeavesFilterForm, this.endPointControllerName + this.employeeLeaveEndPoints.getAllDepartmentLeaves)
            .pipe(map((data: any) => data));
    }

    saveEmployeeLeave(saveEmployeeLeaveCommand: any) {
        return this.post(saveEmployeeLeaveCommand, this.endPointControllerName + this.employeeLeaveEndPoints.saveEmployeeLeave)
            .pipe(map((data: any) => data));
    }

    deleteEmployeeLeave(id: number) {
        return this.delete(id, this.endPointControllerName + this.employeeLeaveEndPoints.deleteEmployeeLeave)
            .pipe();
    }

    getEmployeeLeaveById(id: number) {
        return this.get(id, this.endPointControllerName + this.employeeLeaveEndPoints.getEmployeeLeaveById)
            .pipe(map((data: any) => data));
    }

    getEmployeeLeaveByName(name: string) {
        return this.get(name, this.endPointControllerName + this.employeeLeaveEndPoints.getEmployeeLeaveByName)
            .pipe(map((data: any) => data));
    }

    getEmployeeLeaveBalance() {
        return this.get(this.endPointControllerName + this.employeeLeaveEndPoints.getEmployeeLeaveBalance)
            .pipe(map((data: any) => data));
    }

    processEmployeeLeave(_payload: any) {
        return this.post(_payload, this.endPointControllerName + this.employeeLeaveEndPoints.processEmployeeLeave)
            .pipe(map((data: any) => data));
    }

    approveEmployeeLeave(_payload: any) {
        return this.post(_payload, this.endPointControllerName + this.employeeLeaveEndPoints.approveEmployeeLeave)
            .pipe(map((data: any) => data));
    }

    rejectEmployeeLeave(_payload: any) {
        return this.post(_payload, this.endPointControllerName + this.employeeLeaveEndPoints.rejectEmployeeLeave)
            .pipe(map((data: any) => data));
    }

    getSingleEmployeeLeaves(employeeLeavesFilterForm: any) {
        return this.post(employeeLeavesFilterForm, this.endPointControllerName + this.employeeLeaveEndPoints.getSingleEmployeeLeaves)
            .pipe(map((data: any) => data));
    }

    getLeaveBalanceByEmployee(employeeId: any) {
        return this.get('?EmployeeId=' + employeeId, this.endPointControllerName + this.employeeLeaveEndPoints.getLeaveBalanceByEmployee)
            .pipe(map((data: any) => data));
    }

    managerApproveLeave(_payload: any) {
        return this.post(_payload, this.endPointControllerName + this.employeeLeaveEndPoints.managerApproveLeave)
            .pipe(map((data: any) => data));
    }

   saveEmployeeLeaveByHr(saveEmployeeLeaveCommand: any) {
        return this.post(saveEmployeeLeaveCommand, this.endPointControllerName + this.employeeLeaveEndPoints.saveEmployeeLeaveByHr)
            .pipe(map((data: any) => data));
    }
}