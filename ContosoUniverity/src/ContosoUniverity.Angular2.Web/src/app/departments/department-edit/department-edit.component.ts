import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { Subscription } from 'rxjs/Subscription';

import { IDepartment } from '../idepartment';
import { IInstructor } from '../../instructors/iinstructor';
import { DepartmentService } from '../department.service';
import { InstructorService } from '../../instructors/instructor.service';

@Component({
  selector: 'app-department-edit',
  templateUrl: './department-edit.component.html',
  styleUrls: ['./department-edit.component.css']
})
export class DepartmentEditComponent implements OnInit {
  public title: string = 'Department';
  public department: IDepartment;
  // public department: IDepartment = {//must initialize the department for the form to be visible
  //   "departmentID": 0,
  //   "name": "",
  //   "budget": 0,
  //   "startDate": new Date(),
  //   "instructorID": 0,
  //   "rowVersion": "",
  //   "administratorName": ""
  // };
  public instructors: IInstructor[];
  errorMessage: string;
  private sub: Subscription;

  constructor(private _route: ActivatedRoute,
    private _router: Router,
    private _departmentService: DepartmentService,
    private _instructorService: InstructorService) { }

  ngOnInit() {
    this.sub = this._route.params.subscribe(
      params => {
        let id = +params['id'];
        this.getDepartment(id);
      });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  getDepartment(id: number) {
    this._departmentService.getDepartment(id).subscribe(
      dept => this.department = dept,
      error => this.errorMessage = <any>error);

    this._instructorService.getInstructors()
      .subscribe(instructors => this.instructors = instructors,
      error => this.errorMessage = <any>error);
  }

  onBack(): void {
    this._router.navigate(['/departments']);
  }

  submitClick(): void {
    this._departmentService.updateDepartment(this.department)
      .subscribe(dept => {
        this._router.navigate(['/departments']);
      },
      error => this.errorMessage = <any>error);

  }

  set convertToDate(e) {
    var parts = e.split('-');

    if (parts.length > 2 && !isNaN(parseInt(parts[0])) && !isNaN(parseInt(parts[1])) && !isNaN(parseInt(parts[2]))) {
      let year = parseInt(parts[0]);
      let month = parseInt(parts[1]);
      let day = parseInt(parts[2]);

      this.department.startDate = new Date(year, month - 1, day);
    }
  }

  get convertToDate() {
    if (this.department.startDate) {
      return this.department.startDate.toString().substring(0, 10);
    }
    else
      return null;
  }
}
