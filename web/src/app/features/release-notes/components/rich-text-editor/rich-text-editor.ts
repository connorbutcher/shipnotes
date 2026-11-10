import { ChangeDetectionStrategy, Component, DestroyRef, inject, input } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { QuillEditorComponent, QuillModules } from 'ngx-quill';
import type Quill from 'quill';
import { ImageUploader, enableImageUpload } from './editor-image-upload';

/** Rich text editor for release note content, producing semantic HTML. */
@Component({
  selector: 'app-rich-text-editor',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, QuillEditorComponent],
  templateUrl: './rich-text-editor.html',
  styleUrl: './rich-text-editor.scss',
})
export class RichTextEditor {
  private readonly destroyRef = inject(DestroyRef);

  readonly control = input.required<FormControl<string>>();
  readonly placeholder = input('Describe what changed…');
  /** Uploads images added in the editor. Without one, images cannot be added. */
  readonly uploadImage = input<ImageUploader | null>(null);

  protected readonly modules: QuillModules = {
    toolbar: [
      [{ header: [2, 3, false] }],
      ['bold', 'italic', 'underline', 'strike'],
      [{ list: 'ordered' }, { list: 'bullet' }],
      ['blockquote', 'code-block', 'link', 'image'],
      ['clean'],
    ],
  };

  protected editorCreated(quill: Quill): void {
    const disable = enableImageUpload(quill, () => this.uploadImage());
    this.destroyRef.onDestroy(disable);
  }
}
