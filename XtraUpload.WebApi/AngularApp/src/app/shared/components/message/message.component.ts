import { Component, Input } from '@angular/core';
import { Subject } from 'rxjs';
import { IGenericMessage } from 'app/models';
import { trigger, transition, style, sequence, animate } from '@angular/animations';

export const msgAnimation =
  trigger('msgAnimation', [
    transition(':enter', [
      style({ height: '*', opacity: '0', transform: 'translateX(-550px)', 'box-shadow': 'none' }),
      sequence([
        animate('.35s ease', style({ height: '*', opacity: '.2', transform: 'translateX(0)', 'box-shadow': 'none' })),
        animate('.35s ease', style({ height: '*', opacity: 1, transform: 'translateX(0)' }))
      ])
    ]),
    transition(':leave', [
      animate(400, style({ opacity: 0 }))
    ])
  ]);

@Component({
  selector: 'app-message',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.css'],
  animations: [msgAnimation],
})

export class MessageComponent {
  @Input() message$: Subject<IGenericMessage>;

  hideAlert() {
    this.message$.next(undefined);
  }
}
