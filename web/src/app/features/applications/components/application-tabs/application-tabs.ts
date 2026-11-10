import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink } from '@angular/router';
import { Tab, TabList, Tabs } from 'primeng/tabs';
import { filter, map } from 'rxjs';

interface SectionTab {
  label: string;
  path: string;
  icon: string;
}

const TABS: readonly SectionTab[] = [
  { label: 'Release notes', path: 'release-notes', icon: 'pi pi-list' },
  { label: 'Integration', path: 'integration', icon: 'pi pi-code' },
  { label: 'Settings', path: 'settings', icon: 'pi pi-cog' },
];

/** Router-driven PrimeNG tabs for an application's sections. */
@Component({
  selector: 'app-application-tabs',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, Tabs, TabList, Tab],
  templateUrl: './application-tabs.html',
  styleUrl: './application-tabs.scss',
})
export class ApplicationTabs {
  private readonly router = inject(Router);

  protected readonly tabs = TABS;

  private readonly url = toSignal(
    this.router.events.pipe(
      filter((event) => event instanceof NavigationEnd),
      map(() => this.router.url),
    ),
    { initialValue: this.router.url },
  );

  protected readonly activeTab = computed(() => {
    const segments = this.url().split(/[/?#]/);
    return TABS.find((tab) => segments.includes(tab.path))?.path ?? TABS[0].path;
  });
}
