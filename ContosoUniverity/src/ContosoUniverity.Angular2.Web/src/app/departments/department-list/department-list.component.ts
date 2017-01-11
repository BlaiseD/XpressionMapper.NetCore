import { Component, OnInit } from '@angular/core';

import { IDepartment } from '../idepartment'
import { DepartmentService } from '../department.service';

@Component({
  selector: 'app-department-list',
  templateUrl: './department-list.component.html',
  styleUrls: ['./department-list.component.css']
})
export class DepartmentListComponent implements OnInit {

  constructor(private _departmentService: DepartmentService) { }

  public title: string = 'Departments';

  errorMessage: string;
  departments: IDepartment[];

  ngOnInit() {
    this._departmentService.getDepartments()
            .subscribe(departments => this.departments = departments,
                           error => this.errorMessage = <any>error);
  }

}
