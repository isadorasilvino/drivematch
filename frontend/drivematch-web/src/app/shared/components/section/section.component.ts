import { Component, input } from '@angular/core';

@Component({
  selector: 'app-section',
  standalone: true,
  templateUrl: './section.component.html',
  styleUrl: './section.component.scss',
})
export class SectionComponent {
  readonly title = input.required<string>();
  readonly description = input<string>();
  readonly divided = input(false);
}