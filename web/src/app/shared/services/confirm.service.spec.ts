import { TestBed } from '@angular/core/testing';
import { Confirmation, ConfirmationService } from 'primeng/api';
import { ConfirmService } from './confirm.service';

describe('ConfirmService', () => {
  let requested: Confirmation | undefined;

  beforeEach(() => {
    requested = undefined;
    TestBed.configureTestingModule({
      providers: [
        {
          provide: ConfirmationService,
          useValue: { confirm: (confirmation: Confirmation) => (requested = confirmation) },
        },
      ],
    });
  });

  const ask = () =>
    TestBed.inject(ConfirmService).confirm({
      header: 'Delete',
      message: 'Sure?',
      acceptLabel: 'Delete',
      danger: true,
    });

  it('resolves true when accepted', async () => {
    const result = ask();
    requested!.accept!();
    await expect(result).resolves.toBe(true);
  });

  it('resolves false when rejected or dismissed', async () => {
    const result = ask();
    requested!.reject!();
    await expect(result).resolves.toBe(false);
  });

  it('styles the accept button as destructive for dangerous actions', () => {
    ask();
    expect(requested!.acceptButtonProps).toEqual({ label: 'Delete', severity: 'danger' });
  });
});
