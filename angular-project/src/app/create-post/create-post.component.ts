import {Component, inject, Inject} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatCardModule} from '@angular/material/card';
import {HttpParams} from "@angular/common/http";
import { HttpClient } from '@angular/common/http';
import {takeUntilDestroyed} from "@angular/core/rxjs-interop";
import {AuthenticationService} from "../api-authorization/authentication.service";
import {DestroyRef} from "@angular/core";
import {MatSnackBarModule} from '@angular/material/snack-bar';


@Component({
  selector: 'app-create-post',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatSnackBarModule,

  ],
  templateUrl: './create-post.component.html',
  styleUrl: './create-post.component.css'
})
export class CreatePostComponent {
  public CreatePostForm: FormGroup;
  private httpClient = inject(HttpClient);
  authService = inject(AuthenticationService);
  destroyRef = inject(DestroyRef);
  private snackBar: MatSnackBarModule;
  snackBarMessage: string = '';


  constructor(private formBuilder: FormBuilder, @Inject('BASE_URL') private baseUrl: string) {
    this.CreatePostForm = this.formBuilder.group({
      title: ['', [Validators.required, Validators.maxLength(50)]],
      description: ['', [Validators.required, Validators.maxLength(500)]],
      code: ['']
    });



  }
  submitPost() {
    if (this.CreatePostForm.valid) {
      const postData = {
        title: this.CreatePostForm.value.title,
        description: this.CreatePostForm.value.description,
        code: this.CreatePostForm.value.code,
        authorId: this.authService.getCurrentId()
      };
      console.log(postData);
      this.httpClient.post(`${this.baseUrl}/forum/newPost`, postData)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe(() => {
          this.CreatePostForm.reset();
        }, (error) => {
          console.error('Error creating post:', error);
        });
    }
    }

  }

