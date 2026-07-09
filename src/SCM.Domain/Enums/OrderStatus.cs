namespace SCM.Domain.Enums;

// NOTE: Supplier status and user roles are NOT enums - per the ER diagram they
// live in the StatusTypes and Roles tables respectively (StatusTypes because
// every status-bearing entity needs a full changeable history via
// StatusHistory; Roles because BR-09/FR-01.2 requires roles to be
// admin-manageable data, not hardcoded values).
//
// These enums remain as placeholders for modules not yet built
// (Orders/Logistics/Payments/Procurement - later weeks) and may be replaced
// by the same StatusTypes pattern when those modules are implemented, for
// consistency.
public enum OrderStatus { Pending, Approved, Reserved, Picking, Shipped, Delivered, Cancelled }
public enum ShipmentStatus { Created, InTransit, Delivered, Failed }
public enum PaymentStatus { Pending, Completed, Failed, Refunded }
public enum PurchaseRequestStatus { Draft, Submitted, Approved, Rejected }
public enum PurchaseOrderStatus { Issued, PartiallyDelivered, FullyDelivered, Cancelled }
