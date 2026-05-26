import { Component, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { MatNativeDateModule } from '@angular/material/core';

export interface SlotFilterValues {
  providerId?: string;
  specialty?: string;
  dateFrom?: string;
  dateTo?: string;
}

@Component({
  selector: 'app-slot-filter',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatFormFieldModule,
    MatSelectModule, MatDatepickerModule, MatInputModule, MatNativeDateModule,
  ],
  template: `
    <form [formGroup]="filterForm" class="filter-panel">
      <mat-form-field appearance="outline">
        <mat-label>Provider</mat-label>
        <mat-select formControlName="providerId">
          <mat-option value="">All Providers</mat-option>
          <mat-option value="provider-1">Dr. Smith</mat-option>
          <mat-option value="provider-2">Dr. Jones</mat-option>
        </mat-select>
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Specialty</mat-label>
        <mat-select formControlName="specialty">
          <mat-option value="">All Specialties</mat-option>
          <mat-option value="general">General Practice</mat-option>
          <mat-option value="cardiology">Cardiology</mat-option>
          <mat-option value="dermatology">Dermatology</mat-option>
        </mat-select>
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Start Date</mat-label>
        <input matInput [matDatepicker]="startPicker" formControlName="dateFrom" />
        <mat-datepicker-toggle matSuffix [for]="startPicker"></mat-datepicker-toggle>
        <mat-datepicker #startPicker></mat-datepicker>
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>End Date</mat-label>
        <input matInput [matDatepicker]="endPicker" formControlName="dateTo" />
        <mat-datepicker-toggle matSuffix [for]="endPicker"></mat-datepicker-toggle>
        <mat-datepicker #endPicker></mat-datepicker>
      </mat-form-field>
    </form>
  `,
  styles: [`
    .filter-panel {
      display: flex; flex-wrap: wrap; gap: 12px; padding: 16px 0;
    }
    mat-form-field { flex: 1 1 200px; }
  `],
})
export class SlotFilterComponent implements OnInit {
  @Output() filterChanged = new EventEmitter<SlotFilterValues>();

  filterForm!: FormGroup;

  constructor(private readonly fb: FormBuilder) {}

  ngOnInit(): void {
    this.filterForm = this.fb.group({
      providerId: [''],
      specialty: [''],
      dateFrom: [null],
      dateTo: [null],
    });

    this.filterForm.valueChanges.subscribe(values => {
      this.filterChanged.emit({
        providerId: values.providerId || undefined,
        specialty: values.specialty || undefined,
        dateFrom: values.dateFrom?.toISOString()?.split('T')[0],
        dateTo: values.dateTo?.toISOString()?.split('T')[0],
      });
    });
  }
}
