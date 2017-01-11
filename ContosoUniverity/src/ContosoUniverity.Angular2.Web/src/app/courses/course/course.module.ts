import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule} from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CustomFormsModule } from 'ng2-validation'
import { CourseListComponent } from '../course-list/course-list.component';
import { CourseCreateComponent } from '../course-create/course-create.component';
import { CourseEditComponent } from '../course-edit/course-edit.component';
import { CourseDetailComponent } from '../course-detail/course-detail.component';
import { CourseDeleteComponent } from '../course-delete/course-delete.component';

@NgModule({
  imports: [
    CommonModule,
        RouterModule.forChild([
            { path: 'courses', component: CourseListComponent },
            { path: 'course-create', component: CourseCreateComponent },
            { path: 'course-edit/:id', component: CourseEditComponent },
            { path: 'course-delete/:id', component: CourseDeleteComponent },
            { path: 'course-detail/:id', component: CourseDetailComponent }
        ]),
        FormsModule,
        CustomFormsModule
  ],
  declarations: [
    CourseListComponent,
    CourseCreateComponent,
    CourseEditComponent,
    CourseDetailComponent,
    CourseDeleteComponent
  ]
})
export class CourseModule { }
