import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { ProjectEndPoints } from './project.endpoints';

@Injectable({
    providedIn: 'root'
})

export class ProjectService extends BaseService<any> {

    endPointControllerName = "Project";
    constructor(httpClient: HttpClient, private http: HttpClient, private projectEndPoints: ProjectEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllProjects(projectsFilterForm: any) {
        return this.post(projectsFilterForm, this.endPointControllerName + this.projectEndPoints.getAllProjects)
            .pipe(map((data: any) => data));
    }

    saveProject(saveProjectCommand: any) {
        return this.post(saveProjectCommand, this.endPointControllerName + this.projectEndPoints.saveProject)
            .pipe(map((data: any) => data));
    }

    deleteProject(id: number) {
        return this.delete(id, this.endPointControllerName + this.projectEndPoints.deleteProject)
            .pipe();
    }

    getProjectById(id: number) {
        return this.get(id, this.endPointControllerName + this.projectEndPoints.getProjectById)
            .pipe(map((data: any) => data));
    }

    getProjectByName(name: string) {
        return this.get(name, this.endPointControllerName + this.projectEndPoints.getProjectByName)
            .pipe(map((data: any) => data));
    }

    getProjectByCompany(companyId: string) {
        return this.get('?CompanyId=' + companyId, this.endPointControllerName + this.projectEndPoints.getProjectByCompany)
            .pipe(map((data: any) => data));
    }
}
