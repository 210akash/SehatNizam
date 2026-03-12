import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserEndPoints } from './user.endpoints';
import { map } from 'rxjs';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class UserService extends BaseService<any> {

    endPointControllerName = 'Auth';

    constructor(private http: HttpClient, httpClient: HttpClient, private userEndPoints: UserEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async register(_userFilterForm: string) {
        return await this.post(_userFilterForm, this.endPointControllerName + this.userEndPoints.register)
            .pipe(map((data: any) => data));
    }

    getAllUsers(userFilterForm: any) {
        return this.post(userFilterForm, this.endPointControllerName + this.userEndPoints.getAll)
            .pipe(map((data: any) => data));
    }

    updateUser(_userFilterForm: string) {
        return this.post(_userFilterForm, this.endPointControllerName + this.userEndPoints.updateUser)
            .pipe(map((data: any) => data));
    }

    getAllRoles() {
        return this.get(this.endPointControllerName + this.userEndPoints.getAllRoles)
            .pipe(map((data: any) => data));
    }

    getAllRolesByDepartment(departmentId: number) {
        return this.get('?departmentId=' + departmentId, this.endPointControllerName + this.userEndPoints.getAllRolesByDepartment)
            .pipe(map((data: any) => data));
    }
    async changeUserPassword(changePasswordCommand: any) {
        return await this.post(changePasswordCommand, this.endPointControllerName + this.userEndPoints.changePassword)
            .pipe(map((data: any) => data));
    }

    async saveRole(roleCommand: string) {
        return await this.post(roleCommand, this.endPointControllerName + this.userEndPoints.saveRole)
            .pipe(map((data: any) => data));
    }


    getAllUsersByRole(role: string) {
        return this.get('?role=' + role, this.endPointControllerName + this.userEndPoints.getAllByRole)
            .pipe(map((data: any) => data));
    }

    getAllSaleUsers(userFilterForm: any) {
        return this.post(userFilterForm, this.endPointControllerName + this.userEndPoints.getAllSaleUsers)
            .pipe(map((data: any) => data));
    }

        getById(userId: any) {
        return this.get('?userId=' + userId, this.endPointControllerName + this.userEndPoints.getById)
            .pipe(map((data: any) => data));
    }

         getByName(name: any) {
        return this.get('?name=' + name, this.endPointControllerName + this.userEndPoints.getByName)
            .pipe(map((data: any) => data));
    }

      registerMobileDevice(_userFilterForm: string) {
        return this.post(_userFilterForm, this.endPointControllerName + this.userEndPoints.registerMobileDevice)
            .pipe(map((data: any) => data));
    }
}