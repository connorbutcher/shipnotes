import { HttpErrorResponse } from '@angular/common/http';
import { describeHttpError } from './problem-details';

describe('describeHttpError', () => {
  it('prefers the first validation error', () => {
    const error = new HttpErrorResponse({
      status: 400,
      error: { title: 'Invalid', errors: { name: ['Name is required.'] } },
    });
    expect(describeHttpError(error)).toBe('Name is required.');
  });

  it('falls back to the problem detail', () => {
    const error = new HttpErrorResponse({
      status: 409,
      error: { title: 'Conflict', detail: 'Already exists.' },
    });
    expect(describeHttpError(error)).toBe('Already exists.');
  });

  it('explains network failures', () => {
    expect(describeHttpError(new HttpErrorResponse({ status: 0 }))).toBe(
      'Unable to reach the ShipNotes API.',
    );
  });
});
