import { Component, OnInit } from '@angular/core';
import { FarmerService } from '../../app/services/farmer.service';
import { ChartType } from 'chart.js';
import { NgChartsModule } from 'ng2-charts';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-farmerdashboard',
  standalone: true,
  imports: [CommonModule, HttpClientModule, NgChartsModule],
  templateUrl: './farmerdashboard.component.html',
  styleUrl: './farmerdashboard.component.css'
})
export class FarmerdashboardComponent implements OnInit {
  stats: any;
  
  // Chart setup
  barChartType: ChartType = 'bar';
  pieChartType: ChartType = 'pie';
  barLabels: string[] = [];
  barData: number[] = [];

  pieLabels: string[] = [];
  pieData: number[] = [];

  constructor(private farmerService: FarmerService) {}

  ngOnInit(): void {
    this.getStats();
  }

  getStats() {
    this.farmerService.getDashboardStats().subscribe({
      next: (data) => {
        this.stats = data;

        // Setup bar chart (listings per month)
        this.barLabels = data.listingsPerMonth.map((item: any) => 
          new Date(0, item.month - 1).toLocaleString('default', { month: 'short' })
        );
        this.barData = data.listingsPerMonth.map((item: any) => item.count);

        // Setup pie chart (crop type distribution)
        this.pieLabels = data.cropTypeDistribution.map((item: any) => item.type);
        this.pieData = data.cropTypeDistribution.map((item: any) => item.count);
      },
      error: (err) => console.error('Error loading stats', err)
    });
  }
}
