export class IssuanceEndPoints {
    public readonly saveIssuance = '/SaveIssuance';
    public readonly getAllIssuances = '/GetAllIssuances';
    public readonly getIssuanceById = '/GetIssuanceById';
    public readonly getIssuanceByName = '/GetIssuanceByName';
    public readonly deleteIssuance = '/DeleteIssuance';
    public readonly getIssuanceCode = '/GetIssuanceCode';
    public readonly processIssuance = '/ProcessIssuance';
    public readonly getIndentRequestCount = '/GetIssuanceCount';
    public readonly approveIssuance = '/ApproveIssuance';
    // public readonly getPendingDemand = '/getPendingDemand';
    public readonly getPendingIndentRequestItems = '/GetPendingIndentRequestItems';
    public readonly getPendingIndentRequest = '/GetPendingIndentRequest';
}
