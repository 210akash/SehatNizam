import { ValidatorFn, Validators } from '@angular/forms';
import { createMask } from '@ngneat/input-mask';

export const BLOOD_BANK_NAME_MAX_LENGTH = 50;
export const BLOOD_BANK_NAME_PATTERN = /^[a-zA-Z\s.'-]*$/;
export const BLOOD_BANK_BLOOD_GROUP_MAX_LENGTH = 10;
export const BLOOD_BANK_BLOOD_GROUP_PATTERN = /^[a-zA-Z+\-\s]*$/;
export const BLOOD_BANK_CNIC_PATTERN = /^\d{5}-\d{7}-\d$/;
export const BLOOD_BANK_CNIC_MASK = createMask('99999-9999999-9');

export function bloodBankNameValidators(required = true): ValidatorFn[] {
    const validators: ValidatorFn[] = [
        Validators.maxLength(BLOOD_BANK_NAME_MAX_LENGTH),
        Validators.pattern(BLOOD_BANK_NAME_PATTERN)
    ];
    if (required) {
        validators.unshift(Validators.required);
    }
    return validators;
}

export function bloodBankBloodGroupValidators(required = true): ValidatorFn[] {
    const validators: ValidatorFn[] = [
        Validators.maxLength(BLOOD_BANK_BLOOD_GROUP_MAX_LENGTH),
        Validators.pattern(BLOOD_BANK_BLOOD_GROUP_PATTERN)
    ];
    if (required) {
        validators.unshift(Validators.required);
    }
    return validators;
}

export function bloodBankCnicValidators(required = false): ValidatorFn[] {
    const validators: ValidatorFn[] = [Validators.pattern(BLOOD_BANK_CNIC_PATTERN)];
    if (required) {
        validators.unshift(Validators.required);
    }
    return validators;
}

export function sanitizeBloodBankName(value: string, maxLength = BLOOD_BANK_NAME_MAX_LENGTH): string {
    return (value || '').replace(/[^a-zA-Z\s.'-]/g, '').slice(0, maxLength);
}

export function sanitizeBloodBankBloodGroup(value: string, maxLength = BLOOD_BANK_BLOOD_GROUP_MAX_LENGTH): string {
    return (value || '').replace(/[^a-zA-Z+\-\s]/g, '').slice(0, maxLength);
}

export function formatBloodBankCnic(value: string): string {
    const digits = (value || '').replace(/\D/g, '').slice(0, 13);
    if (digits.length <= 5) {
        return digits;
    }
    if (digits.length <= 12) {
        return `${digits.slice(0, 5)}-${digits.slice(5)}`;
    }
    return `${digits.slice(0, 5)}-${digits.slice(5, 12)}-${digits.slice(12)}`;
}
