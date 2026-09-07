import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgChartsModule } from 'ng2-charts';
import {
  ChartConfiguration,
  ChartData,
  ChartType
} from 'chart.js';
import { AdminService } from '../../app/services/admin.service';
import { UserRole } from '../../app/models/signup';
import { NotificationBellComponent } from '../../app/notification-bell/notification-bell.component';

@Component({
  selector: 'app-admindashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, NgChartsModule, NotificationBellComponent],
  templateUrl: './admindashboard.component.html',
  styleUrls: ['./admindashboard.component.css'],
})
export class AdmindashboardComponent implements OnInit {
  stats: any;
  user: any = {};
  adminName: string = '';
  currentDateTime = new Date();
  motivationalMessage = 'Keep pushing forward, success is near!';
  UserRole = UserRole;

  // Bar Chart: Monthly Transactions
  barChartType: ChartType = 'bar';
barChartData: ChartData<'bar', number[], string> = {
  labels: [],
  datasets: [
    {
      data: [],
      label: 'Transactions',
      backgroundColor: '#0d6efd'
    }
  ]
};


  // Pie Chart: User Distribution
  // Pie chart
userPieChartType: ChartType = 'pie';
userPieChartData: ChartData<'pie', number[], string> = {
  labels: ['Farmers', 'Dealers', 'Admins'],
  datasets: [
    {
      data: [],
      backgroundColor: ['#198754', '#0d6efd', '#ffc107']
    }
  ]
};

pendingPieChartType: 'pie' = 'pie';
pendingPieChartData: ChartData<'pie', number[], string> = {
  labels: ['Pending', 'Approved', 'Rejected'],
  datasets: [
    {
      data: [],
      backgroundColor: ['#ffc107', '#198754', '#dc3545']
    }
  ]
};

pendingPieChartOptions: ChartConfiguration<'pie'>['options'] = {
  responsive: true,
  plugins: {
    legend: {
      position: 'top'
    }
  }
};

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.adminName = this.user.name || 'Admin';
    this.getDashboardStats();

    setInterval(() => {
      this.currentDateTime = new Date();
    }, 60000);
  }

  getDashboardStats() {
    this.adminService.getDashBoard().subscribe((data) => {
      this.stats = data;
      this.prepareChartData();
    });
  }

  prepareChartData() {
  const months = this.stats.transactionsPerMonth.map((m: any) => m.month);
  const counts = this.stats.transactionsPerMonth.map((m: any) => m.count);

  this.barChartData = {
    labels: months,
    datasets: [
      {
        data: counts,
        label: 'Transactions',
        backgroundColor: '#0d6efd'
      }
    ]
  };

  this.userPieChartData = {
    labels: ['Farmers', 'Dealers', 'Admins'],
    datasets: [
      {
        data: [
          this.stats?.farmers || 0,
          this.stats?.dealers || 0,
          1 // Assuming 1 admin
        ],
        backgroundColor: ['#198754', '#0d6efd', '#ffc107']
      }
    ]
  };
  this.pendingPieChartData = {
  labels: ['Pending', 'Approved', 'Rejected'],
  datasets: [
    {
      data: [
        this.stats?.newCropRequest || 0,
        this.stats?.approvedRequest || 0,
        this.stats?.rejectedRequest || 0
      ],
      backgroundColor: ['#ffc107', '#198754', '#dc3545']
    }
  ]
};

}

}
