import { TestBed } from '@angular/core/testing';
import { MessageService, ToastMessageOptions } from 'primeng/api';
import { NotificationService } from './notification.service';

describe('NotificationService', () => {
  const added: ToastMessageOptions[] = [];

  beforeEach(() => {
    added.length = 0;
    TestBed.configureTestingModule({
      providers: [
        { provide: MessageService, useValue: { add: (m: ToastMessageOptions) => added.push(m) } },
      ],
    });
  });

  it('shows success and error toasts', () => {
    const notifications = TestBed.inject(NotificationService);
    notifications.success('Saved.');
    notifications.error('Failed.');

    expect(added.map(({ severity, detail }) => ({ severity, detail }))).toEqual([
      { severity: 'success', detail: 'Saved.' },
      { severity: 'error', detail: 'Failed.' },
    ]);
  });
});
