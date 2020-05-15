import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { MemberrListComponent } from './memberr-list/memberr-list.component';
import { AuthGuard } from './_guards/auth.guard';

// Routes work on first match , so the ordering is important.
export const appRoutes: Routes = [
    {
        path: '', component: HomeComponent // this is nothing path
    },
    {
        path: '', // dummy path , so child path will be '' + messages example and thing way with just one shot we can protect all
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [{
            path: 'members', component: MemberrListComponent
        },
        {
            path: 'messages', component: MessagesComponent
        },
        {
            path: 'lists', component: ListsComponent
        }]
    },
    {
        path: '**', redirectTo: '', pathMatch: 'full'
    }
];
