export interface UserView {
  index: number;
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  dateCreated: Date;
  isLockout: boolean;
  roles: string;
}
