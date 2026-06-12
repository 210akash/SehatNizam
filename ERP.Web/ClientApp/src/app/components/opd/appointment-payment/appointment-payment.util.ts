export function getBillingStatusLabel(statusId: number): string {
    if (statusId === 3) {
        return 'Paid';
    }
    if (statusId === 1 || statusId === 2) {
        return 'UnPaid';
    }
    return '';
}

export function isUnPaidStatus(statusId: number): boolean {
    return statusId === 1 || statusId === 2;
}

export function isPaidStatus(statusId: number): boolean {
    return statusId === 3;
}
