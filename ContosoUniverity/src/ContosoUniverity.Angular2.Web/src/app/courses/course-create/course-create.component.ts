import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CustomFormsModule } from 'ng2-validation'

import { Subscription } from 'rxjs/Subscription';

import { ICourse } from '../icourse';
import { IDepartment } from '../../departments/idepartment';
import { CourseService } from '../course.service';
import { DepartmentService } from '../../departments/department.service';

@Component({
  selector: 'app-course-create',
  templateUrl: './course-create.component.html',
  styleUrls: ['./course-create.component.css']
})
export class CourseCreateComponent implements OnInit {

  public pageTitle: string = 'Course';
  public course: ICourse={
    "courseID": null,
    "title": null,
    "credits": null,
    "departmentID": null,
    "departmentName": null,
  };

  public departments: IDepartment[];
  errorMessage: string;

  constructor(private _route: ActivatedRoute,
    private _router: Router,
    private _courseService: CourseService,
    private _departmentService: DepartmentService) { }

  ngOnInit() {
    this.getDepartments();
  }

   ngOnDestroy() {
  }

  getDepartments() {
    this._departmentService.getDepartments()
      .subscribe(departments => this.departments = departments,
      error => this.errorMessage = <any>error);
  }

  onBack(): void {
    this._router.navigate(['/courses']);
  }

  submitClick(): void {
    this._courseService.insertCourse(this.course)
      .subscribe(dept => {
        this._router.navigate(['/courses']);
      },
      error => this.errorMessage = <any>error);

  }

}
