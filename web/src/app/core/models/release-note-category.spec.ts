import { RELEASE_NOTE_CATEGORIES, byCategoryOrder, categoryOption } from './release-note-category';

describe('release note categories', () => {
  it('lists breaking changes first', () => {
    expect(RELEASE_NOTE_CATEGORIES.map((c) => c.value)).toEqual([
      'breaking',
      'added',
      'changed',
      'deprecated',
      'removed',
      'fixed',
      'security',
    ]);
  });

  it('sorts into display order', () => {
    expect(byCategoryOrder(['security', 'added', 'breaking'] as const, (c) => c)).toEqual([
      'breaking',
      'added',
      'security',
    ]);
  });

  it('finds the label for a category', () => {
    expect(categoryOption('fixed').label).toBe('Fixed');
  });
});
