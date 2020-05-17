import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';


// This provides a protection if we want to warn the user if ther are any unsaved changes and he clicks elswhere other tab. Doesn't work
  // for closing browser
@Injectable()
export class PreventUnsavedChanges implements CanDeactivate<MemberEditComponent> {
    canDeactivate(component: MemberEditComponent) {
        if (component.editForm.dirty) {
            return confirm('Are you sure you want to continue ? Any changes will be lost');
        }
        return true;
    }
}
