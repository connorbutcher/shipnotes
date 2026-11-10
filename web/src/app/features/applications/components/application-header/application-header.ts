import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { Tag } from 'primeng/tag';
import { Application } from '../../../../core/models/application.model';
import { AppIcon } from '../../../../shared/ui/app-icon/app-icon';

@Component({
  selector: 'app-application-header',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [Tag, AppIcon],
  templateUrl: './application-header.html',
  styleUrl: './application-header.scss',
})
export class ApplicationHeader {
  readonly application = input.required<Application>();
}
