import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Button, ButtonDirective, ButtonIcon, ButtonLabel } from 'primeng/button';
import { Tag } from 'primeng/tag';
import { Application } from '../../../../core/models/application.model';
import { AppIcon } from '../../../../shared/ui/app-icon/app-icon';

/** Title of the showcase: the demo application and actions to open or reset it. */
@Component({
  selector: 'app-showcase-header',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, Button, ButtonDirective, ButtonIcon, ButtonLabel, Tag, AppIcon],
  templateUrl: './showcase-header.html',
  styleUrl: './showcase-header.scss',
})
export class ShowcaseHeader {
  readonly application = input.required<Application>();
  readonly resetting = input(false);
  readonly resetRequested = output();
}
