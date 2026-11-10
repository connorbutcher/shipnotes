import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ApplicationsStore } from '../../core/state/applications-store';
import { SidePanel } from '../side-panel/side-panel';

/** Top-level layout: application side panel on the left, routed content on the right. */
@Component({
  selector: 'app-shell',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterOutlet, SidePanel],
  templateUrl: './shell.html',
  styleUrl: './shell.scss',
})
export class Shell implements OnInit {
  private readonly store = inject(ApplicationsStore);

  ngOnInit(): void {
    this.store.load();
  }
}
