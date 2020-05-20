import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { MemberrListComponent } from './members/memberr-list/memberr-list.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';
import { ListsResolver } from './_resolvers/lists.resolver';

// Routes work on first match , so the ordering is important.
export const appRoutes: Routes = [
    {
        path: '', component: HomeComponent // this is nothing path or wildcard path
    },
    {
        path: '', // dummy path , so child path will be '' + messages example and thing way with just one shot we can protect all
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [{
            path: 'members', component: MemberrListComponent , resolve: {
                users: MemberListResolver
            }
        },
        {
            path: 'members/:id', component: MemberDetailComponent, resolve: {
                user: MemberDetailResolver
            }
        },
        {
            path: 'member/edit', component: MemberEditComponent, resolve: {
                user: MemberEditResolver
            },
            canDeactivate: [PreventUnsavedChanges]
        },
        {
            path: 'messages', component: MessagesComponent
        },
        {
            path: 'lists', component: ListsComponent, resolve: {
                users: ListsResolver
            }
        }]
    },
    {
        path: '**', redirectTo: '', pathMatch: 'full'
    }
];
