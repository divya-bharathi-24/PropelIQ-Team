import { Injectable, OnDestroy } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { webSocket, WebSocketSubject } from 'rxjs/webSocket';
import { retryWhen, delay, takeUntil } from 'rxjs/operators';
import { environment } from '@environments/environment';

export interface SlotUpdate {
  slotId: string;
  isAvailable: boolean;
  action: 'added' | 'removed' | 'updated';
}

@Injectable({ providedIn: 'root' })
export class SlotWebsocketService implements OnDestroy {
  private socket$: WebSocketSubject<SlotUpdate> | null = null;
  private readonly destroy$ = new Subject<void>();
  private readonly wsUrl = environment.apiBaseUrl.replace('https', 'wss').replace('http', 'ws') + '/ws/slots';

  connect(): Observable<SlotUpdate> {
    if (!this.socket$) {
      this.socket$ = webSocket<SlotUpdate>(this.wsUrl);
    }

    return this.socket$.pipe(
      retryWhen(errors => errors.pipe(delay(5000))),
      takeUntil(this.destroy$),
    );
  }

  disconnect(): void {
    this.socket$?.complete();
    this.socket$ = null;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.disconnect();
  }
}
