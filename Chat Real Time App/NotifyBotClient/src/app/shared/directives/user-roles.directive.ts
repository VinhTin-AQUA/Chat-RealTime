import {
  Directive,
  Input,
  ViewContainerRef,
  TemplateRef,
} from '@angular/core';

@Directive({
  selector: '[appUserRoles]',
})
export class UserRolesDirective {
  private userRoles: any;

  @Input() set appUserRoles(condition: string) {
    if(condition === 'Admin') {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainerRef.clear();
    }
  }

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainerRef: ViewContainerRef,
  ) {}
}
