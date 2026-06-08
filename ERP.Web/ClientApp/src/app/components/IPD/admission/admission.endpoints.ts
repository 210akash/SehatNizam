export class AdmissionEndPoints {
    public readonly saveAdmission = '/SaveAdmission';
    public readonly getAllAdmissions = '/GetAllAdmissions';
    public readonly getAllAdmissionByDoctor = '/GetAllAdmissionByDoctor';
    public readonly getAdmissionById = '/GetAdmissionById';
    public readonly getAdmissionByName = '/GetAdmissionByName';
    public readonly getAdmissionByToken = '/getAdmissionByToken';
    public readonly deleteAdmission = '/DeleteAdmission';
    public readonly getAllAdmissionStatus = '/GetAllAdmissionStatus';
    public readonly saveConsultation = '/SaveConsultation';
    public readonly confirmAdmission = '/ConfirmAdmission';
    public readonly cancelAppoinment = '/CancelAppoinment';
    public readonly getAdmissionsByBookingNo = '/GetAdmissionsByBookingNo';
}
