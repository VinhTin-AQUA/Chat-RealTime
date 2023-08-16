import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { environment } from 'src/environments/environment';
import { SharedService } from '../shared/shared.service';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.scss'],
})
export class ContactComponent implements OnInit {
  formGroup: FormGroup = new FormGroup({});
  submitted: boolean = false;

  constructor(
    private http: HttpClient,
    private formBuilder: FormBuilder,
    private sharedService: SharedService
  ) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  private initializeForm() {
    this.formGroup = this.formBuilder.group({
      email: ['', [Validators.email, Validators.required]],
      title: ['', [Validators.required]],
      description: ['', [Validators.required]],
    });
  }

  sendContact() {
    this.submitted = true;

    if (this.formGroup.valid) {
      this.http
        .post(
          `${environment.appUrl}/contact/add-contact`,
          this.formGroup.value
        )
        .subscribe({
          next: (res: any) => {
            this.sharedService.showDialog(
              true,
              'Send contact successfully',
              res.value.message
            );
          },
          error: (err) => {
            this.sharedService.showDialog(false, 'Error', err.error);
          },
        });
    }
  }
}
