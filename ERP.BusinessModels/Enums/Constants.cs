//-----------------------------------------------------------------------
// <copyright file="Constants.cs" company="sensyrtech">
//     copy right sensyrtech.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.BusinessModels.Enums
{
    /// <summary>
    /// Declaration of Constants class.
    /// </summary>
    public static partial class Constants
    {
        /// <summary>
        /// Gets the system roles.
        /// </summary>
        /// <value>
        /// The system roles.
        /// </value>
        public const string SystemRoles = RolesPrefix.Admin + " , " + RolesPrefix.Agent;

        /// <summary>
        /// The default role
        /// </summary>
        public const string DefaultRole = RolesPrefix.Agent;

        /// <summary>
        /// The login token
        /// </summary>
        public const string LoginToken = "Token";
        public const string RefreshToken = "RefreshToken";
        public const string ViewOnlyRoleId = "8715E1F3-D0D0-4E4B-B458-06B9AA378103";
        public const string AdminRoleId = "38D42A6F-E91E-4498-B1B3-7CA8DA85E8CE";
        public const string LeadTechnicianRoleId = "27F1CE7C-1CA9-44A2-8AF7-7DC027E5535B";
        public const string AccountOwnerRoleId = "93F204A2-747D-4F00-9A8F-A27043565BA8";
        public const string OperatorRoleId = "AF018CA4-0BA4-4BED-AEF8-AC8A02A2DC56";
        public const string TechnicianRoleId = "93986EA8-F64D-4F30-9393-FA05F1A6FC11";
        /// <summary>
        /// Gets the global error.
        /// </summary>
        /// <value>
        /// The global error.
        /// </value>
        public static string GlobalError => "There is some error please contact with administrator";
        //public const string SQLScriptDefaultPath = "Scripts/script.sql";

        #region Auth
        /// <summary>
        /// Gets the registered body text.
        /// </summary>
        /// <value>
        /// The register body text.
        /// </value>
        public static string REGBODY => "Thanks for trying sensyrtech. Please Confirm your email to access<br />your transferaccount from your device";

        /// <summary>
        /// Gets the reset password body.
        /// </summary>
        /// <value>
        /// The reset password body.
        /// </value>
        public static string ResetPasswordBody => "You recently requested to reset your password for transferaccount. Click the button bellow to reset it.";

        /// <summary>
        /// Gets the invalid user or password.
        /// </summary>
        /// <value>
        /// The invalid user or password.
        /// </value>
        public static string InvalidUsrOrPwd => "Invalid email OR password";

        /// <summary>
        /// Gets the authentication code not verified.
        /// </summary>
        /// <value>
        /// The authentication code not verified.
        /// </value>
        public static string AuthenticationCodeNotVerified => "Invalid OTP";

        /// <summary>
        /// Gets the documents deleted.
        /// </summary>
        /// <value>
        /// The documents deleted.
        /// </value>
        public static string DocumentsDeleted => "Documents successfully deleted.";

        /// <summary>
        /// Gets the user not found.
        /// </summary>
        /// <value>
        /// The user not found.
        /// </value>
        public static string ErrorDeletingDocuments => "Error while delteing the documents.";

        /// <summary>
        /// Gets the user not found.
        /// </summary>
        /// <value>
        /// The user not found.
        /// </value>
        public static string UserNotFound => "User not found";
        /// <summary>
        /// Gets the activated user.
        /// </summary>
        /// <value>
        /// The user is currently deactivated.
        /// </value>
        public static string UserDeactivated => "User is Deactivated";

        /// <summary>
        /// Gets the authentication code not sent.
        /// </summary>
        /// <value>
        /// The authentication code not sent.
        /// </value>
        public static string AuthenticationCodeNotSent => "Error while sending verification code";

        /// <summary>
        /// Gets the invalid refresh token.
        /// </summary>
        /// <value>
        /// The invalid refresh token.
        /// </value>
        public static string InvalidRefreshToken => "Invalid refresh token";

        /// <summary>
        /// Gets the invalid user or password.
        /// </summary>
        /// <value>
        /// The invalid user or password.
        /// </value>
        public static string InvalidEmail => "Invalid email address.";

        /// <summary>
        /// Gets the reset email subject.
        /// </summary>
        /// <value>
        /// The reset email subject.
        /// </value>
        public static string ResetEmailSubject => "Reset Password";

        /// <summary>
        /// Gets the reset email subject.
        /// </summary>
        /// <value>
        /// The reset email subject.
        /// </value>
        public static string ForgotPassworTemplate => "ForgotPassword.cshtml";
        /// <summary>
        /// Gets the reset password email BTN text.
        /// </summary>
        /// <value>
        /// The reset password email BTN text.
        /// </value>
        public static string ResetPasswordEmailBtnTxt => "Reset Password";

        /// <summary>
        /// Gets the reset confirmation email.
        /// </summary>
        /// <value>
        /// The reset confirmation email.
        /// </value>
        public static string ResetConfirmationEmail => "A mail sent in the given address. Please check the mail and reset your password!";

        /// <summary>
        /// Gets the invalid user or password.
        /// </summary>
        /// <value>
        /// The invalid user or password.
        /// </value>
        public static string PasswordChangeSuccessfull => "Password changed sucessfully.";

        /// <summary>
        /// Gets the invalid phone number.
        /// </summary>
        /// <value>
        /// The invalid phone number.
        /// </value>
        public static string InvalidPhoneNumber => "Invalid phone number.";
        public static string InvalidCode => "Invalid Code.";

        /// <summary>
        /// Gets the REG confirmation email.
        /// </summary>
        /// <value>
        /// The REG confirmation email.
        /// </value>
        public static string RegConfirmationEmail => "A mail sent in the given address. Please check the mail and confrim your account!";

        /// <summary>
        /// Gets the change password successful.
        /// </summary>
        /// <value>
        /// The change password successful.
        /// </value>
        public static string ChangePasswordSuccessful => "Password changed successfully!";

        /// <summary>
        /// Gets the invalid user identifier.
        /// </summary>
        /// <value>
        /// The invalid user identifier.
        /// </value>
        public static string InvalidUserId => "Invalid user id address.";

        /// <summary>
        /// Gets the email verified.
        /// </summary>
        /// <value>
        /// The email verified.
        /// </value>
        public static string EmailCnfrmd => "Thank you for confirming your email.";

        /// <summary>
        /// Gets the log out successfully.
        /// </summary>
        /// <value>
        /// The log out successfully.
        /// </value>
        public static string LogOutSuccessfully => "Logout out successfully from the application";

        /// <summary>
        /// Gets the phone confirmation SMS sent.
        /// </summary>
        /// <value>
        /// The phone confirmation SMS sent.
        /// </value>
        public static string PhoneConfirmationSmsSent => "A confirmation message has been sent to the Phone Number.";

        /// <summary>
        /// Gets the reset confirmation email SMS.
        /// </summary>
        /// <value>
        /// The reset confirmation email SMS.
        /// </value>
        public static string ResetConfirmationEmailSMS => "A mail/SMS sent in the given address/number. Please check the mail/SMS and reset your password!";

        /// <summary>
        /// Gets the confirmation code identifier.
        /// </summary>
        /// <value>
        /// The confirmation code identifier.
        /// </value>
        public static string ConfirmationCodeId => "Your confirmation code is {0}";

        /// <summary>
        /// Gets the add phone number error.
        /// </summary>
        /// <value>
        /// The add phone number error.
        /// </value>
        public static string AddPhoneNumberError => "An error occurred while adding the phone number.";

        /// <summary>
        /// Gets the resend phone confirmation code successfully.
        /// </summary>
        /// <value>
        /// The resend phone confirmation code successfully.
        /// </value>
        public static string ResendPhoneConfirmationCodeSuccessfully => "Succesfully resend phone number verification code.";

        /// <summary>
        /// Gets the resend phone confirmation code error.
        /// </summary>
        /// <value>
        /// The resend phone confirmation code error.
        /// </value>
        public static string ResendPhoneConfirmationCodeError => "An error occurred while resending the phone number confirmation code.";

        /// <summary>
        /// Gets the phone number confirmed.
        /// </summary>
        /// <value>
        /// The phone number confirmed.
        /// </value>
        public static string PhoneNumberConfirmed => "Phone Number has been confirmed Sucessfully.";

        /// <summary>
        /// Gets the phone number not confirmed.
        /// </summary>
        /// <value>
        /// The phone number not confirmed.
        /// </value>
        public static string PhoneNumberNotConfirmed => "Your phone number not confirm.";

        /// <summary>
        /// Gets the enable two factor authentication successfully.
        /// </summary>
        /// <value>
        /// The enable two factor authentication successfully.
        /// </value>
        public static string EnableTwoFactorAuthenticationSuccessfully => "Enable two factor Authentication successfully.";

        /// <summary>
        /// Gets the disable two factor authentication successfully.
        /// </summary>
        /// <value>
        /// The disable two factor authentication successfully.
        /// </value>
        public static string DisableTwoFactorAuthenticationSuccessfully => "Disable two factor Authentication successfully.";

        /// <summary>
        /// Gets the add role error.
        /// </summary>
        /// <value>
        /// The add role error.
        /// </value>
        public static string AddRoleError => "An error occurred while adding the role";

        /// <summary>
        /// Gets the add role successfully.
        /// </summary>
        /// <value>
        /// The add role successfully.
        /// </value>
        public static string AddRoleSuccessfully => "Role has been added sucessfully";

        /// <summary>
        /// Gets the email exist.
        /// </summary>
        /// <value>
        /// The email exist.
        /// </value>
        public static string EmailExist => "Email already exists.";

        /// <summary>
        /// Gets the email not exist.
        /// </summary>
        /// <value>
        /// The email not exist.
        /// </value>
        public static string EmailNotExist => "Email not exists.";

        /// <summary>
        /// Gets the user Privacy updated success.
        /// </summary>
        /// <value>
        /// The user updated success.
        /// </value>
        public static string AddPrivacySuccess => "User Privacy added successfully!";

        /// <summary>
        /// Gets the user updated success.
        /// </summary>
        /// <value>
        /// The user updated success.
        /// </value>
        public static string AddPrivacyError => "User Privacy not added!";

        /// <summary>
        /// Gets the voice message.
        /// </summary>
        /// <value>
        /// The voice message.
        /// </value>
        public static string VoiceMessage => "<Response><Say loop='{0}'>{1}</Say><Pause length='{2}'/><Say loop='{0}'>{1}</Say></Response>";
       // public static string VoiceMessage => "<Response><Say loop='{0}' length='{2}'>{1}</Say></Response>";

        /// <summary>
        /// Gets the user Privacy updated success.
        /// </summary>
        /// <value>
        /// The user updated success.
        /// </value>
        public static string GetPrivacySuccess => "User Privacy found successfully!";

        /// <summary>
        /// gets the email sign up value
        /// </summary>
        public static string CommonEmailTemplate => "CommonEmailTemplate.cshtml";
        public static string TechnicianAssignmentTemplate => "TechnicianAssignmentTemplate.cshtml";
        public static string AcknowledgedAssignmentTemplate => "AcknowledgedAssignmentTemplate.cshtml";
        public static string ResolvedAssignmentTemplate => "ResolvedAssignmentTemplate.cshtml";
        public static string MentionedUserTemplate => "MentionedUserTemplate.cshtml";
        public static string ReassignTechnicianTemplate => "ReassignTechnicianTemplate.cshtml";
        public static string RevokedAssignment => "RevokedAssignment.cshtml";
        public static string CollaborateInvitationEmail => "CollaborateInvitationEmail.cshtml";
        public static string CollaborateSubject => "Collaborate Invitation";


        public static string VerifyOTPTemplate => "VerifyOTP.cshtml";

        public static string CriticalAlarmTriggered => "CriticalAlarmTriggered.cshtml";

        /// <summary>
        /// Gets the base path.
        /// </summary>
        /// <value>
        /// The base path.
        /// </value>
        public static string BasePath => "/EmailTemplates/";

        /// <summary>
        /// Gets the update privacy failure.
        /// </summary>
        /// <value>
        /// The update privacy failure.
        /// </value>
        public static string UpdatePrivacyNotFound => "User Privacy not found against this user";

        /// <summary>
        /// Gets the update privacy success.
        /// </summary>
        /// <value>
        /// The update privacy success.
        /// </value>
        public static string UpdatePrivacySuccess => "User Privacy Updated successfully!";

        /// <summary>
        /// Gets the Notification Settings retrieve successfully.
        /// </summary>
        /// <value>
        /// The Notification Settings retrieve successfully.
        /// </value>
        public static string NotificationsSettingsRetrievedSuccessfully => "Notification Settings Retrieved Successfully";

        /// <summary>
        /// Gets the Notification Settings retrieve error.
        /// </summary>
        /// <value>
        /// The Notification Settings retrieve error.
        /// </value>
        public static string NotificationsSettingsRetrievedError => "Notification Settings Retrieve Error";

        /// <summary>
        /// Gets the notification settings added.
        /// </summary>
        /// <value>
        /// The notification settings added.
        /// </value>
        public static string NotificationSettingsAdded => "User Notifications setting Added Successfully.";

        /// <summary>
        /// Gets the notification settings error.
        /// </summary>
        /// <value>
        /// The notification settings error.
        /// </value>
        public static string NotificationSettingsAddError => "User Notifications setting Not Added.";

        /// <summary>
        /// Gets the notification settings Updated.
        /// </summary>
        /// <value>
        /// The notification settings Updated.
        /// </value>
        public static string NotificationSettingsUpdated => "User Notifications Settings Updated Successfully.";

        /// <summary>
        /// Gets the notification settings error.
        /// </summary>
        /// <value>
        /// The notification settings error.
        /// </value>
        public static string NotificationSettingsUpdateError => "User Notifications Settings Not Updated.";

        /// <summary>
        /// Gets the email not confirmed.
        /// </summary>
        /// <value>
        /// The email not confirmed.
        /// </value>
        public static string EmailNotConfirmed => "Email is not confirmed";

        /// <summary>
        /// Gets the user locked out.
        /// </summary>
        /// <value>
        /// The user locked out.
        /// </value>
        public static string UserLockedOut => "The account is locked out";
        public static string EmailAlreadyConfirmed => "Email Already Confirmed.";
        public static string EmailConfirmationCodeExpired => "Email confirmation code has been expired. Please resend the code.";
        public static string ResendConfirmationSuccessful => "Resend Confirmation Code successful";
        public static string InvalidEmailConfirmationCode => "Invalid Email confirmation code.";
        public static string REGSUCCSS => "Successfully Registered";
        public static string EmailAlreadyExists => "Email already exists please use different Email";
        public static string ChangeEmailError => "An error occured while changing the email";
        public static string ChangeEmailSuccessful => "Email Change successfully";
        public static string UserInvitationSubject => "Invitation";
        public static string EmailConfirmation => "Email Confirmation";
        public static string WarningAlarm => "Warning alarm on a sensor";
        public static string EmailConfirmationTemplate => "EmailConfirmation.cshtml";
        public static string InvitationEmailTemplate => "InvitationEmail.cshtml";
        public static string InvitationReminderSubject => "Invitation Reminder";
        public static string InvitationReminderTemplate => "InvitationReminder.cshtml";
        public static string InvalidToken => "Invalid token";

        #endregion

        #region Blob Controller        
        /// <summary>
        /// Gets the file successfully upload.
        /// </summary>
        /// <value>
        /// The file successfully upload.
        /// </value>
        public static string FileSuccessfullyUpload => "File successfully uploaded on the Azure Blob";

        /// <summary>
        /// Gets the file not upload.
        /// </summary>
        /// <value>
        /// The file not upload.
        /// </value>
        public static string FileNotUpload => "File not upload on the Azure Blob";

        /// <summary>
        /// Gets the image successfully upload.
        /// </summary>
        /// <value>
        /// The image successfully upload.
        /// </value>
        public static string ImageSuccessfullyUpload => "Image successfully uploaded on the Azure Blob";

        /// <summary>
        /// Gets the image not upload.
        /// </summary>
        /// <value>
        /// The image not upload.
        /// </value>
        public static string ImageNotUpload => "Image not upload on the Azure Blob";

        /// <summary>
        /// Gets the image not deleted.
        /// </summary>
        /// <value>
        /// The image not deleted.
        /// </value>
        public static string FileUrlInvalidFormat => "File Url is not in valid format.";

        /// <summary>
        /// Gets the image successfully deleted.
        /// </summary>
        /// <value>
        /// The image successfully deleted.
        /// </value>
        public static string ImageSuccessfullyDeleted => "Image successfully Deleted from the Azure Blob";

        /// <summary>
        /// Gets the image not deleted.
        /// </summary>
        /// <value>
        /// The image not deleted.
        /// </value>
        public static string ImageNotDeleted => "Image not deleted from the Azure Blob";

        /// <summary>
        /// Gets the phone number already exists.
        /// </summary>
        /// <value>
        /// The phone number already exists.
        /// </value>
        public static string PhoneNumberAlreadyExists => "Phone Number has already associated with another account.";

        /// <summary>
        /// Form Created.
        /// </summary>
        /// <value>
        /// Form Created.
        /// </value>
        public static string FormCreated => "Has Been Created.";


        /// <summary>
        /// Form Updated.
        /// </summary>
        /// <value>
        /// Form Updated.
        /// </value>
        public static string FormUpdated => "Has Been Updated.";
        #endregion

    }
}
