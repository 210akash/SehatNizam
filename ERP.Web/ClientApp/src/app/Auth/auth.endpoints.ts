let endPointControllerName = "Auth";
export class AuthEndPoints {
    public readonly login = '/Login';
    public readonly verifyPassword = '/VerifyPassword';
    public readonly verifyOtp = '/VerifyOtp'
    public readonly send2FaOtp = '/Send2FaOtp'
    public readonly register = '/Register'
    public readonly forgetPassword = '/ForgetPassword'
    public readonly logOut = '/LogOut'
    public readonly resetPassword = '/ResetPassword'
    public readonly changePassword = '/ChangePassword'
    public readonly changeEmail = '/ChangeEmail'
    public readonly confirmChangeEmail = '/ConfirmChangeEmail'
    public readonly resendEmailConfirmation = '/ResendEmailConfirmation'
    public readonly isValidPhoneNumber = '/IsValidPhoneNumber'
    public readonly addPhonenumber = '/AddPhoneNumber'
    public readonly confirmPhoneNumber = '/ConfirmPhoneNumber'
    public readonly completeDeviceWizard = '/CompleteDeviceWizard'
}