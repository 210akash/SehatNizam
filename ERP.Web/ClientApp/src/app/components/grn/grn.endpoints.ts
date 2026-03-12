export class GRNEndPoints {
  public readonly saveGRN = '/SaveGRN';
  public readonly getAllGRNs = '/GetAllGRNs';
  public readonly getGRNById = '/GetGRNById';
  public readonly getGRNByName = '/GetGRNByName';
  public readonly deleteGRN = '/DeleteGRN';
  public readonly getGRNCode = '/GetGRNCode';
  public readonly processGRN = '/ProcessGRN';
  public readonly getIndentRequestCount = '/GetGRNCount';
  public readonly approveGRN = '/ApproveGRN';
  // public readonly getPendingDemand = '/getPendingDemand';
  public readonly getPendingInspectionItems = '/GetPendingInspectionItems';
  public readonly getPendingInspection = '/GetPendingInspection';
  public readonly getAllPurchaseInvoices = '/GetAllPurchaseInvoices';
  public readonly getPurchaseInvoiceCount = '/GetPurchaseInvoiceCount';
  public readonly approvePurchaseInvoice = '/ApprovePurchaseInvoice';
  public readonly updateWHTPercentage = '/UpdateWHTPercentage';
  public readonly getPendingCostSheet = '/GetPendingCostSheet';
  public readonly processPurchaseInvoice = '/ProcessPurchaseInvoice';
  public readonly rejectPurchaseInvoice = '/RejectPurchaseInvoice';
}