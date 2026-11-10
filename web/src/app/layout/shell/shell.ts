import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ApplicationsStore } from '../../core/state/applications-store';
import { ToastOutlet } from '../../shared/ui/toast-outlet/toast-outlet';
import { SidePanel } from '../side-panel/side-panel';

/** Top-level layout: application side panel on the left, routed content on the right. */
@Component({
  selector: 'app-shell',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterOutlet, SidePanel, ToastOutlet],
  template: `
    <app-side-panel />
    <main>
      <router-outlet />
    </main>
    <app-toast-outlet />
  `,
  styleUrl: './shell.scss',
})
export class Shell implements OnInit {
  private readonly store = inject(ApplicationsStore);

  ngOnInit(): void {
    this.store.load();
  }
}
