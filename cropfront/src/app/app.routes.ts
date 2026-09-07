import { Routes } from '@angular/router';
import { SignupComponent } from '../components/signup/signup.component';
import { LoginComponent } from '../components/login/login.component';
import { UsersComponent } from '../components/users/users.component';
import { CroplistingsComponent } from '../components/croplistings/croplistings.component';
import { AdmindashboardComponent } from '../components/admindashboard/admindashboard.component';
import { FarmerdashboardComponent } from '../components/farmerdashboard/farmerdashboard.component';
import { DealerdashboardComponent } from '../components/dealerdashboard/dealerdashboard.component';
import { CropComponent } from '../components/crop/crop.component';
import { AddressComponent } from '../components/address/address.component';
import { roleGuard } from './guards/role.guard';
import { UpdateprofileComponent } from '../components/updateprofile/updateprofile.component';
import { HomeComponent } from '../components/home/home.component';
import { TransactionComponent } from '../components/transaction/transaction.component';
import { RatingsComponent } from '../components/ratings/ratings.component';
import { ReportComponent } from '../components/report/report.component';
import { BankComponent } from '../components/bank/bank.component';
import { SubscriptionComponent } from '../components/subscription/subscription.component';
import { AuthCallbackComponent } from '../components/auth-callback/auth-callback.component';
import { NegotiationsComponent } from '../components/negotiations/negotiations.component';
import { PickupComponent } from '../components/pickup/pickup.component';

export const routes: Routes = [
    {
        path: 'home',
        component: HomeComponent
    },
    {
        path: 'signup',
        component: SignupComponent
    },
    {
        path: 'login',
        component: LoginComponent
    },
    {
        path: 'users',
        component: UsersComponent
    },
    { path: 'pickup-schedule/:listingId',
         component: PickupComponent },
    {
        path: 'admindashboard',
        component: AdmindashboardComponent,
        canActivate: [roleGuard],
        data: { expectedRole: 'Admin' }
    },
    {
        path: 'farmerdashboard',
        component: FarmerdashboardComponent,
        canActivate: [roleGuard],
        data: { expectedRole: 'Farmer' }
    },
    {
        path: 'dealerdashboard',
        component: DealerdashboardComponent,
        canActivate: [roleGuard],
        data: { expectedRole: 'Dealer' }
    },
    {
        path: 'crop',
        component: CropComponent
    },
    {
        path: 'listings',
        component: CroplistingsComponent
    },
    {
        path: 'rating',
        component: RatingsComponent
    },
    {
        path: 'report',
        component: ReportComponent
    },
     {
        path: 'subscription',
        component: SubscriptionComponent
    },
     {
        path: 'bank',
        component: BankComponent
    },
    {
        path: 'transaction',
        component: TransactionComponent
    },
    {
        path: 'manageuser',
        component: UpdateprofileComponent
    },
    {
        path: 'negotiation',
        component: NegotiationsComponent
    },
    {
        path: 'address',
        component: AddressComponent
    },
     { path: 'auth-callback', component: AuthCallbackComponent },
  { path: '', redirectTo: '/home', pathMatch: 'full' },  
  { path: '**', redirectTo: '/home' }
];
