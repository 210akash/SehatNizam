import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { InterviewService } from '../interview.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { AuthenticationService } from '../../../Auth/authentication.service';

@Component({
  selector: 'app-add-comments',
  templateUrl: './add-comments.component.html',
  styleUrl: './add-comments.component.css',
  standalone: false
})

export class AddCommentsComponent {
  interviewForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  interviewAttendeesList: any;

  currentPage = 0;
  pageSize = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['interviewDate', 'attendees', 'joinAfterDays', 'comments', 'status', 'createdDate', 'createdBy'];
  dataSource: any;
  take = 50;
  totalRows = 0;

  userList: any[] = [];
  selectedUsers: any[] = [];

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private authenticationService: AuthenticationService,
    private interviewService: InterviewService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.dataSource = this.data.element?.interviewHistory;
    this.interviewForm = this.formBuilder.group({
      id: [0],
      interviewId: [this.data.element.id, Validators.required],
      interviewDate: ['', Validators.required],
      joinAfterDays: ['', Validators.required],
      comments: ['', Validators.required],
      statusId: ['', Validators.required],
      // interviewAttendees: ['', Validators.required],
      user: [''],
    });

    this.getInterviewAttendees();
  }

  get f() { return this.interviewForm.controls; }

  SaveData() {
    if (this.interviewForm.invalid || (!this.selectedUsers.length && this.interviewForm.get('statusId')?.value == 2)) {
      this.constantService.markFormGroupTouched(this.interviewForm);
      return;
    }

    this.isLoading = true;
    let _interviewForm: any = {};
    _interviewForm = Object.assign(_interviewForm, this.interviewForm.value);

    _interviewForm['interviewAttendees'] = this.selectedUsers.map(x => x.id);

    this.interviewService.addComments(_interviewForm).subscribe({
      next: (data) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else
          this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        console.error(error);
        this.isLoading = false;
      }
    });
  }

  getInterviewAttendees(): void {
    this.interviewService.getInterviewAttendees().subscribe(data => {
      this.interviewAttendeesList = data;
    });
  }

  updateValidity() {
    let statusId = this.interviewForm.get('statusId')?.value;

    if (statusId === 2) {
      this.interviewForm.get('interviewDate')?.setValidators(Validators.required);
      this.interviewForm.get('joinAfterDays')?.setValidators(Validators.required);
      this.interviewForm.get('interviewAttendees')?.setValidators(Validators.required);
    }
    else if (statusId === 3 || statusId === 180 || statusId === 4) {
      this.interviewForm.get('interviewDate')?.clearValidators();
      this.interviewForm.get('joinAfterDays')?.clearValidators();
      this.interviewForm.get('interviewAttendees')?.clearValidators();
    }

    this.interviewForm.get('interviewDate')?.updateValueAndValidity();
    this.interviewForm.get('joinAfterDays')?.updateValueAndValidity();
    this.interviewForm.get('interviewAttendees')?.updateValueAndValidity();
  }

  getAttendeesString(element: any) {
    return element.interviewAttendees
      .map((attendee: any) => {
        const user = attendee.aspNetUsers;
        return user ? `${user.firstName} ${user.lastName}` : null;
      })
      .filter((name: string | null) => !!name)
      .join(', ');

    return '';
  }

  async searchUser(event: any) {
    const filter = event.currentTarget.value;
    this.userList = [];

    if (!filter) {
      return;
    }

    (await this.authenticationService.getByName(filter)).subscribe(
      (data: any) => {
        this.userList = data || [];
      },
      (error: any) => {
        console.error('Error fetching list:', error);
        this.userList = [];
      }
    );
  }

  onUserInputCleared(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    console.log('Current Input Value:', inputValue); // Debugging output

    if (!inputValue.trim()) {
      console.log(`Input cleared!`);
      this.interviewForm.get('user')?.patchValue('');
    }
  }

  onUserOptionSelected(event: MatAutocompleteSelectedEvent): void {

    const selectedValue = event.option.value;

    if (!this.selectedUsers.some(user => user.id === selectedValue.id)) {
      this.selectedUsers.push(selectedValue);
    }

    if (!selectedValue) {
      console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');
      return;
    }

    const inputElement = document.querySelector('input[formControlName="plot"]') as HTMLInputElement;
    if (inputElement) {
      inputElement.value = '';  // Clear the input field
    }
    this.interviewForm.get('user')?.setValue('');
    this.userList = [];
  }

  removeUser(user: any) {
    const index = this.selectedUsers.indexOf(user);
    if (index >= 0) {
      this.selectedUsers.splice(index, 1);
    }
  }


}