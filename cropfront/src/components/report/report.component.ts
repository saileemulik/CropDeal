import { Component } from '@angular/core';
import { ReportService } from '../../app/services/report.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AdminService } from '../../app/services/admin.service';
import { SnackbarService } from '../../app/services/snackbar.service';

@Component({
  selector: 'app-report',
  imports: [FormsModule, CommonModule],
  templateUrl: './report.component.html',
  styleUrl: './report.component.css'
})
export class ReportComponent {
   activeSection: string = '';
   reports: any[] = [];
  selectedReport: any = null;
  dealerIdInput = '';
  filterInput: string | null = null;
  dealers: { id: string, name: string }[] = [];
  loading = false;
  errorMessage = '';
  showGenerate: boolean = false;

   constructor(private reportService: ReportService, private adminService : AdminService, private snackbar : SnackbarService) {}
   
     ngOnInit(): void {
       this.activeSection= 'report';
       this.loadDealers();
       this.loadAllReports();
     }

   loadDealers() {
  this.adminService.getAllUsers().subscribe({
    next: (users) => {
      this.dealers = users
        .filter((user: any) => user.role === 'Dealer')
        .map((dealer: any) => ({
          id: dealer.id,
          name: dealer.name 
        }));
    },
    error: (err) => {
      console.error('Error fetching users', err);
    }
  });
}


     loadAllReports() {
    this.reportService.getAllReports().subscribe({
      next: (data) => {
        this.reports = data;
      },
      error: (err) => {
        this.errorMessage = 'Failed to load reports.';
        console.error(err);
      }
    });
  }

     generateReport() {
    if (!this.dealerIdInput) {
      this.snackbar.warning('Please enter a dealer ID');
      return;
    }

    this.loading = true;
    this.reportService.generateDealerReport(this.dealerIdInput, this.filterInput).subscribe({
      next: (report) => {
        this.snackbar.success('Report generated successfully!');
        this.selectedReport = report;
        this.loadAllReports(); // refresh list
        this.loading = false;
      },
      error: (err) => {
        this.snackbar.error('Failed to generate report.');
        this.errorMessage = 'Failed to generate report.';
        console.error(err);
        this.loading = false;
      }
    });
  }

  viewReport(id: string) {
    this.reportService.getReportById(id).subscribe({
      next: (report) => {
        this.selectedReport = report;
      },
      error: (err) => {
        this.snackbar.error("Failed to fetch report details.");
        this.errorMessage = 'Failed to fetch report details.';
        console.error(err);
      }
    });
  }

  downloadReport(reportId: string) {
    this.reportService.downloadDealerReport(reportId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'Dealer_Report.xlsx';
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        this.snackbar.error("Failed to download report.");
        this.errorMessage = 'Failed to download report.';
        console.error(err);
      }
    });
  }


}
