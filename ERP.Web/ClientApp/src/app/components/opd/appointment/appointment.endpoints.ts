export class AppointmentEndPoints {
    public readonly saveAppointment = '/SaveAppointment';
    public readonly getAllAppointments = '/GetAllAppointments';
    public readonly getAllAppointmentByDoctor = '/GetAllAppointmentByDoctor';
    public readonly getAppointmentById = '/GetAppointmentById';
    public readonly getAppointmentByName = '/GetAppointmentByName';
    public readonly getAppointmentByToken = '/getAppointmentByToken';
    public readonly deleteAppointment = '/DeleteAppointment';
    public readonly getAllAppointmentStatus = '/GetAllAppointmentStatus';
    public readonly saveConsultation = '/SaveConsultation';
    public readonly confirmAppointment = '/ConfirmAppointment';
    public readonly cancelAppoinment = '/CancelAppoinment';
    public readonly getAppointmentsByBookingNo = '/GetAppointmentsByBookingNo';
}
