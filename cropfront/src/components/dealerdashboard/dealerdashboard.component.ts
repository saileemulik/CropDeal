import { Component, OnInit } from '@angular/core';
import { DealerService } from '../../app/services/dealer.service';
import { ChartConfiguration, ChartData, ChartOptions, ChartType } from 'chart.js';
import { NgChartsModule } from 'ng2-charts';

@Component({
  selector: 'app-dealerdashboard',
  standalone: true,
  imports: [NgChartsModule],
  templateUrl: './dealerdashboard.component.html',
  styleUrls: ['./dealerdashboard.component.css']
})
export class DealerdashboardComponent implements OnInit {
  stats: any;

  // Bar chart setup
  barChartType: 'bar' = 'bar'; 

  barChartData: ChartData<'bar'> = {
    labels: [],
    datasets: [
      {
        data: [],
        label: 'Monthly Subscriptions',
        backgroundColor: '#198754'
      }
    ]
  };

  barChartOptions: ChartOptions<'bar'> = {
    responsive: true,
    plugins: {
      legend: {
        display: true
      }
    },
    scales: {
      x: {},
      y: {
        beginAtZero: true
      }
    }
  };

  constructor(private dealerService: DealerService) {}

  ngOnInit(): void {
    this.dealerService.getDashboardStats().subscribe(data => {
      this.stats = data;

      // Extract labels and data
      const months = data.monthlySubscriptions.map((m: any) => `Month ${m.month}`);
      const counts = data.monthlySubscriptions.map((m: any) => m.count);

      // Update chart data dynamically
      this.barChartData = {
        labels: months,
        datasets: [
          {
            data: counts,
            label: 'Monthly Subscriptions',
            backgroundColor: '#0d6efd'
          }
        ]
      };
    });
  }
}
