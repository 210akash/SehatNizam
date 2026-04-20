import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { RosterEndPoints } from './roster.endpoints';
import { BaseService } from '../../../Service/base.service';
import { environment } from '../../../../environments/environment';

@Injectable({
    providedIn: 'root'
})

export class RosterService extends BaseService<any> {

    endPointControllerName = "Roster";
    constructor(httpClient: HttpClient, private http: HttpClient, private rosterEndPoints: RosterEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllRosters(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.rosterEndPoints.getAllRosters)
            .pipe(map((data: any) => data));
    }

    async getAllRostersByManager(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.rosterEndPoints.getAllRostersByManager)
            .pipe(map((data: any) => data));
    }

    async getAllRostersByEmployee(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.rosterEndPoints.getAllRostersByEmployee)
            .pipe(map((data: any) => data));
    }

    saveRoster(saveRosterCommand: any) {
        return this.post(saveRosterCommand, this.endPointControllerName + this.rosterEndPoints.saveRoster)
            .pipe(map((data: any) => data));
    }

    saveRosterByManager(saveRosterCommand: any) {
        return this.post(saveRosterCommand, this.endPointControllerName + this.rosterEndPoints.saveRosterByManager)
            .pipe(map((data: any) => data));
    }

    deleteRoster(id: number) {
        return this.delete(id, this.endPointControllerName + this.rosterEndPoints.deleteRoster)
            .pipe();
    }

    getRosterById(id: number) {
        return this.get(id, this.endPointControllerName + this.rosterEndPoints.getRosterById)
            .pipe(map((data: any) => data));
    }

    getRosterCode() {
        return this.get(this.endPointControllerName + this.rosterEndPoints.getRosterCode)
            .pipe(map((data: any) => data));
    }

    processRoster(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.rosterEndPoints.processRoster)
            .pipe();
    }

    getRosterCount(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.rosterEndPoints.getRosterCount)
            .pipe(map((data: any) => data));
    }

    approveRoster(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.rosterEndPoints.approveRoster)
            .pipe();
    }

    rejectRoster(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.rosterEndPoints.rejectRoster)
            .pipe();
    }
}
