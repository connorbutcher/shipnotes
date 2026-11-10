import type { TagSeverity } from 'primeng/types/tag';

/** The kind of change a release note section describes. */
export type ReleaseNoteCategory =
  'breaking' | 'added' | 'changed' | 'deprecated' | 'removed' | 'fixed' | 'security';

export interface ReleaseNoteCategoryOption {
  value: ReleaseNoteCategory;
  label: string;
  /** A PrimeIcons class, e.g. `pi pi-plus`. */
  icon: string;
  severity: TagSeverity;
  description: string;
}

/** Every category in display order, matching the API (breaking changes first). */
export const RELEASE_NOTE_CATEGORIES: readonly ReleaseNoteCategoryOption[] = [
  {
    value: 'breaking',
    label: 'Breaking changes',
    icon: 'pi pi-exclamation-triangle',
    severity: 'danger',
    description: 'Changes that need users or integrators to act',
  },
  {
    value: 'added',
    label: 'Added',
    icon: 'pi pi-plus',
    severity: 'success',
    description: 'New features',
  },
  {
    value: 'changed',
    label: 'Changed',
    icon: 'pi pi-sync',
    severity: 'info',
    description: 'Changes to existing behaviour',
  },
  {
    value: 'deprecated',
    label: 'Deprecated',
    icon: 'pi pi-clock',
    severity: 'warn',
    description: 'Features that will be removed later',
  },
  {
    value: 'removed',
    label: 'Removed',
    icon: 'pi pi-minus-circle',
    severity: 'contrast',
    description: 'Features that are gone',
  },
  {
    value: 'fixed',
    label: 'Fixed',
    icon: 'pi pi-wrench',
    severity: 'secondary',
    description: 'Bug fixes',
  },
  {
    value: 'security',
    label: 'Security',
    icon: 'pi pi-shield',
    severity: 'danger',
    description: 'Vulnerability fixes and hardening',
  },
];

export function categoryOption(category: ReleaseNoteCategory): ReleaseNoteCategoryOption {
  return RELEASE_NOTE_CATEGORIES.find((c) => c.value === category) ?? RELEASE_NOTE_CATEGORIES[0];
}

/** Sorts categories (or anything with a category) into display order. */
export function byCategoryOrder<T>(
  items: readonly T[],
  category: (item: T) => ReleaseNoteCategory,
): T[] {
  const order = (item: T) => RELEASE_NOTE_CATEGORIES.findIndex((c) => c.value === category(item));
  return [...items].sort((a, b) => order(a) - order(b));
}
