export interface UpdateUser {
  email: string;
  firstName: string;
  lastName: string;
  
  oldPassword: string;
  newPassword: string;
  reEnterPassword: string;
}
