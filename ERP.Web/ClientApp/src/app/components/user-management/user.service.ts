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

    getAllUsers() {
        return this.get(this.endPointControllerName + this.userEndPoints.getAll)
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

    async changeUserPassword(changePasswordCommand: any) {
        return await this.post(changePasswordCommand, this.endPointControllerName + this.userEndPoints.changePassword)
            .pipe(map((data: any) => data));
    }

    async saveRole(roleCommand: string) {
        return await this.post(roleCommand, this.endPointControllerName + this.userEndPoints.addRole)
            .pipe(map((data: any) => data));
    }


}