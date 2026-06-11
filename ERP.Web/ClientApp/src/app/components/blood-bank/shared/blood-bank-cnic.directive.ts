import { Directive, HostListener, Optional, Self } from '@angular/core';
import { NgControl } from '@angular/forms';
import { formatBloodBankCnic } from './blood-bank-input.utils';

@Directive({
    selector: 'input[appBloodBankCnic]',
    standalone: false
})
export class BloodBankCnicDirective {
    constructor(@Optional() @Self() private ngControl: NgControl) { }

    @HostListener('keypress', ['$event'])
    onKeypress(event: KeyboardEvent): void {
        const key = event.key;
        if (key === 'Backspace' || key === 'Delete' || key === 'Tab' || key === 'ArrowLeft' || key === 'ArrowRight') {
            return;
        }
        if (!/^\d$/.test(key)) {
            event.preventDefault();
        }
    }

    @HostListener('input', ['$event'])
    onInput(event: Event): void {
        const input = event.target as HTMLInputElement;
        const formatted = formatBloodBankCnic(input.value);
        if (input.value === formatted) {
            return;
        }

        input.value = formatted;
        this.ngControl?.control?.setValue(formatted);
    }
}
