import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { ButtonDirective, ButtonIcon } from 'primeng/button';
import { Menu } from 'primeng/menu';
import { Ripple } from 'primeng/ripple';
import { Skeleton } from 'primeng/skeleton';
import { Tooltip } from 'primeng/tooltip';
import { Application } from '../../core/models/application.model';
import { ApplicationsStore } from '../../core/state/applications-store';
import { ApplicationNavItem } from '../application-nav-item/application-nav-item';

const SKELETON_ROWS = [0, 1, 2];

/** Left-hand panel listing every application as a PrimeNG menu. */
@Component({
  selector: 'app-side-panel',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    RouterLink,
    RouterLinkActive,
    ButtonDirective,
    ButtonIcon,
    Menu,
    Ripple,
    Skeleton,
    Tooltip,
    ApplicationNavItem,
  ],
  templateUrl: './side-panel.html',
  styleUrl: './side-panel.scss',
})
export class SidePanel {
  protected readonly store = inject(ApplicationsStore);
  protected readonly skeletonRows = SKELETON_ROWS;

  protected readonly items = computed<MenuItem[]>(() =>
    this.store.applications().map((application: Application) => ({
      id: application.id,
      label: application.name,
      state: { application },
    })),
  );
}
