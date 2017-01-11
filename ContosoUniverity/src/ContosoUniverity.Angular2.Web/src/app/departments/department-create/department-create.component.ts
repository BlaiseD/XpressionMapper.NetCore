import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { Subscription } from 'rxjs/Subscription';

import { IDepartment } from '../idepartment';
import { IInstructor } from '../../instructors/iinstructor';
import { DepartmentService } from '../department.service';
import { InstructorService } from '../../instructors/instructor.service';

@Component({
  selector: 'app-department-create',
  templateUrl: './department-create.component.html',
  styleUrls: ['./department-create.component.css']
})
export class DepartmentCreateComponent implements OnInit {

  public title: string = 'Department';

  public department: IDepartment = {//must initialize the department for the form to be visible
    "departmentID":0,
    "name": null,
    "budget": null,
    "startDate": null,
    "instructorID":null,
    "rowVersion": null,
    "administratorName": null
  };
  public instructors: IInstructor[];
  errorMessage: string;
  //private sub: Subscription;

  constructor(private _route: ActivatedRoute,
    private _router: Router,
    private _departmentService: DepartmentService,
    private _instructorService: InstructorService) { }

  ngOnInit() {

    this.getInstructors();
  }

  ngOnDestroy() {
    //this.sub.unsubscribe();
  }

  getInstructors() {
    this._instructorService.getInstructors()
      .subscribe(instructors => this.instructors = instructors,
      error => this.errorMessage = <any>error);
  }

  onBack(): void {
    this._router.navigate(['/departments']);
  }

  submitClick(): void {
    this._departmentService.insertDepartment(this.department)
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
