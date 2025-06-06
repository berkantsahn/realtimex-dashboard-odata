import { Component, OnInit } from '@angular/core';
import { DataService } from '../../services/data.service';
import { RealTimeService } from '../../services/real-time.service';
import { RealTimeData } from '../../models/real-time-data.model';
import CustomStore from 'devextreme/data/custom_store';
import { lastValueFrom } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';

interface GridRow {
  id: number;
  [key: string]: any;
}

@Component({
  selector: 'app-data-grid',
  templateUrl: './data-grid.component.html',
  styleUrls: ['./data-grid.component.scss'],
  standalone: false
})
export class DataGridComponent implements OnInit {
  dataSource: any;
  refreshMode = 'reshape';
  loadingIndicator = true;
  loading = false;

  constructor(
    private dataService: DataService,
    private realTimeService: RealTimeService,
    private snackBar: MatSnackBar
  ) {
    this.dataSource = new CustomStore({
      key: 'id',
      load: (loadOptions: any) => {
        let params: any = {};

        // Filtreleme
        if (loadOptions.filter) {
          params.filter = this.buildODataFilter(loadOptions.filter);
        }

        // Sıralama
        if (loadOptions.sort) {
          params.orderby = this.buildODataOrderBy(loadOptions.sort);
        }

        // Sayfalama
        if (loadOptions.skip) {
          params.skip = loadOptions.skip;
        }
        if (loadOptions.take) {
          params.top = loadOptions.take;
        }

        // Toplam kayıt sayısı
        if (loadOptions.requireTotalCount) {
          params.count = true;
        }

        return lastValueFrom(this.dataService.getRealTimeData(params))
          .then((response) => {
            return {
              data: response.value,
              totalCount: response['@odata.count']
            };
          })
          .catch((error) => {
            console.error('Data loading error:', error);
            throw error;
          });
      },
      insert: (values) => {
        return lastValueFrom(this.dataService.createRealTimeData(values as RealTimeData));
      },
      update: (key, values) => {
        return lastValueFrom(this.dataService.updateRealTimeData(key, values as RealTimeData));
      },
      remove: (key) => {
        return lastValueFrom(this.dataService.deleteRealTimeData(key));
      }
    });
  }

  ngOnInit(): void {
    // SignalR üzerinden gelen gerçek zamanlı güncellemeleri dinle
    this.realTimeService.dataReceived.subscribe((data: RealTimeData) => {
      // Grid'i yenile
      this.dataSource.push([{ type: 'insert', data: data }]);
    });
  }

  convertMetadataToArray(metadata: { [key: string]: any } | undefined): Array<{ key: string; value: any }> {
    if (!metadata) {
      return [];
    }
    return Object.entries(metadata).map(([key, value]) => ({ key, value }));
  }

  private buildODataFilter(filterValue: any[]): string {
    // DevExtreme filtre formatını OData filtre formatına dönüştür
    if (!Array.isArray(filterValue)) {
      return '';
    }

    let result = '';

    if (filterValue.length === 3) {
      const [field, operator, value] = filterValue;

      switch (operator.toLowerCase()) {
        case '=':
          result = `${field} eq ${this.formatValue(value)}`;
          break;
        case '<>':
          result = `${field} ne ${this.formatValue(value)}`;
          break;
        case '>':
          result = `${field} gt ${this.formatValue(value)}`;
          break;
        case '>=':
          result = `${field} ge ${this.formatValue(value)}`;
          break;
        case '<':
          result = `${field} lt ${this.formatValue(value)}`;
          break;
        case '<=':
          result = `${field} le ${this.formatValue(value)}`;
          break;
        case 'contains':
          result = `contains(${field}, ${this.formatValue(value)})`;
          break;
        case 'notcontains':
          result = `not contains(${field}, ${this.formatValue(value)})`;
          break;
        case 'startswith':
          result = `startswith(${field}, ${this.formatValue(value)})`;
          break;
        case 'endswith':
          result = `endswith(${field}, ${this.formatValue(value)})`;
          break;
      }
    } else if (filterValue.length === 2) {
      const [condition1, condition2] = filterValue;
      const operand = condition1[1]; // 'and' veya 'or'

      const filter1 = this.buildODataFilter(condition1);
      const filter2 = this.buildODataFilter(condition2);

      if (filter1 && filter2) {
        result = `(${filter1} ${operand} ${filter2})`;
      }
    }

    return result;
  }

  private buildODataOrderBy(sortValue: any[]): string {
    if (!Array.isArray(sortValue)) {
      return '';
    }

    return sortValue
      .map(item => `${item.selector} ${item.desc ? 'desc' : 'asc'}`)
      .join(',');
  }

  private formatValue(value: any): string {
    if (typeof value === 'string') {
      return `'${value}'`;
    }
    if (value instanceof Date) {
      return `${value.toISOString()}`;
    }
    return `${value}`;
  }

  onToolbarPreparing(e: any): void {
    if (e.toolbarOptions?.items) {
      e.toolbarOptions.items.unshift({
        location: 'after',
        widget: 'dxButton',
        options: {
          icon: 'refresh',
          onClick: () => this.loadData()
        }
      });
    }
  }

  onRowUpdating(e: { oldData: GridRow; newData: Partial<GridRow> }): void {
    try {
      this.dataService.update(e.oldData.id, e.newData, {}).subscribe({
        error: (error: Error) => this.showError('Update failed: ' + error.message)
      });
    } catch (error) {
      if (error instanceof Error) {
        this.showError('Update failed: ' + error.message);
      }
    }
  }

  onRowInserting(e: { data: GridRow }): void {
    try {
      this.dataService.create(e.data, {}).subscribe({
        error: (error: Error) => this.showError('Insert failed: ' + error.message)
      });
    } catch (error) {
      if (error instanceof Error) {
        this.showError('Insert failed: ' + error.message);
      }
    }
  }

  onRowRemoving(e: { data: GridRow }): void {
    try {
      this.dataService.delete(e.data.id, {}).subscribe({
        error: (error: Error) => this.showError('Delete failed: ' + error.message)
      });
    } catch (error) {
      if (error instanceof Error) {
        this.showError('Delete failed: ' + error.message);
      }
    }
  }

  private showError(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom'
    });
  }
}