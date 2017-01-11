import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { StudentModule } from './students/student/student.module';
import { DepartmentModule } from './departments/department/department.module';
import { CourseModule } from './courses/course/course.module';
import { WelcomeComponent } from './welcome/welcome.component';
import { AboutComponent } from './about/about.component';
import { AboutService } from './about/about.service';
import { DepartmentService } from './departments/department.service';
import { StudentService } from './students/student.service';
import { CourseService } from './courses/course.service';
import { InstructorService } from './instructors/instructor.service';

@NgModule({
  declarations: [
    AppComponent,
    WelcomeComponent,
    AboutComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpModule,
    RouterModule.forRoot([
      { path: 'welcome', component: WelcomeComponent },
      { path: 'about', component: AboutComponent },
      { path: '', redirectTo: 'welcome', pathMatch: 'full' },
      { path: '**', redirectTo: 'welcome', pathMatch: 'full' }
    ]),
    StudentModule,
    DepartmentModule,
    CourseModule
  ],
  providers: [AboutService, DepartmentService, CourseService, InstructorService, StudentService],
  bootstrap: [AppComponent]
})
export class AppModule { }
